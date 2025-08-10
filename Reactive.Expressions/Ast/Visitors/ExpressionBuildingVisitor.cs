using System.Globalization;
using Antlr4.Runtime.Misc;
using Dawn;
using Reactive.Expressions.Grammar;
using Reactive.Expressions.Models;

namespace Reactive.Expressions.Ast.Visitors;

/// <summary>
/// ANTLR visitor implementation that converts the generated parse tree into our custom AST nodes.
/// This visitor implements the Visitor pattern to traverse the ANTLR parse tree and build
/// corresponding ExpressionNode objects that represent the semantic structure of the expression.
/// </summary>
/// <remarks>
/// The visitor handles:
/// - Converting logical operators (AND, OR) into BinaryOperatorNode objects
/// - Building ConditionNode objects for simple and aggregation conditions
/// - Processing arithmetic expressions with proper operator precedence
/// - Resolving variables and constants in threshold expressions
/// - Converting time window specifications into TimeSpan objects
///
/// This separation between parse tree (ANTLR's representation) and AST (our domain model)
/// allows for cleaner code and easier testing/manipulation of the expression structure.
/// </remarks>
public class ExpressionBuildingVisitor : DynamicExpressionBaseVisitor<ExpressionNode>
{
    /// <inheritdoc/>
    public override ExpressionNode VisitExpression(DynamicExpressionParser.ExpressionContext context)
    {
        Guard.Argument(context, nameof(context)).NotNull();

        return Visit(context.orExpression());
    }

    /// <inheritdoc/>
    public override ExpressionNode VisitOrExpression(DynamicExpressionParser.OrExpressionContext context)
    {
        Guard.Argument(context, nameof(context)).NotNull();

        var andExpressions = context.andExpression();
        if (andExpressions.Length == 1)
        {
            return Visit(andExpressions[0]);
        }

        ExpressionNode result = Visit(andExpressions[0]);
        for (int i = 1; i < andExpressions.Length; i++)
        {
            var right = Visit(andExpressions[i]);
            result = new BinaryOperatorNode(result, right, BinaryOperator.Or);
        }

        return result;
    }

    /// <inheritdoc/>
    public override ExpressionNode VisitAndExpression(DynamicExpressionParser.AndExpressionContext context)
    {
        Guard.Argument(context, nameof(context)).NotNull();

        var conditions = context.condition();
        if (conditions.Length == 1)
        {
            return Visit(conditions[0]);
        }

        ExpressionNode result = Visit(conditions[0]);
        for (int i = 1; i < conditions.Length; i++)
        {
            var right = Visit(conditions[i]);
            result = new BinaryOperatorNode(result, right, BinaryOperator.And);
        }

        return result;
    }

    /// <inheritdoc/>
    public override ExpressionNode VisitCondition(DynamicExpressionParser.ConditionContext context)
    {
        Guard.Argument(context, nameof(context)).NotNull();

        if (context.aggregationCondition() != null)
        {
            return VisitAggregationCondition(context.aggregationCondition());
        }
        else if (context.simpleCondition() != null)
        {
            return VisitSimpleCondition(context.simpleCondition());
        }
        else if (context.expression() != null)
        {
            return Visit(context.expression());
        }

        throw new ArgumentException("Invalid condition");
    }

    /// <inheritdoc/>
    public override ExpressionNode VisitAggregationCondition(DynamicExpressionParser.AggregationConditionContext context)
    {
        Guard.Argument(context, nameof(context)).NotNull();

        var aggregationType = context.aggregationType().GetText().ToLower(CultureInfo.InvariantCulture);
        var metricName = context.metricName().GetText();
        var timeWindow = ParseTimeWindow(context.timeWindow());
        var operatorText = context.@operator().GetText();
        var threshold = VisitThresholdInternal(context.threshold());

        var conditionInfo = new ConditionInfo
        {
            IsAggregation = true,
            AggregationType = aggregationType,
            MetricName = metricName,
            TimeWindow = timeWindow,
            Operator = operatorText,
            Threshold = threshold.constantValue,
            ThresholdExpression = threshold.expression,
            RawExpression = context.GetText(),
        };

        return new ConditionNode(conditionInfo);
    }

    /// <inheritdoc/>
    public override ExpressionNode VisitSimpleCondition(DynamicExpressionParser.SimpleConditionContext context)
    {
        Guard.Argument(context, nameof(context)).NotNull();

        var metricName = context.metricName().GetText();
        var operatorText = context.@operator().GetText();
        var threshold = VisitThresholdInternal(context.threshold());

        var conditionInfo = new ConditionInfo
        {
            IsAggregation = false,
            MetricName = metricName,
            Operator = operatorText,
            Threshold = threshold.constantValue,
            ThresholdExpression = threshold.expression,
            RawExpression = context.GetText(),
        };

        return new ConditionNode(conditionInfo);
    }

    /// <inheritdoc/>
    public override ExpressionNode VisitArithmeticExpression([NotNull] DynamicExpressionParser.ArithmeticExpressionContext context)
    {
        return base.VisitArithmeticExpression(context);
    }

    private static TimeSpan ParseTimeWindow(DynamicExpressionParser.TimeWindowContext context)
    {
        Guard.Argument(context, nameof(context)).NotNull();

        var number = int.Parse(context.NUMBER().GetText());
        var unit = context.timeUnit().GetText().ToLower(CultureInfo.InvariantCulture);

        return unit switch
        {
            "s" => TimeSpan.FromSeconds(number),
            "m" => TimeSpan.FromMinutes(number),
            "h" => TimeSpan.FromHours(number),
            _ => throw new ArgumentException($"Unknown time unit: {unit}"),
        };
    }

    private (double constantValue, ArithmeticExpression? expression) VisitThresholdInternal(DynamicExpressionParser.ThresholdContext context)
    {
        var arithmeticExpr = VisitArithmeticExpressionInternal(context.arithmeticExpression());

        // If it's a simple constant, return the value and null expression
        if (arithmeticExpr is ConstantExpression constant)
        {
            return (constant.Value, null);
        }

        // Otherwise, return the expression
        return (0, arithmeticExpr);
    }

    private ArithmeticExpression VisitArithmeticExpressionInternal(DynamicExpressionParser.ArithmeticExpressionContext context)
    {
        var expressions = context.multiplyDivideExpression();
        if (expressions.Length == 1)
        {
            return VisitMultiplyDivideExpressionInternal(expressions[0]);
        }

        ArithmeticExpression result = VisitMultiplyDivideExpressionInternal(expressions[0]);
        var operators = context.children.Where(c => c.GetText() == "+" || c.GetText() == "-").ToList();

        for (int i = 1; i < expressions.Length; i++)
        {
            var right = VisitMultiplyDivideExpressionInternal(expressions[i]);
            var operatorText = operators[i - 1].GetText();
            var op = operatorText == "+" ? ArithmeticOperator.Add : ArithmeticOperator.Subtract;
            result = new BinaryArithmeticExpression
            {
                Left = result,
                Right = right,
                Operator = op,
            };
        }

        return result;
    }

    private ArithmeticExpression VisitMultiplyDivideExpressionInternal(DynamicExpressionParser.MultiplyDivideExpressionContext context)
    {
        var expressions = context.primaryExpression();
        if (expressions.Length == 1)
        {
            return VisitPrimaryExpressionInternal(expressions[0]);
        }

        ArithmeticExpression result = VisitPrimaryExpressionInternal(expressions[0]);
        var operators = context.children.Where(c => c.GetText() == "*" || c.GetText() == "/").ToList();

        for (int i = 1; i < expressions.Length; i++)
        {
            var right = VisitPrimaryExpressionInternal(expressions[i]);
            var operatorText = operators[i - 1].GetText();
            var op = operatorText == "*" ? ArithmeticOperator.Multiply : ArithmeticOperator.Divide;
            result = new BinaryArithmeticExpression
            {
                Left = result,
                Right = right,
                Operator = op,
            };
        }

        return result;
    }

    private ArithmeticExpression VisitPrimaryExpressionInternal(DynamicExpressionParser.PrimaryExpressionContext context)
    {
        if (context is DynamicExpressionParser.NumberExpressionContext numberCtx)
        {
            return new ConstantExpression { Value = double.Parse(numberCtx.NUMBER().GetText()) };
        }
        else if (context is DynamicExpressionParser.VariableExpressionContext variableCtx)
        {
            return new VariableExpression { VariableName = variableCtx.IDENTIFIER().GetText() };
        }
        else if (context is DynamicExpressionParser.ParenthesizedArithmeticExpressionContext parenCtx)
        {
            return VisitArithmeticExpressionInternal(parenCtx.arithmeticExpression());
        }

        throw new ArgumentException("Invalid primary expression");
    }
}
