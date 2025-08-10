using Reactive.Expressions.Ast;
using Reactive.Expressions.Models;

namespace Reactive.Expressions.Parser;

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
