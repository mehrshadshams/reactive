using Dawn;

namespace Reactive.Expressions.Ast.Visitors;

using System.Collections.Generic;
using Reactive.Expressions.Ast;

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
