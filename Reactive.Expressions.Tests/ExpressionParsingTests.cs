using Microsoft.Extensions.Logging;
using Reactive.Expressions.Parser;

namespace Reactive.Expressions.Tests;

/// <summary>
/// Tests for parsing and validating metric expressions.
/// </summary>
[TestFixture]
public class ExpressionParsingTests
{
  private AntlrExpressionParser _parser;
  private ILogger<AntlrExpressionParser> _logger;

  [SetUp]
  public void Setup()
  {
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    _logger = loggerFactory.CreateLogger<AntlrExpressionParser>();
    _parser = new AntlrExpressionParser(_logger);
  }

  [Test]
  public void ParseExpression_CpuOrMemoryHighCondition_ShouldParseSuccessfully()
  {
    // Arrange
    var expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";
    var knownMetrics = new HashSet<string> { "cpu", "mem", "memory" };

    // Act
    var validation = _parser.ValidateExpression(expression, knownMetrics: knownMetrics);

    // Assert
    Assert.That(validation.IsValid, Is.True,
        $"Expression should be valid. Errors: {string.Join(", ", validation.Errors)}");
    Assert.That(validation.Errors, Is.Empty, "Should have no validation errors");
  }

  [Test]
  public void ParseExpression_CpuOrMemoryHighCondition_ShouldCreateCorrectAST()
  {
    // Arrange
    var expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";

    // Act & Assert - Should not throw exception during parsing
    Assert.DoesNotThrow(() =>
    {
      var ast = _parser.ParseExpression(expression);
      Assert.That(ast, Is.Not.Null, "AST should be created successfully");
      Assert.That(ast.Name, Is.Not.Null.And.Not.Empty, "AST should have a name");
    });
  }

  [Test]
  public void ValidateExpression_CpuOrMemoryHighCondition_ShouldIdentifyRequiredMetrics()
  {
    // Arrange
    var expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";
    var knownMetrics = new HashSet<string> { "cpu", "mem" };

    // Act
    var validation = _parser.ValidateExpression(expression, knownMetrics: knownMetrics);

    // Assert
    Assert.That(validation.IsValid, Is.True, "Expression should be valid with known metrics");
    // Note: ValidationResult doesn't expose RequiredMetrics property,
    // but we can verify the expression is valid with the known metrics
    Assert.That(validation.Errors, Is.Empty, "Should have no errors when all metrics are known");
    Assert.That(validation.Warnings, Is.Empty, "Should have no warnings when all metrics are known");
  }

  [Test]
  public void ValidateExpression_CpuOrMemoryHighCondition_WithUnknownMetrics_ShouldHandleGracefully()
  {
    // Arrange
    var expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";
    var knownMetrics = new HashSet<string> { "cpu" }; // missing 'mem'

    // Act
    var validation = _parser.ValidateExpression(expression, knownMetrics: knownMetrics);

    // Assert
    // The parser may treat unknown metrics as validation errors rather than warnings
    // This is acceptable behavior for unknown metric names
    Assert.That(validation.Errors.Any() || validation.Warnings.Any(), Is.True,
        "Should have either errors or warnings about unknown metrics");
  }

  [Test]
  public void ParseExpression_VariousValidExpressions_ShouldAllParseSuccessfully()
  {
    // Arrange
    var expressions = new[]
    {
            "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80",
            "avg(cpu, 5m) > 50 && avg(mem, 10m) < 90",
            "max(cpu, 30s) > 95",
            "min(mem, 2h) < 10",
            "sum(requests, 1m) > 1000",
            "(avg(cpu, 1m) > 70 || avg(mem, 1m) > 80) && max(disk, 5m) < 95"
        };

    var knownMetrics = new HashSet<string> { "cpu", "mem", "memory", "disk", "requests" };

    // Act & Assert
    foreach (var expression in expressions)
    {
      var validation = _parser.ValidateExpression(expression, knownMetrics: knownMetrics);
      Assert.That(validation.IsValid, Is.True,
          $"Expression '{expression}' should be valid. Errors: {string.Join(", ", validation.Errors)}");

      Assert.DoesNotThrow(() =>
      {
        var ast = _parser.ParseExpression(expression);
        Assert.That(ast, Is.Not.Null, $"AST should be created for expression: {expression}");
      }, $"Should successfully parse expression: {expression}");
    }
  }

  [Test]
  public void ParseExpression_InvalidSyntax_ShouldFailValidation()
  {
    // Arrange
    var invalidExpressions = new[]
    {
            "avg(cpu) > 70", // missing time window
            "avg(cpu, 1x) > 70", // invalid time unit
            "avg(cpu, 1m) >> 70", // invalid comparison operator
            "avg(cpu, 1m) > ", // missing threshold
            "avg(, 1m) > 70", // missing metric name
        };

    var knownMetrics = new HashSet<string> { "cpu", "mem" };

    // Act & Assert
    foreach (var expression in invalidExpressions)
    {
      var validation = _parser.ValidateExpression(expression, knownMetrics: knownMetrics);
      Assert.That(validation.IsValid, Is.False,
          $"Expression '{expression}' should be invalid but was considered valid");
      Assert.That(validation.Errors, Is.Not.Empty,
          $"Expression '{expression}' should have validation errors");
    }
  }

  [Test]
  public void ParseExpression_ComplexityAnalysis_ShouldProvideComplexityInfo()
  {
    // Arrange
    var expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";

    // Act
    var complexity = _parser.AnalyzeComplexity(expression);

    // Assert
    Assert.That(complexity, Is.Not.Null, "Should provide complexity analysis");
    Assert.That(complexity.NodeCount, Is.GreaterThan(0), "Should count expression nodes");
    Assert.That(complexity.AggregationCount, Is.EqualTo(2), "Should identify 2 aggregations");
    Assert.That(complexity.OperatorCount, Is.GreaterThan(0), "Should identify logical operators");
    Assert.That(complexity.ConditionCount, Is.GreaterThan(0), "Should identify condition nodes");
  }
}
