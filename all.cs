


namespace Reactive.Ext.Expressions.Ast;

using System;
using System.Collections.Generic;
using Reactive.Ext.Expressions.Ast.Visitors;

/// <summary>
/// Abstract base class for arithmetic expressions used in threshold calculations.
/// Supports evaluation with variable resolution and visitor pattern traversal.
/// </summary>
/// <remarks>
/// ArithmeticExpression enables complex threshold calculations like:
/// - Constants: "5.0"
/// - Variables: "threshold_multiplier"
/// - Binary operations: "threshold * 2.0"
/// - Nested expressions: "(base_threshold + offset) * multiplier"
///
/// The expression tree can be evaluated at runtime with variable substitution,
/// allowing for dynamic threshold calculations based on external configuration.
/// </remarks>
public abstract class ArithmeticExpression
{
    /// <summary>
    /// Evaluates this arithmetic expression to a numeric result.
    /// Variables are resolved using the provided resolver, if any.
    /// </summary>
    /// <param name="variableResolver">Optional resolver for variable values.</param>
    /// <returns>The computed numeric result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when variables cannot be resolved.</exception>
    public abstract double Evaluate(IVariableResolver? variableResolver = null);

    /// <summary>
    /// Accepts a visitor for traversing the expression tree using the Visitor pattern.
    /// This enables operations like validation, transformation, or analysis without
    /// modifying the expression classes themselves.
    /// </summary>
    /// <typeparam name="T">Return type of the visitor operation.</typeparam>
    /// <param name="visitor">Visitor implementation to apply.</param>
    /// <returns>Result of the visitor operation.</returns>
    public abstract T Accept<T>(IArithmeticVisitor<T> visitor);

    /// <summary>
    /// Extracts all variable names referenced in this expression and its sub-expressions.
    /// Used for dependency analysis and validation.
    /// </summary>
    /// <returns>Set of unique variable names found in the expression.</returns>
    public abstract HashSet<string> GetVariables();
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\ArithmeticOperator.cs =====



namespace Reactive.Ext.Expressions.Ast;

/// <summary>
/// Represents supported arithmetic operators for expression evaluation.
/// </summary>
public enum ArithmeticOperator
{
    /// <summary>
    /// Addition operator.
    /// </summary>
    Add,

    /// <summary>
    /// Subtraction operator.
    /// </summary>
    Subtract,

    /// <summary>
    /// Multiplication operator.
    /// </summary>
    Multiply,

    /// <summary>
    /// Division operator.
    /// </summary>
    Divide,
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\BinaryArithmeticExpression.cs =====



namespace Reactive.Ext.Expressions.Ast;

using System;
using System.Collections.Generic;
using Reactive.Ext.Expressions.Ast.Visitors;

/// <summary>
/// Binary arithmetic operation expression (e.g., 2*variable, variable+5).
/// </summary>
public class BinaryArithmeticExpression : ArithmeticExpression
{
    /// <summary>
    /// Gets or sets the left operand of the binary arithmetic expression.
    /// </summary>
    public required ArithmeticExpression Left { get; set; }

    /// <summary>
    /// Gets or sets the right operand of the binary arithmetic expression.
    /// </summary>
    public required ArithmeticExpression Right { get; set; }

    /// <summary>
    /// Gets or sets the arithmetic operator (Add, Subtract, Multiply, Divide, Modulo).
    /// </summary>
    public ArithmeticOperator Operator { get; set; }

    /// <inheritdoc/>
    public override double Evaluate(IVariableResolver? variableResolver = null)
    {
        var leftValue = Left.Evaluate(variableResolver);
        var rightValue = Right.Evaluate(variableResolver);

        return Operator switch
        {
            ArithmeticOperator.Add => leftValue + rightValue,
            ArithmeticOperator.Subtract => leftValue - rightValue,
            ArithmeticOperator.Multiply => leftValue * rightValue,
            ArithmeticOperator.Divide => rightValue != 0 ? leftValue / rightValue : throw new DivideByZeroException(),
            _ => throw new NotSupportedException($"Arithmetic operator {Operator} not supported"),
        };
    }

    /// <inheritdoc/>
    public override T Accept<T>(IArithmeticVisitor<T> visitor)
    {
        Guard.Argument(visitor, nameof(visitor)).NotNull();

        return visitor.VisitBinaryOperation(this);
    }

    /// <inheritdoc/>
    public override HashSet<string> GetVariables()
    {
        var variables = Left.GetVariables();
        variables.UnionWith(Right.GetVariables());
        return variables;
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\BinaryOperator.cs =====



namespace Reactive.Ext.Expressions.Ast;

/// <summary>
/// Binary operator for logical expressions.
/// </summary>
public enum BinaryOperator
{
    /// <summary>
    /// And operator, used to combine two conditions where both must be true.
    /// </summary>
    And,

    /// <summary>
    /// Or operator, used to combine two conditions where at least one must be true.
    /// </summary>
    Or,
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\BinaryOperatorNode.cs =====



namespace Reactive.Ext.Expressions;

using System;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Ext.Expressions.Ast;
using Reactive.Ext.Expressions.Ast.Visitors;
using Reactive.Ext.Expressions.Models;

/// <summary>
/// Binary operator node representing operations like AND/OR between two expressions.
/// </summary>
public class BinaryOperatorNode : ExpressionNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryOperatorNode"/> class.
    /// </summary>
    /// <param name="left">Left expression.</param>
    /// <param name="right">Right expression.</param>
    /// <param name="op">Operator.</param>
    public BinaryOperatorNode(ExpressionNode left, ExpressionNode right, BinaryOperator op)
    {
        Left = Guard.Argument(left, nameof(left)).NotNull();
        Right = Guard.Argument(right, nameof(right)).NotNull();
        Operator = op;
        Name = $"{op}_{left.Name}_{right.Name}";
    }

    /// <inheritdoc/>
    public override string Name { get; }

    /// <summary>
    /// Gets the left operand expression node.
    /// </summary>
    public ExpressionNode Left { get; private set; }

    /// <summary>
    /// Gets the right operand expression node.
    /// </summary>
    public ExpressionNode Right { get; private set; }

    /// <summary>
    /// Gets the binary operator (And, Or) used to combine the left and right expressions.
    /// </summary>
    public BinaryOperator Operator { get; private set; }

    /// <inheritdoc/>
    public override IObservable<EvaluationResult> Evaluate(MetricExpressionBuilder builder)
    {
        var leftObs = Left.Evaluate(builder);
        var rightObs = Right.Evaluate(builder);

        var result = Operator switch
        {
            BinaryOperator.And => leftObs.CombineLatest(rightObs, (a, b) => a.And(b)),
            BinaryOperator.Or => leftObs.CombineLatest(rightObs, (a, b) => a.Or(b)),
            _ => throw new NotSupportedException($"Operator {Operator} not supported"),
        };

        return result
            .Select(x =>
            {
                return x with { NodeName = Name };
            });
    }

    /// <inheritdoc/>
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        Guard.Argument(visitor, nameof(visitor)).NotNull();
        return visitor.VisitBinaryOperator(this);
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\ConditionNode.cs =====



namespace Reactive.Ext.Expressions.Ast;

using System;
using Reactive.Ext.Expressions;
using Reactive.Ext.Expressions.Ast.Visitors;
using Reactive.Ext.Expressions.Models;

/// <summary>
/// Represents a condition node in an expression tree.
/// </summary>
public class ConditionNode : ExpressionNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConditionNode"/> class with the specified condition information.
    /// </summary>
    /// <param name="condition">Condition.</param>
    public ConditionNode(ConditionInfo condition)
    {
        Condition = Guard.Argument(condition, nameof(condition)).NotNull();
        Name = $"{Guid.NewGuid():N}";
    }

    /// <summary>
    /// Gets the condition information that defines the metric comparison logic.
    /// </summary>
    public ConditionInfo Condition { get; private set; }

    /// <inheritdoc/>
    public override string Name { get; }

    /// <inheritdoc/>
    public override IObservable<EvaluationResult> Evaluate(MetricExpressionBuilder builder)
    {
        Guard.Argument(builder, nameof(builder)).NotNull();

        if (Condition.IsAggregation)
        {
            return builder.BuildSlidingWindowAggregation(this);
        }
        else
        {
            return builder.BuildSimpleConditionObservable(this);
        }
    }

    /// <inheritdoc/>
    public override T Accept<T>(IExpressionVisitor<T> visitor)
    {
        Guard.Argument(visitor, nameof(visitor)).NotNull();

        return visitor.VisitCondition(this);
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\ConstantExpression.cs =====



namespace Reactive.Ext.Expressions.Ast;

using System.Collections.Generic;
using Reactive.Ext.Expressions.Ast.Visitors;

/// <summary>
/// Represents a constant numeric value in an arithmetic expression.
/// </summary>
public class ConstantExpression : ArithmeticExpression
{
    /// <summary>
    /// Gets or sets the constant numeric value.
    /// </summary>
    public double Value { get; set; }

    /// <inheritdoc/>
    public override double Evaluate(IVariableResolver? variableResolver = null)
    {
        return Value;
    }

    /// <inheritdoc/>
    public override T Accept<T>(IArithmeticVisitor<T> visitor)
    {
        Guard.Argument(visitor, nameof(visitor)).NotNull();
        return visitor.VisitConstant(this);
    }

    /// <inheritdoc/>
    public override HashSet<string> GetVariables()
    {
        return new HashSet<string>();
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\ExpressionNode.cs =====



namespace Reactive.Ext.Expressions.Ast;

using System;
using Reactive.Ext.Expressions.Ast.Visitors;
using Reactive.Ext.Expressions.Models;

/// <summary>
/// AST Node types for expression tree.
/// </summary>
public abstract class ExpressionNode
{
    /// <summary>
    /// Gets the unique name of this expression node.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Evaluates this expression node within the context of the provided builder.
    /// </summary>
    /// <param name="builder">Expression builder.</param>
    /// <returns>Observable sequence.</returns>
    public abstract IObservable<EvaluationResult> Evaluate(MetricExpressionBuilder builder);

    /// <summary>
    /// Accepts a visitor for traversing the expression tree using the Visitor pattern.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="visitor">Visitor.</param>
    /// <returns>Transformed object.</returns>
    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\IVariableResolver.cs =====



namespace Reactive.Ext.Expressions.Ast;

using System.Collections.Generic;

/// <summary>
/// Interface for resolving variable names to numeric values during expression evaluation.
/// Implementations can provide static mappings, database lookups, or runtime calculations.
/// </summary>
public interface IVariableResolver
{
    /// <summary>
    /// Gets the set of all variable names known to this resolver.
    /// </summary>
    ISet<string> Variables { get; }

    /// <summary>
    /// Gets the value of a variable by its name.
    /// </summary>
    /// <param name="variableName">Variable name.</param>
    /// <returns>value.</returns>
    double? GetVariableValue(string variableName);

    /// <summary>
    /// Checks if a variable with the given name exists in the resolver.
    /// </summary>
    /// <param name="variableName">Variable name.</param>
    /// <returns><c>true</c> if variable exists.</returns>
    bool HasVariable(string variableName);
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\VariableExpression.cs =====



namespace Reactive.Ext.Expressions.Ast;

using System;
using System.Collections.Generic;
using Reactive.Ext.Expressions.Ast.Visitors;

/// <summary>
/// Represents a variable reference in an arithmetic expression that gets resolved at evaluation time.
/// Variables enable dynamic threshold calculations where values are determined at runtime rather than compile time.
/// </summary>
/// <remarks>
/// VariableExpression supports:
/// - Runtime variable resolution through IVariableResolver
/// - Dynamic threshold configuration without code changes
/// - Environment-specific parameter substitution
/// - Parameterized expression evaluation
///
/// Examples of variable usage:
/// - "cpu > threshold_multiplier" - where threshold_multiplier is resolved at runtime
/// - "memory > base_threshold * scaling_factor" - using multiple variables
/// - "disk > configuration_threshold" - environment-specific thresholds
///
/// Variables must be defined in the IVariableResolver or evaluation will throw an exception.
/// </remarks>
public class VariableExpression : ArithmeticExpression
{
    /// <summary>
    /// Gets or sets the name of the variable to be resolved at evaluation time.
    /// </summary>
    public required string VariableName { get; set; }

    /// <inheritdoc/>
    public override double Evaluate(IVariableResolver? variableResolver = null)
    {
        if (variableResolver == null)
        {
            throw new InvalidOperationException($"Variable '{VariableName}' cannot be resolved: no variable resolver provided");
        }

        var value = variableResolver.GetVariableValue(VariableName);
        if (value == null)
        {
            throw new InvalidOperationException($"Variable '{VariableName}' is not defined");
        }

        return value.Value;
    }

    /// <inheritdoc/>
    public override T Accept<T>(IArithmeticVisitor<T> visitor)
    {
        Guard.Argument(visitor, nameof(visitor)).NotNull();
        return visitor.VisitVariable(this);
    }

    /// <inheritdoc/>
    public override HashSet<string> GetVariables()
    {
        return new HashSet<string> { VariableName };
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\VariableResolver.cs =====



namespace Reactive.Ext.Expressions.Ast;

using System.Collections.Generic;
using System.Collections.Immutable;

/// <summary>
/// Simple implementation of IVariableResolver that stores variables in an in-memory dictionary.
/// This resolver allows dynamic assignment and retrieval of variable values used in arithmetic expressions.
/// </summary>
/// <remarks>
/// The VariableResolver provides:
/// - Variable storage and retrieval for expression evaluation
/// - Dynamic variable assignment at runtime
/// - Support for variable dependency checking
/// - Thread-safe variable access for concurrent expression evaluation
///
/// Variables are commonly used for:
/// - Dynamic threshold configuration: "cpu > threshold_multiplier"
/// - Environment-specific parameters: "memory > base_threshold * scaling_factor"
/// - Runtime configuration adjustments: "disk > (min_threshold + offset)".
/// </remarks>
public class VariableResolver : IVariableResolver
{
    private readonly Dictionary<string, double> _variables = new();

    /// <inheritdoc/>
    public ISet<string> Variables => _variables.Keys.ToImmutableHashSet();

    public void SetVariable(string name, double value)
    {
        _variables[name] = value;
    }

    /// <inheritdoc/>
    public double? GetVariableValue(string variableName)
    {
        return _variables.TryGetValue(variableName, out var value) ? value : null;
    }

    /// <inheritdoc/>
    public bool HasVariable(string variableName)
    {
        return _variables.ContainsKey(variableName);
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\Visitors\ComplexityAnalyzer.cs =====



namespace Reactive.Ext.Expressions.Ast.Visitors;

using System;
using Reactive.Ext.Expressions.Ast;
using Reactive.Ext.Expressions.Models;

/// <summary>
/// Analyzer that calculates various complexity metrics for expression trees.
/// Provides insights into expression structure that can be used for performance
/// optimization, cost estimation, and complexity management.
/// </summary>
/// <remarks>
/// The complexity analyzer computes:
/// - Total node count (overall expression size)
/// - Condition count (number of leaf conditions)
/// - Aggregation count (number of time-windowed aggregations)
/// - Maximum depth (nesting level)
/// - Operator count (logical AND/OR operations)
///
/// These metrics help with:
/// - Performance optimization decisions
/// - Resource allocation planning
/// - Expression complexity limits enforcement
/// - Cost-based query optimization
/// - Debugging and monitoring.
/// </remarks>
public class ComplexityAnalyzer : IExpressionVisitor<ExpressionComplexity>
{
    /// <summary>
    /// Analyzes the complexity of an expression tree starting from the root node.
    /// This is the main entry point for complexity analysis.
    /// </summary>
    /// <param name="expression">Root expression node to analyze.</param>
    /// <returns>ExpressionComplexity object containing various complexity metrics.</returns>
    public ExpressionComplexity AnalyzeComplexity(ExpressionNode expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Accept(this);
    }

    /// <summary>
    /// Visits a binary operator node and combines complexity metrics from both operands.
    /// Increments node count, operator count, and depth while combining child metrics.
    /// </summary>
    /// <param name="node">Binary operator node (AND/OR).</param>
    /// <returns>Combined complexity metrics including this operator.</returns>
    public ExpressionComplexity VisitBinaryOperator(BinaryOperatorNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var leftComplexity = node.Left.Accept(this);
        var rightComplexity = node.Right.Accept(this);

        return new ExpressionComplexity
        {
            NodeCount = leftComplexity.NodeCount + rightComplexity.NodeCount + 1,
            ConditionCount = leftComplexity.ConditionCount + rightComplexity.ConditionCount,
            AggregationCount = leftComplexity.AggregationCount + rightComplexity.AggregationCount,
            MaxDepth = Math.Max(leftComplexity.MaxDepth, rightComplexity.MaxDepth) + 1,
            OperatorCount = leftComplexity.OperatorCount + rightComplexity.OperatorCount + 1,
        };
    }

    /// <summary>
    /// Visits a condition node and returns its complexity metrics.
    /// Leaf nodes contribute 1 to node count and condition count, plus 1 to aggregation
    /// count if the condition involves an aggregation function.
    /// </summary>
    /// <param name="node">Condition node to analyze.</param>
    /// <returns>Complexity metrics for this leaf condition.</returns>
    public ExpressionComplexity VisitCondition(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        return new ExpressionComplexity
        {
            NodeCount = 1,
            ConditionCount = 1,
            AggregationCount = node.Condition.IsAggregation ? 1 : 0,
            MaxDepth = 1,
            OperatorCount = 0,
        };
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\Visitors\ExpressionBuildingVisitor.cs =====



using System.Globalization;
using Antlr4.Runtime.Misc;
using Reactive.Ext.Expressions.Grammar;
using Reactive.Ext.Expressions.Models;

namespace Reactive.Ext.Expressions.Ast.Visitors;

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

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\Visitors\ExpressionValidator.cs =====



namespace Reactive.Ext.Expressions.Ast.Visitors;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Ext.Expressions;
using Reactive.Ext.Expressions.Ast;
using Reactive.Ext.Expressions.Models;

/// <summary>
/// Expression validator that uses the Visitor pattern to traverse and validate AST nodes.
/// Checks for semantic correctness including known metrics, valid operators, and proper syntax.
/// </summary>
/// <remarks>
/// The validator performs:
/// - Metric name validation against known metrics
/// - Variable name validation against known variables
/// - Operator validity checking
/// - Aggregation function validation
/// - Threshold expression validation
/// - Time window format validation
///
/// Validation results include both errors (which make expressions invalid) and warnings
/// (which indicate potential issues but don't prevent execution).
/// </remarks>
public class ExpressionValidator : IExpressionVisitor<ValidationResult>
{
    /// <summary>
    /// Set of metric names that are considered valid for validation.
    /// </summary>
    private readonly HashSet<string> _knownMetrics;

    /// <summary>
    /// Set of variable names that are considered valid for validation.
    /// </summary>
    private readonly HashSet<string> _knownVariables;

    /// <summary>
    /// Set of valid aggregation function names.
    /// </summary>
    private readonly HashSet<string> _validAggregations = new HashSet<string> { "avg", "sum", "max", "min" };

    /// <summary>
    /// Set of valid comparison operators.
    /// </summary>
    private readonly HashSet<string> _validOperators = new HashSet<string> { ">", ">=", "<", "<=", "==", "!=" };

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionValidator"/> class.
    /// </summary>
    /// <param name="knownMetrics">Set of valid metric names (case-insensitive).</param>
    /// <param name="knownVariables">Set of valid variable names (case-insensitive).</param>
    public ExpressionValidator(ISet<string>? knownMetrics = null, ISet<string>? knownVariables = null)
    {
        _knownMetrics = new HashSet<string>(knownMetrics ?? new HashSet<string>(), StringComparer.OrdinalIgnoreCase);
        _knownVariables = new HashSet<string>(knownVariables ?? new HashSet<string>(), StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates an expression tree starting from the root node.
    /// This is the main entry point for expression validation.
    /// </summary>
    /// <param name="expression">Root node of the expression to validate.</param>
    /// <returns>ValidationResult containing errors, warnings, and validity status.</returns>
    public ValidationResult ValidateExpression(ExpressionNode expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Accept(this);
    }

    /// <summary>
    /// Validates a binary operator node (AND/OR operations).
    /// Recursively validates both operands and combines the results.
    /// </summary>
    /// <param name="node">Binary operator node to validate.</param>
    /// <returns>Combined validation result from both operands.</returns>
    public ValidationResult VisitBinaryOperator(BinaryOperatorNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var result = new ValidationResult { IsValid = true };

        // Validate left and right operands
        var leftResult = node.Left.Accept(this);
        var rightResult = node.Right.Accept(this);

        // Combine results
        result.IsValid = leftResult.IsValid && rightResult.IsValid;
        result.Errors.AddRange(leftResult.Errors);
        result.Errors.AddRange(rightResult.Errors);
        result.Warnings.AddRange(leftResult.Warnings);
        result.Warnings.AddRange(rightResult.Warnings);

        // Validate operator
        if (node.Operator != BinaryOperator.And && node.Operator != BinaryOperator.Or)
        {
            result.AddError($"Unknown binary operator: {node.Operator}");
        }

        return result;
    }

    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Ignore exceptions during validation")]
    public ValidationResult VisitCondition(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var result = new ValidationResult { IsValid = true };
        var condition = node.Condition!;

        // Validate metric name
        if (string.IsNullOrWhiteSpace(condition.MetricName))
        {
            result.AddError("Metric name cannot be null or empty");
        }
        else if (_knownMetrics.Any() && !_knownMetrics.Contains(condition.MetricName))
        {
            result.AddError($"Unknown metric: {condition.MetricName}");
        }

        // Validate operator
        if (!_validOperators.Contains(condition.Operator))
        {
            result.AddError($"Invalid operator: {condition.Operator}");
        }

        // Validate aggregation-specific fields
        if (condition.IsAggregation)
        {
            if (string.IsNullOrWhiteSpace(condition.AggregationType))
            {
                result.AddError("Aggregation type cannot be null or empty for aggregation conditions");
            }
            else if (!_validAggregations.Contains(condition.AggregationType, StringComparer.OrdinalIgnoreCase))
            {
                result.AddError($"Invalid aggregation type: {condition.AggregationType}");
            }

            if (condition.TimeWindow <= TimeSpan.Zero)
            {
                result.AddError("Time window must be greater than zero for aggregation conditions");
            }
            else if (condition.TimeWindow > TimeSpan.FromHours(24))
            {
                result.AddWarning("Time window greater than 24 hours may impact performance");
            }
        }
        else
        {
            // For non-aggregation conditions, these should be null/default
            if (!string.IsNullOrEmpty(condition.AggregationType))
            {
                result.AddWarning("Aggregation type is set for non-aggregation condition");
            }

            if (condition.TimeWindow != TimeSpan.Zero)
            {
                result.AddWarning("Time window is set for non-aggregation condition");
            }
        }

        // Validate threshold or threshold expression
        if (condition.HasVariableThreshold)
        {
            // Validate threshold expression and its variables
            var variables = condition.ThresholdExpression!.GetVariables();
            foreach (var variable in variables)
            {
                if (_knownVariables.Any() && !_knownVariables.Contains(variable))
                {
                    result.AddError($"Unknown variable: {variable}");
                }
            }

            // Try to evaluate the expression with default values to check for errors
            try
            {
                var testResolver = new VariableResolver();
                foreach (var variable in variables)
                {
                    testResolver.SetVariable(variable, 1.0); // Use test value
                }

                condition.ThresholdExpression.Evaluate(testResolver);
            }
            catch (Exception ex)
            {
                result.AddError($"Error in threshold expression: {ex.Message}");
            }
        }
        else
        {
            // Validate simple threshold
            if (double.IsNaN(condition.Threshold) || double.IsInfinity(condition.Threshold))
            {
                result.AddError("Threshold cannot be NaN or Infinity");
            }
        }

        return result;
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\Visitors\IArithmeticVisitor.cs =====


namespace Reactive.Ext.Expressions.Ast.Visitors;

/// <summary>
/// Visitor interface for arithmetic expressions.
/// </summary>
/// <typeparam name="T">Type parameter.</typeparam>
public interface IArithmeticVisitor<T>
{
    /// <summary>
    /// Visits a constant expression in the arithmetic expression tree.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <returns>Transformed type.</returns>
    T VisitConstant(ConstantExpression expression);

    /// <summary>
    /// Visits a variable expression in the arithmetic expression tree.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <returns>Transformed type.</returns>
    T VisitVariable(VariableExpression expression);

    /// <summary>
    /// Visits a binary expression in the arithmetic expression tree.
    /// </summary>
    /// <param name="expression">Expression.</param>
    /// <returns>Transformed type.</returns>
    T VisitBinaryOperation(BinaryArithmeticExpression expression);
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\Visitors\IExpressionVisitor.cs =====


namespace Reactive.Ext.Expressions.Ast.Visitors;

/// <summary>
/// Visitor interface for traversing and processing expression trees using the Visitor pattern.
/// Enables operations like validation, complexity analysis, metric collection, and variable extraction.
/// </summary>
/// <typeparam name="T">The type of result returned by visitor operations.</typeparam>
/// <remarks>
/// The Visitor pattern allows adding new operations to expression trees without modifying
/// the tree node classes themselves. Common implementations include:
/// - Validation visitors that check expression semantic correctness
/// - Complexity analyzers that compute performance metrics
/// - Metric collectors that extract metric dependencies
/// - Variable collectors that identify variable dependencies
/// - Code generators that convert expressions to other formats.
/// </remarks>
public interface IExpressionVisitor<T>
{
    /// <summary>
    /// Visits a binary operator node (AND/OR) in the expression tree.
    /// </summary>
    /// <param name="node">The binary operator node to visit.</param>
    /// <returns>The result of processing the binary operator node.</returns>
    T VisitBinaryOperator(BinaryOperatorNode node);

    /// <summary>
    /// Visits a condition node (leaf node containing metric comparisons) in the expression tree.
    /// </summary>
    /// <param name="node">The condition node to visit.</param>
    /// <returns>The result of processing the condition node.</returns>
    T VisitCondition(ConditionNode node);
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\Visitors\MetricCollector.cs =====



namespace Reactive.Ext.Expressions.Ast.Visitors;

using System.Collections.Generic;
using Reactive.Ext.Expressions.Ast;

/// <summary>
/// Visitor implementation that traverses an expression tree to collect all metric names.
/// This is useful for dependency analysis, validation, and understanding what metrics
/// an expression requires for evaluation.
/// </summary>
/// <remarks>
/// The collector uses the Visitor pattern to traverse the AST and accumulate
/// metric names from all condition nodes. This enables:
/// - Dependency tracking for expressions
/// - Validation against available metrics
/// - Resource planning for metric collection
/// - Performance optimization through selective metric streaming.
/// </remarks>
public class MetricCollector : IExpressionVisitor<HashSet<string>>
{
    /// <summary>
    /// Collects all metric names referenced in an expression tree.
    /// This is the main entry point for metric collection.
    /// </summary>
    /// <param name="expression">Root expression node to analyze.</param>
    /// <returns>HashSet containing all unique metric names found.</returns>
    public HashSet<string> CollectMetrics(ExpressionNode expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Accept(this);
    }

    /// <summary>
    /// Visits a binary operator node and combines metrics from both operands.
    /// </summary>
    /// <param name="node">Binary operator node (AND/OR).</param>
    /// <returns>Union of metrics from left and right operands.</returns>
    public HashSet<string> VisitBinaryOperator(BinaryOperatorNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var leftMetrics = node.Left.Accept(this);
        var rightMetrics = node.Right.Accept(this);

        leftMetrics.UnionWith(rightMetrics);
        return leftMetrics;
    }

    /// <summary>
    /// Visits a condition node and extracts the metric name.
    /// </summary>
    /// <param name="node">Condition node containing metric reference.</param>
    /// <returns>HashSet containing the metric name from this condition.</returns>
    public HashSet<string> VisitCondition(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();
        return new HashSet<string> { node.Condition.MetricName };
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Ast\Visitors\VariableCollector.cs =====



namespace Reactive.Ext.Expressions.Ast.Visitors;

using System.Collections.Generic;
using Reactive.Ext.Expressions.Ast;

/// <summary>
/// Visitor implementation that traverses an expression tree to collect all variable names
/// used in arithmetic threshold expressions. Variables enable dynamic threshold calculation
/// based on runtime configuration or external parameters.
/// </summary>
/// <remarks>
/// The variable collector identifies variables used in expressions like:
/// - "cpu > threshold_multiplier"
/// - "memory > base_threshold * scaling_factor"
/// - "disk > (min_threshold + offset) * multiplier"
///
/// This enables:
/// - Variable dependency analysis
/// - Runtime configuration validation
/// - Dynamic threshold parameter management
/// - Expression parameterization for different environments.
/// </remarks>
public class VariableCollector : IExpressionVisitor<HashSet<string>>
{
    /// <summary>
    /// Collects all variable names referenced in threshold expressions within an AST.
    /// This is the main entry point for variable collection.
    /// </summary>
    /// <param name="expression">Root expression node to analyze.</param>
    /// <returns>HashSet containing all unique variable names found.</returns>
    public HashSet<string> CollectVariables(ExpressionNode expression)
    {
        Guard.Argument(expression, nameof(expression)).NotNull();

        return expression.Accept(this);
    }

    /// <summary>
    /// Visits a binary operator node and combines variables from both operands.
    /// </summary>
    /// <param name="node">Binary operator node (AND/OR).</param>
    /// <returns>Union of variables from left and right operands.</returns>
    public HashSet<string> VisitBinaryOperator(BinaryOperatorNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var leftVariables = node.Left.Accept(this);
        var rightVariables = node.Right.Accept(this);

        leftVariables.UnionWith(rightVariables);
        return leftVariables;
    }

    /// <summary>
    /// Visits a condition node and extracts variables from its threshold expression (if any).
    /// Only conditions with variable-based thresholds contribute variables.
    /// </summary>
    /// <param name="node">Condition node to analyze.</param>
    /// <returns>HashSet containing variables from this condition's threshold expression.</returns>
    public HashSet<string> VisitCondition(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();

        var variables = new HashSet<string>();

        if (node.Condition.HasVariableThreshold)
        {
            variables.UnionWith(node.Condition.ThresholdExpression!.GetVariables());
        }

        return variables;
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\ExpressionComplexity.cs =====


// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\IExpressionVisitor.cs =====


// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\MetricExpressionBuilder.cs =====



namespace Reactive.Ext.Expressions;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Reactive.Ext.Expressions.Ast;
using Reactive.Ext.Expressions.Models;
using Reactive.Ext.Expressions.Parser;
using Microsoft.Azure.UsageBilling.Reactive;
using Microsoft.Extensions.Logging;

/// <summary>
/// Builds and manages reactive streams for metric-based expression evaluation.
/// Orchestrates metric data flow and expression evaluation in real-time streaming scenarios.
/// </summary>
/// <remarks>
/// MetricExpressionBuilder provides:
/// - Reactive stream management for metric data processing
/// - Expression evaluation against streaming metric data
/// - Time-windowed aggregation support for complex conditions
/// - Dynamic metric routing and filtering
/// - Variable resolution for parameterized expressions
///
/// The builder supports:
/// - Real-time metric stream processing
/// - Time-based aggregations (avg, sum, max, min, count over time windows)
/// - Expression evaluation triggering on metric updates
/// - Dynamic expression compilation and execution
/// - Multi-metric expression coordination
///
/// Typical workflow:
/// 1. Create builder with source metric stream and configuration
/// 2. Build expression trees using parser
/// 3. Evaluate expressions against streaming data
/// 4. Subscribe to evaluation results for real-time monitoring.
/// </remarks>
public class MetricExpressionBuilder
{
    private readonly IObservable<MetricData> _sourceStream;
    private readonly HashSet<string> _knownMetrics;
    private readonly ConcurrentDictionary<string, ISubject<MetricData>> _metricStreams;
    private readonly IVariableResolver? _variableResolver;
    private readonly MetricOptions _metricOptions;
    private readonly IExpressionParser _parser;
    private readonly ILogger<MetricExpressionBuilder> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricExpressionBuilder"/> class.
    /// </summary>
    /// <param name="sourceStream">The source stream of metric data to process.</param>
    /// <param name="parser">The expression parser for converting string expressions to AST.</param>
    /// <param name="logger">Logger for tracking builder operations and diagnostics.</param>
    /// <param name="knownMetrics">Optional set of known metric names for validation. If null, all metrics are accepted.</param>
    /// <param name="variableResolver">Optional resolver for dynamic variable substitution in expressions.</param>
    /// <param name="metricOptions">Optional configuration options for metric processing. Uses defaults if null.</param>
    public MetricExpressionBuilder(
        IObservable<MetricData> sourceStream,
        IExpressionParser parser,
        ILogger<MetricExpressionBuilder> logger,
        HashSet<string>? knownMetrics = null,
        IVariableResolver? variableResolver = null,
        MetricOptions? metricOptions = null)
    {
        _sourceStream = Guard.Argument(sourceStream, nameof(sourceStream)).NotNull().Value;
        _parser = Guard.Argument(parser, nameof(parser)).NotNull().Value;
        _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;

        _knownMetrics = new HashSet<string>(knownMetrics ?? new HashSet<string>(), StringComparer.OrdinalIgnoreCase);
        _metricStreams = new ConcurrentDictionary<string, ISubject<MetricData>>();
        _variableResolver = variableResolver;
        _metricOptions = metricOptions ?? new MetricOptions();
    }

    /// <summary>
    /// Builds a reactive observable stream for evaluating the specified expression against metric data.
    /// Validates the expression syntax and semantics before creating the evaluation stream.
    /// </summary>
    /// <param name="expression">The expression string to parse and evaluate (e.g., "cpu &gt. 0.8 AND memory &lt; 0.9").</param>
    /// <returns>An observable stream of evaluation results that emits when conditions change.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression has validation errors that prevent execution.</exception>
    public IObservable<EvaluationResult> BuildExpression(string expression)
    {
        // Validate the expression first
        var validation = _parser.ValidateExpression(expression, knownMetrics: _knownMetrics, knownVariables: _variableResolver?.Variables);
        if (!validation.IsValid)
        {
            throw new ArgumentException($"Invalid expression: {string.Join(", ", validation.Errors)}");
        }

        if (validation.Warnings.Any())
        {
            _logger.LogWarning(null, "Expression validation warnings: {warnings}", string.Join(", ", validation.Warnings));
        }

        // Parse the expression into an AST using the configured parser
        var ast = _parser.ParseExpression(expression);

        // Evaluate the AST
        return ast.Evaluate(this);
    }

    /// <summary>
    /// Builds a reactive observable for time-windowed aggregation conditions.
    /// Creates sliding window aggregations (avg, sum, max, min) over specified time periods.
    /// </summary>
    /// <param name="node">The condition node containing aggregation configuration and metric details.</param>
    /// <returns>An observable stream that emits evaluation results when aggregation windows complete.</returns>
    public IObservable<EvaluationResult> BuildSlidingWindowAggregation(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();
        var condition = node.Condition;
        var metricStream = GetMetricStream(condition.MetricName);

        return metricStream
            .Buffer(condition.TimeWindow, _metricOptions.TimeWindow) // Sliding window every second
            .Where(buffer => buffer.Any())
            .SelectMany(buffer => buffer.OrderBy(m => m.Timestamp).ToList())
            .WindowByTimestamp(metric => metric.Timestamp.Truncate(condition.TimeWindow).Ticks, condition.TimeWindow)
            .SelectMany(buffer =>
            {
                return buffer.ToList();
            }) // Flatten the buffered windows
            .Where(buffer => buffer.Any())
            .Select(buffer => buffer.OrderBy(m => m.Timestamp).ToList())
            .Select(buffer =>
            {
                var windowStart = buffer.First().Timestamp;
                var windowEnd = buffer.Last().Timestamp;

                // Calculate the aggregated value based on the specified aggregation type
                (double aggregatedValue, AggregationType aggregationType) = condition.AggregationType?.ToLower(CultureInfo.InvariantCulture) switch
                {
                    "avg" => (buffer.Average(m => m.Value), AggregationType.Average),
                    "sum" => (buffer.Sum(m => m.Value), AggregationType.Sum),
                    "max" => (buffer.Max(m => m.Value), AggregationType.Max),
                    "min" => (buffer.Min(m => m.Value), AggregationType.Min),
                    _ => throw new NotSupportedException($"Aggregation type '{condition.AggregationType}' not supported"),
                };
                return new AggregationResult(node.Name, aggregationType, new Period(windowStart.DateTime, windowEnd.DateTime), aggregatedValue);
            })
            .Select(aggregatedValue =>
            {
                bool result = EvaluateCondition(aggregatedValue.Value, condition);

                return new EvaluationResult(aggregatedValue.NodeName, result, aggregatedValue.Period);
            });
    }

    /// <summary>
    /// Builds a reactive observable for simple condition evaluation without time-based aggregation.
    /// Evaluates conditions immediately against each incoming metric value.
    /// </summary>
    /// <param name="node">The condition node containing the metric comparison logic.</param>
    /// <returns>An observable stream that emits evaluation results for each metric update.</returns>
    public IObservable<EvaluationResult> BuildSimpleConditionObservable(ConditionNode node)
    {
        Guard.Argument(node, nameof(node)).NotNull();
        var condition = node.Condition;
        var metricStream = GetMetricStream(condition.MetricName);

        return metricStream
            .Select(metric =>
            {
                bool result = EvaluateCondition(metric.Value, condition);

                return new EvaluationResult(condition.MetricName, result, Period.SinglePoint(metric.Timestamp.DateTime));
            });
    }

    private IObservable<MetricData> GetMetricStream(string metricName)
    {
        if (!_metricStreams.ContainsKey(metricName))
        {
            _metricStreams[metricName] = new Subject<MetricData>();

            // Subscribe to source and filter by metric name
            _sourceStream
                .Where(m => m.Name == metricName)
                .Subscribe(_metricStreams[metricName]);
        }

        return _metricStreams[metricName].AsObservable();
    }

    private bool EvaluateCondition(double value, ConditionInfo condition)
    {
        double threshold;

        if (condition.HasVariableThreshold)
        {
            try
            {
                threshold = condition.ThresholdExpression!.Evaluate(_variableResolver);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to evaluate threshold expression for condition '{Condition}', value='{Value}'", condition.RawExpression, value);
                throw;
            }
        }
        else
        {
            threshold = condition.Threshold;
        }

        return condition.Operator switch
        {
            ">" => value > threshold,
            ">=" => value >= threshold,
            "<" => value < threshold,
            "<=" => value <= threshold,
            "==" => Math.Abs(value - threshold) < 0.0001,
            "!=" => Math.Abs(value - threshold) >= 0.0001,
            _ => throw new ArgumentException($"Unknown operator: {condition.Operator}"),
        };
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\AggregationResult.cs =====



namespace Reactive.Ext.Expressions.Models;

/// <summary>
/// Represents the result of an aggregation operation over a time period.
/// Contains the aggregation type, time period, and computed value.
/// </summary>
/// <param name="NodeName">Identifier for the aggregation (usually the metric name).</param>
/// <param name="AggregationType">Type of aggregation performed (avg, sum, max, min, count).</param>
/// <param name="Period">Time period over which the aggregation was computed.</param>
/// <param name="Value">The computed aggregation result.</param>
public record AggregationResult(string NodeName, AggregationType AggregationType, Period Period, double Value);

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\AggregationType.cs =====



namespace Reactive.Ext.Expressions.Models;

/// <summary>
/// Type of aggregation to perform on metric data.
/// </summary>
public enum AggregationType
{
    /// <summary>
    /// Average value over the specified time period.
    /// </summary>
    Average,

    /// <summary>
    /// Sum of all values over the specified time period.
    /// </summary>
    Sum,

    /// <summary>
    /// Maximum value observed over the specified time period.
    /// </summary>
    Max,

    /// <summary>
    /// Minimum value observed over the specified time period.
    /// </summary>
    Min,
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\ConditionInfo.cs =====



namespace Reactive.Ext.Expressions.Models;

using System;
using Reactive.Ext.Expressions.Ast;

/// <summary>
/// Contains detailed information about a parsed condition from an expression.
/// This class represents both simple conditions (metric > threshold) and
/// complex aggregation conditions (avg(metric, timeWindow) > threshold).
/// </summary>
/// <remarks>
/// ConditionInfo supports:
/// - Simple metric conditions: "cpu > 0.8"
/// - Aggregation conditions: "avg(cpu, 5m) > 0.8"
/// - Variable threshold expressions: "memory > threshold * multiplier"
/// - Different comparison operators: >, >=. <, <=, ==, !=, ~
/// - Time window specifications for aggregations
/// </remarks>
public class ConditionInfo
{
    /// <summary>
    /// Gets or sets the name of the metric being evaluated (e.g., "cpu", "memory").
    /// </summary>
    public required string MetricName { get; set; }

    /// <summary>
    /// Gets or sets the comparison operator used in the condition (>, >=. <, <=, ==, !=, ~)
    /// </summary>
    public required string Operator { get; set; }

    /// <summary>
    /// Gets or sets the numeric threshold value when using constant thresholds.
    /// Used only when ThresholdExpression is null.
    /// </summary>
    public double Threshold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this condition involves an aggregation function.
    /// </summary>
    public bool IsAggregation { get; set; }

    /// <summary>
    /// Gets or sets the type of aggregation if IsAggregation is true (avg, sum, max, min, count).
    /// </summary>
    public string? AggregationType { get; set; }

    /// <summary>
    /// Gets or sets the time window for aggregation functions (e.g., 5 minutes for "avg(cpu, 5m)").
    /// </summary>
    public TimeSpan TimeWindow { get; set; }

    /// <summary>
    /// Gets or sets optional arithmetic expression for dynamic threshold calculation.
    /// When present, this takes precedence over the Threshold property.
    /// </summary>
    public ArithmeticExpression? ThresholdExpression { get; set; }

    /// <summary>
    /// Gets a value indicating whether this condition uses a variable/expression-based threshold
    /// instead of a constant threshold value.
    /// </summary>
    public bool HasVariableThreshold => ThresholdExpression != null;

    /// <summary>
    /// Gets or sets the original expression text that generated this condition (for debugging/logging).
    /// </summary>
    public string? RawExpression { get; set; }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\EvaluationResult.cs =====



namespace Reactive.Ext.Expressions.Models;

using System.Collections.Generic;
using Reactive.Ext.Expressions;

/// <summary>
/// Represents the result of evaluating a node in an expression tree.
/// Contains the node identifier, boolean evaluation result, and the time period over which the evaluation occurred.
/// </summary>
/// <param name="NodeName">The unique identifier for the expression node that was evaluated.</param>
/// <param name="Value">The boolean result of the node evaluation (true if condition was met, false otherwise).</param>
/// <param name="Period">The time period during which this evaluation result is valid.</param>
public record EvaluationResult(string NodeName, bool Value, Period Period)
{
    /// <summary>
    /// Gets the default equality comparer for EvaluationResult instances.
    /// Uses EvaluationResultComparer for consistent comparison logic.
    /// </summary>
    public static readonly IEqualityComparer<EvaluationResult> DefaultComparer = new EvaluationResultComparer();

    /// <summary>
    /// Combines this evaluation result with another using logical AND operation.
    /// The result is true only if both this and the other evaluation are true.
    /// </summary>
    /// <param name="other">The other evaluation result to combine with.</param>
    /// <returns>A new EvaluationResult representing the AND combination of both results.</returns>
    public EvaluationResult And(EvaluationResult other)
    {
        Guard.Argument(other, nameof(other)).NotNull();

        bool value = Value && other.Value;
        string name = $"and_{NodeName}_{other.NodeName}";
        return CombineWith(other, name, value);
    }

    /// <summary>
    /// Combines this evaluation result with another using logical OR operation.
    /// The result is true if either this or the other evaluation (or both) are true.
    /// </summary>
    /// <param name="other">The other evaluation result to combine with.</param>
    /// <returns>A new EvaluationResult representing the OR combination of both results.</returns>
    public EvaluationResult Or(EvaluationResult other)
    {
        Guard.Argument(other, nameof(other)).NotNull();

        bool value = Value || other.Value;
        string name = $"or_{NodeName}_{other.NodeName}";
        return CombineWith(other, name, value);
    }

    private EvaluationResult CombineWith(EvaluationResult other, string name, bool value)
    {
        Guard.Argument(other, nameof(other)).NotNull();

        return new EvaluationResult(name, value, Period.Join(Period, other.Period));
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\EvaluationResultComparer.cs =====



namespace Reactive.Ext.Expressions.Models;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Comparer for EvaluationResult records.
/// </summary>
public class EvaluationResultComparer : IEqualityComparer<EvaluationResult>
{
    /// <inheritdoc/>
    public bool Equals(EvaluationResult? x, EvaluationResult? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        if (x == null || y == null)
        {
            return false;
        }

        return Equals(x.Value, y.Value);
    }

    /// <inheritdoc/>
    public int GetHashCode([DisallowNull] EvaluationResult obj)
    {
        Guard.Argument(obj, nameof(obj)).NotNull();

        return obj.Value.GetHashCode();
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\ExpressionComplexity.cs =====



namespace Reactive.Ext.Expressions.Models;

/// <summary>
/// Represents complexity metrics for an expression tree, providing insights into expression structure
/// and computational requirements for performance optimization and resource planning.
/// </summary>
/// <remarks>
/// ExpressionComplexity tracks:
/// - NodeCount: Total number of nodes in the expression tree
/// - ConditionCount: Number of leaf condition nodes (actual metric comparisons)
/// - AggregationCount: Number of time-windowed aggregation operations
/// - MaxDepth: Maximum nesting depth of the expression tree
/// - OperatorCount: Number of logical operators (AND/OR)
///
/// These metrics help with:
/// - Performance optimization decisions
/// - Resource allocation planning
/// - Expression complexity limits enforcement
/// - Cost-based query optimization
/// - Debugging and monitoring expression evaluation performance
///
/// The IsHighComplexity property provides a quick check for expressions that may
/// require special handling due to computational or memory requirements.
/// </remarks>
public class ExpressionComplexity
{
    /// <summary>
    /// Gets or sets the total number of nodes in the expression tree.
    /// Includes all nodes: conditions, operators, constants, and variables.
    /// </summary>
    public int NodeCount { get; set; }

    /// <summary>
    /// Gets or sets the number of leaf condition nodes that perform actual metric comparisons.
    /// Each condition represents a comparison between a metric and a threshold value.
    /// </summary>
    public int ConditionCount { get; set; }

    /// <summary>
    /// Gets or sets the number of time-windowed aggregation operations in the expression.
    /// Aggregations include operations like AVG, SUM, MAX, MIN over time windows.
    /// </summary>
    public int AggregationCount { get; set; }

    /// <summary>
    /// Gets or sets the maximum nesting depth of the expression tree.
    /// Represents the longest path from root to any leaf node.
    /// </summary>
    public int MaxDepth { get; set; }

    /// <summary>
    /// Gets or sets the number of logical operators (AND, OR) in the expression.
    /// Does not include arithmetic operators, only boolean logic operators.
    /// </summary>
    public int OperatorCount { get; set; }

    /// <summary>
    /// Gets a value indicating whether this expression is considered high complexity.
    /// Returns true if NodeCount > 20, MaxDepth > 10, or AggregationCount > 5.
    /// High complexity expressions may require special handling or optimization.
    /// </summary>
    public bool IsHighComplexity => NodeCount > 20 || MaxDepth > 10 || AggregationCount > 5;
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\MetricData.cs =====



namespace Reactive.Ext.Expressions.Models;

using System;
using Microsoft.Azure.UsageBilling.Reactive;

/// <summary>
/// Represents a single metric data point with name, value, and timestamp.
/// This is the fundamental data unit that flows through the metric processing pipeline.
/// </summary>
/// <remarks>
/// MetricData objects are used to:
/// - Transport metric values from data sources
/// - Trigger expression evaluations based on new data
/// - Maintain temporal ordering for time-based aggregations
/// - Provide context for metric-based conditions.
/// </remarks>
public class MetricData : ITimestamped
{
    /// <summary>
    /// Gets or sets the name/identifier of the metric (e.g., "cpu", "memory", "disk_usage").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the numeric value of the metric at the given timestamp.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when this metric value was recorded.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Returns a string representation of the metric data for debugging and logging.
    /// </summary>
    public override string ToString()
    {
        return $"{Name}: {Value} at {Timestamp}";
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\MetricOptions.cs =====



namespace Reactive.Ext.Expressions.Models;

using System;

/// <summary>
/// Configuration options for metric processing and evaluation.
/// Controls timing, windows, and other operational parameters.
/// </summary>
public class MetricOptions
{
    /// <summary>
    /// Gets or sets default time window for metric processing when not specified in expressions.
    /// This affects how metric streams are buffered and processed.
    /// </summary>
    public TimeSpan TimeWindow { get; set; } = TimeSpan.FromSeconds(5);
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\Period.cs =====



namespace Reactive.Ext.Expressions;

using System;

/// <summary>
/// Represents a time period with start and end timestamps.
/// Used for time-windowed aggregations and result tracking.
/// </summary>
/// <param name="Start">The start timestamp of the period.</param>
/// <param name="End">The end timestamp of the period.</param>
public record Period
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Period"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    public Period(DateTime start, DateTime end)
    {
        Guard.Argument(start, nameof(start)).NotDefault();
        Guard.Argument(end, nameof(end)).NotDefault();
        if (end < start)
        {
            throw new ArgumentException("End time must be greater than or equal to start time.");
        }

        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the start time of this period in ISO 8601 format.
    /// </summary>
    public DateTime Start { get; init; }

    /// <summary>
    /// Gets the end time of this period in ISO 8601 format.
    /// </summary>
    public DateTime End { get; init; }

    /// <summary>
    /// Gets the duration of this time period.
    /// </summary>
    public TimeSpan Duration => End - Start;

    /// <summary>
    /// Gets a value indicating whether gets whether this period represents an empty/zero duration.
    /// </summary>
    public bool IsEmpty => Duration == TimeSpan.Zero;

    /// <summary>
    /// Joins two periods to create a period that encompasses both.
    /// Returns the earliest start time and latest end time.
    /// </summary>
    /// <param name="left">First period to join.</param>
    /// <param name="right">Second period to join.</param>
    /// <returns>Combined period spanning both input periods.</returns>
    public static Period Join(Period left, Period right)
    {
        Guard.Argument(left, nameof(left)).NotNull();
        Guard.Argument(right, nameof(right)).NotNull();

        if (left.IsEmpty)
        {
            return right;
        }

        if (right.IsEmpty)
        {
            return left;
        }

        return new Period(
            left.Start < right.Start ? left.Start : right.Start,
            left.End > right.End ? left.End : right.End);
    }

    /// <summary>
    /// Checks if a specific timestamp falls within this period (inclusive).
    /// </summary>
    /// <param name="time">Timestamp to check.</param>
    /// <returns>True if the time is within the period bounds.</returns>
    public bool Contains(DateTime time)
    {
        return time >= Start && time <= End;
    }

    /// <summary>
    /// Returns a string representation of the period in ISO 8601 format.
    /// </summary>
    public override string ToString()
    {
        return $"{Start:O}..{End:O}";
    }

    /// <summary>
    /// Creates a period representing a single point in time (start == end).
    /// </summary>
    /// <param name="timestamp">The timestamp for the single-point period.</param>
    /// <returns>Period with identical start and end times.</returns>
    public static Period SinglePoint(DateTime timestamp) => new Period(timestamp, timestamp);
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Models\ValidationResult.cs =====



namespace Reactive.Ext.Expressions.Models;

using System.Collections.Generic;

/// <summary>
/// Contains the results of expression validation including errors, warnings, and overall validity status.
/// Used to provide detailed feedback about semantic correctness of parsed expressions.
/// </summary>
/// <remarks>
/// ValidationResult provides:
/// - Overall validity flag indicating if expression can be safely executed
/// - Detailed error messages for issues that prevent execution
/// - Warning messages for potential issues that don't prevent execution
/// - Structured feedback for debugging and user guidance
///
/// Typical validation checks include:
/// - Metric name validation against known metrics
/// - Variable name validation against known variables
/// - Operator syntax and semantic correctness
/// - Time window format validation
/// - Aggregation function parameter validation.
/// </remarks>
public class ValidationResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the expression validation passed without errors.
    /// False if any errors were encountered during validation.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Gets or sets the list of error messages that prevent the expression from being executed.
    /// Each error represents a blocking issue that must be resolved.
    /// </summary>
    public List<string> Errors { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets the list of warning messages about potential issues that don't prevent execution.
    /// Warnings highlight areas for improvement or potential problems.
    /// </summary>
    public List<string> Warnings { get; set; } = new List<string>();

    /// <summary>
    /// Adds an error message to the validation result and sets IsValid to false.
    /// </summary>
    /// <param name="error">The error message to add.</param>
    public void AddError(string error)
    {
        IsValid = false;
        Errors.Add(error);
    }

    /// <summary>
    /// Adds a warning message to the validation result without affecting validity.
    /// </summary>
    /// <param name="warning">The warning message to add.</param>
    public void AddWarning(string warning)
    {
        Warnings.Add(warning);
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Parser\AntlrExpressionParser.cs =====



using Antlr4.Runtime;
using Reactive.Ext.Expressions.Ast;
using Reactive.Ext.Expressions.Ast.Visitors;
using Reactive.Ext.Expressions.Models;
using Microsoft.Extensions.Logging;

namespace Reactive.Ext.Expressions.Parser;

/// <summary>
/// ANTLR-based implementation of IExpressionParser that uses a generated parser from a grammar file.
/// This parser provides robust, formally-defined parsing capabilities with excellent error handling
/// and is automatically generated from the DynamicExpression.g4 grammar file.
/// </summary>
/// <remarks>
/// This implementation leverages ANTLR 4 to:
/// - Generate lexer and parser from grammar definition
/// - Provide consistent parsing behavior
/// - Handle complex expressions with proper precedence
/// - Generate detailed parse trees
/// - Support visitor pattern for AST traversal
///
/// The parser supports the full expression grammar including:
/// - Boolean logical operations (AND, OR)
/// - Comparison operations (> >=. < <= == !=)
/// - Aggregation functions (avg, sum, max, min, count)
/// - Time windows (5s, 10m, 1h)
/// - Arithmetic expressions with variables
/// - Parentheses for grouping.
/// </remarks>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "testing.")]
public class AntlrExpressionParser : IExpressionParser
{
    private readonly ILogger<AntlrExpressionParser> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AntlrExpressionParser"/> class.
    /// </summary>
    /// <param name="logger">Logger.</param>
    public AntlrExpressionParser(ILogger<AntlrExpressionParser> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets the display name of this parser implementation.
    /// </summary>
    public string ParserName => nameof(AntlrExpressionParser);

    /// <summary>
    /// Parses an expression string into an Abstract Syntax Tree using ANTLR-generated lexer and parser.
    /// This method creates the lexer/parser pipeline, configures error handling, and uses a visitor
    /// to build the AST from the parse tree.
    /// </summary>
    /// <param name="expression">The expression string to parse.</param>
    /// <returns>Root ExpressionNode of the parsed AST.</returns>
    /// <exception cref="ArgumentException">Thrown when syntax errors are encountered during parsing.</exception>
    public ExpressionNode ParseExpression(string expression)
    {
        var inputStream = new AntlrInputStream(expression);
        var lexer = new Grammar.DynamicExpressionLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new Grammar.DynamicExpressionParser(tokenStream);

        // Add error handling
        parser.RemoveErrorListeners();
        var errorListener = new ThrowingErrorListener();
        parser.AddErrorListener(errorListener);

        var tree = parser.expression();
        var visitor = new ExpressionBuildingVisitor();
        return visitor.Visit(tree);
    }

    /// <summary>
    /// Validates an expression using the ANTLR parser to check for syntax errors and semantic validity.
    /// </summary>
    /// <param name="expression">Expression to validate.</param>
    /// <param name="knownMetrics">Optional set of known metrics for semantic validation.</param>
    /// <param name="knownVariables">Optional set of known variables for semantic validation.</param>
    /// <returns>ValidationResult containing any errors or warnings.</returns>
    public ValidationResult ValidateExpression(string expression, ISet<string>? knownMetrics = null, ISet<string>? knownVariables = null)
    {
        try
        {
            var ast = ParseExpression(expression);
            var validator = new ExpressionValidator(knownMetrics, knownVariables);
            return validator.ValidateExpression(ast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating expression: {Expression}", expression);
            var result = new ValidationResult();
            result.AddError($"Parse error: {ex.Message}");
            return result;
        }
    }

    /// <inheritdoc/>
    public HashSet<string> ExtractMetrics(string expression)
    {
        try
        {
            var ast = ParseExpression(expression);
            var collector = new MetricCollector();
            return collector.CollectMetrics(ast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exc expression: {Expression}", expression);
            return new HashSet<string>();
        }
    }

    /// <inheritdoc/>
    public HashSet<string> ExtractVariables(string expression)
    {
        try
        {
            var ast = ParseExpression(expression);
            var collector = new VariableCollector();
            return collector.CollectVariables(ast);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting variables from expression: {Expression}", expression);
            return new HashSet<string>();
        }
    }

    /// <inheritdoc/>
    public ExpressionComplexity AnalyzeComplexity(string expression)
    {
        try
        {
            var ast = ParseExpression(expression);
            var analyzer = new ComplexityAnalyzer();
            return analyzer.AnalyzeComplexity(ast);
        }
        catch
        {
            return new ExpressionComplexity { NodeCount = 0 };
        }
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Parser\IExpressionParser.cs =====



using Reactive.Ext.Expressions.Ast;
using Reactive.Ext.Expressions.Models;

namespace Reactive.Ext.Expressions.Parser;

/// <summary>
/// Interface for expression parsers that can parse dynamic expressions into Abstract Syntax Tree (AST) nodes.
/// This interface provides a unified API for different parser implementations (hand-crafted or ANTLR-based)
/// to parse metric expressions, validate them, and extract metadata.
/// </summary>
/// <remarks>
/// The interface supports parsing expressions like:
/// - Simple conditions: "cpu > 0.8"
/// - Complex logical expressions: "cpu > 0.8 && memory. < 0.9"
/// - Aggregation conditions: "avg(cpu, 5m) > threshold"
/// - Variable expressions: "metric_x > variable_name * 2"
/// </remarks>
public interface IExpressionParser
{
    /// <summary>
    /// Gets get the name/identifier of this parser implementation.
    /// Useful for debugging, logging, and performance comparisons between different parsers.
    /// </summary>
    /// <returns>A string identifying the parser type (e.g., "Hand-Crafted Recursive Descent Parser").</returns>
    string ParserName { get; }

    /// <summary>
    /// Parse an expression string into an ExpressionNode Abstract Syntax Tree (AST).
    /// </summary>
    /// <param name="expression">The expression string to parse (e.g., "cpu > 0.8 && memory. < 0.9")</param>
    /// <returns>The root ExpressionNode of the parsed AST representing the expression structure.</returns>
    /// <exception cref="ArgumentException">Thrown when the expression has syntax errors.</exception>
    /// <exception cref="ArgumentNullException">Thrown when expression is null.</exception>
    /// <example>
    /// <code>
    /// var parser = new HandCraftedExpressionParser();
    /// var ast = parser.ParseExpression("cpu > 0.8");
    /// // Returns a ConditionNode with metric "cpu", operator ">", threshold 0.8
    /// </code>
    /// </example>
    ExpressionNode ParseExpression(string expression);

    /// <summary>
    /// Validate an expression for syntax correctness and semantic validity without fully parsing it.
    /// This method checks for known metrics, variables, and proper syntax structure.
    /// </summary>
    /// <param name="expression">The expression string to validate.</param>
    /// <param name="knownMetrics">Optional set of known metric names for validation. If null, metric validation is skipped.</param>
    /// <param name="knownVariables">Optional set of known variable names for validation. If null, variable validation is skipped.</param>
    /// <returns>ValidationResult containing errors, warnings, and overall validity status.</returns>
    /// <example>
    /// <code>
    /// var knownMetrics = new HashSet&lt;string&gt; { "cpu", "memory" };
    /// var result = parser.ValidateExpression("cpu > 0.8 && unknown_metric &lt; 0.5", knownMetrics);
    /// // result.IsValid = false, result.Errors contains "Unknown metric: unknown_metric"
    /// </code>
    /// </example>
    ValidationResult ValidateExpression(string expression, ISet<string>? knownMetrics = null, ISet<string>? knownVariables = null);

    /// <summary>
    /// Extract all metric names referenced in the expression.
    /// This is useful for dependency analysis and metric validation.
    /// </summary>
    /// <param name="expression">The expression string to analyze.</param>
    /// <returns>HashSet of unique metric names found in the expression.</returns>
    /// <example>
    /// <code>
    /// var metrics = parser.ExtractMetrics("avg(cpu, 5m) > 0.8 && max(memory, 1h) < 0.9");
    /// // Returns: {"cpu", "memory"}
    /// </code>
    /// </example>
    HashSet<string> ExtractMetrics(string expression);

    /// <summary>
    /// Extract all variable names referenced in arithmetic expressions within conditions.
    /// Variables are used in threshold expressions and can be resolved at runtime.
    /// </summary>
    /// <param name="expression">The expression string to analyze.</param>
    /// <returns>HashSet of unique variable names found in the expression.</returns>
    /// <example>
    /// <code>
    /// var variables = parser.ExtractVariables("cpu > threshold * multiplier");
    /// // Returns: {"threshold", "multiplier"}
    /// </code>
    /// </example>
    HashSet<string> ExtractVariables(string expression);

    /// <summary>
    /// Analyze the structural complexity of the expression by counting nodes and specific constructs.
    /// This is useful for performance optimization and complexity metrics.
    /// </summary>
    /// <param name="expression">The expression string to analyze.</param>
    /// <returns>ExpressionComplexity object containing various complexity metrics.</returns>
    /// <example>
    /// <code>
    /// var complexity = parser.AnalyzeComplexity("cpu > 0.8 && (memory < 0.9 || disk > 0.95)");
    /// // Returns: NodeCount = 5, ConditionCount = 3, etc.
    /// </code>
    /// </example>
    ExpressionComplexity AnalyzeComplexity(string expression);
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\Parser\ThrowingErrorListener.cs =====



using Antlr4.Runtime;

namespace Reactive.Ext.Expressions.Parser;

/// <summary>
/// Custom ANTLR error listener that throws exceptions instead of just logging errors.
/// This provides immediate feedback when syntax errors are encountered during parsing,
/// making it easier to handle parse failures in calling code.
/// </summary>
public class ThrowingErrorListener : BaseErrorListener
{
    /// <summary>
    /// Overrides the default syntax error handler to throw an exception instead of logging.
    /// This converts ANTLR's default error reporting mechanism into exceptions that can be caught
    /// and handled by the calling application.
    /// </summary>
    /// <param name="recognizer">The parser that encountered the error.</param>
    /// <param name="offendingSymbol">The token that caused the error.</param>
    /// <param name="line">Line number where the error occurred.</param>
    /// <param name="charPositionInLine">Character position in the line where error occurred.</param>
    /// <param name="msg">Error message from ANTLR.</param>
    /// <param name="e">The recognition exception that was thrown.</param>
    /// <exception cref="ArgumentException">Always thrown with detailed error location and message.</exception>
    public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        throw new ArgumentException($"Syntax error at line {line}, position {charPositionInLine}: {msg}");
    }
}

// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\ValidationResult.cs =====


// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\VariableExpression.cs =====


// ===== File: C:\Code\UB\src\Reactive.Ext.Expressions\VariableResolver.cs =====

