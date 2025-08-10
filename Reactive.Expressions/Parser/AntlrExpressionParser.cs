using Antlr4.Runtime;
using Reactive.Expressions.Ast;
using Reactive.Expressions.Ast.Visitors;
using Reactive.Expressions.Models;
using Microsoft.Extensions.Logging;

namespace Reactive.Expressions.Parser;

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
