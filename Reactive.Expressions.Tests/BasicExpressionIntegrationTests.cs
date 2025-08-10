using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using Reactive.Expressions;
using Reactive.Expressions.Models;
using Reactive.Expressions.Parser;

namespace Reactive.Expressions.Tests;

/// <summary>
/// Basic integration tests for metric expression evaluation.
/// These tests focus on the core functionality without complex reactive scenarios.
/// </summary>
[TestFixture]
public class BasicExpressionIntegrationTests
{
  private ILogger<AntlrExpressionParser> _parserLogger;
  private AntlrExpressionParser _parser;

  [SetUp]
  public void Setup()
  {
    using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
    _parserLogger = loggerFactory.CreateLogger<AntlrExpressionParser>();
    _parser = new AntlrExpressionParser(_parserLogger);
  }

  [Test]
  public void IntegrationTest_CpuOrMemoryExpression_ShouldParseAndBuildSuccessfully()
  {
    // Arrange
    var expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";
    var knownMetrics = new HashSet<string> { "cpu", "mem" };

    // Act - Parse and validate the expression
    var validation = _parser.ValidateExpression(expression, knownMetrics: knownMetrics);

    // Assert parsing
    Assert.That(validation.IsValid, Is.True,
        $"Expression should be valid. Errors: {string.Join(", ", validation.Errors)}");

    // Act - Parse to AST
    var ast = _parser.ParseExpression(expression);

    // Assert AST creation
    Assert.That(ast, Is.Not.Null, "Should create AST successfully");
    Assert.That(ast.Name, Is.Not.Null.And.Not.Empty, "AST should have a name");

    // Verify this is the expression we expected (the name should start with "Or" for OR operations)
    Assert.That(ast.Name, Does.StartWith("Or"), "Should contain OR operation in the name");
  }

  [Test]
  public void IntegrationTest_ComplexityAnalysis_CpuOrMemoryExpression_ShouldProvideCorrectMetrics()
  {
    // Arrange
    var expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";

    // Act
    var complexity = _parser.AnalyzeComplexity(expression);

    // Assert
    Assert.That(complexity, Is.Not.Null, "Should provide complexity analysis");
    Assert.That(complexity.NodeCount, Is.GreaterThanOrEqualTo(3), "Should have multiple nodes (at least 3)");
    Assert.That(complexity.AggregationCount, Is.EqualTo(2), "Should identify exactly 2 aggregations (avg cpu and avg mem)");
    Assert.That(complexity.ConditionCount, Is.GreaterThan(0), "Should have condition nodes");
    Assert.That(complexity.OperatorCount, Is.GreaterThan(0), "Should have logical operators");
    Assert.That(complexity.IsHighComplexity, Is.False, "This expression should not be considered high complexity");
  }

  [Test]
  public void IntegrationTest_VariousValidExpressions_ShouldAllParseCorrectly()
  {
    // Arrange
    var testCases = new[]
    {
            new { Expression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80", ExpectedAggregations = 2, Description = "OR with two aggregations" },
            new { Expression = "avg(cpu, 5m) > 50 && avg(mem, 10m) < 90", ExpectedAggregations = 2, Description = "AND with two aggregations" },
            new { Expression = "max(cpu, 30s) > 95", ExpectedAggregations = 1, Description = "Single MAX aggregation" },
            new { Expression = "min(mem, 2h) < 10", ExpectedAggregations = 1, Description = "Single MIN aggregation" },
            new { Expression = "sum(requests, 1m) > 1000", ExpectedAggregations = 1, Description = "Single SUM aggregation" }
        };

    var knownMetrics = new HashSet<string> { "cpu", "mem", "memory", "requests" };

    // Act & Assert
    foreach (var testCase in testCases)
    {
      // Validate
      var validation = _parser.ValidateExpression(testCase.Expression, knownMetrics: knownMetrics);
      Assert.That(validation.IsValid, Is.True,
          $"{testCase.Description}: Expression should be valid. Errors: {string.Join(", ", validation.Errors)}");

      // Parse
      var ast = _parser.ParseExpression(testCase.Expression);
      Assert.That(ast, Is.Not.Null, $"{testCase.Description}: Should create AST");

      // Analyze complexity
      var complexity = _parser.AnalyzeComplexity(testCase.Expression);
      Assert.That(complexity.AggregationCount, Is.EqualTo(testCase.ExpectedAggregations),
          $"{testCase.Description}: Should identify {testCase.ExpectedAggregations} aggregation(s)");
    }
  }

  [Test]
  public void IntegrationTest_EdgeCases_ShouldHandleCorrectly()
  {
    // Arrange
    var testCases = new[]
    {
            new { Expression = "(avg(cpu, 1m) > 70 || avg(mem, 1m) > 80) && max(disk, 5m) < 95", Description = "Parentheses and mixed operators" },
            new { Expression = "avg(cpu_usage, 1m) > 70.5", Description = "Underscore in metric name and decimal threshold" },
            new { Expression = "avg(CPU, 1M) > 70", Description = "Uppercase metric and time unit" },
        };

    var knownMetrics = new HashSet<string> { "cpu", "mem", "disk", "cpu_usage", "CPU" };

    // Act & Assert
    foreach (var testCase in testCases)
    {
      var validation = _parser.ValidateExpression(testCase.Expression, knownMetrics: knownMetrics);
      Assert.That(validation.IsValid, Is.True,
          $"{testCase.Description}: Should handle edge case correctly. Errors: {string.Join(", ", validation.Errors)}");

      var ast = _parser.ParseExpression(testCase.Expression);
      Assert.That(ast, Is.Not.Null, $"{testCase.Description}: Should create AST for edge case");
    }
  }

  [Test]
  public void IntegrationTest_MainExpression_FullWorkflow_Success()
  {
    // This test demonstrates the complete workflow for the requested expression
    // Arrange
    var targetExpression = "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80";
    var knownMetrics = new HashSet<string> { "cpu", "mem", "memory" };

    // Act - Complete workflow

    // Step 1: Validate expression
    var validation = _parser.ValidateExpression(targetExpression, knownMetrics: knownMetrics);

    // Step 2: Parse to AST
    var ast = _parser.ParseExpression(targetExpression);

    // Step 3: Analyze complexity
    var complexity = _parser.AnalyzeComplexity(targetExpression);

    // Assert - All steps should succeed
    Assert.That(validation.IsValid, Is.True,
        "Expression 'avg(cpu, 1m) > 70 || avg(mem, 1m) > 80' should be valid");
    Assert.That(validation.Errors, Is.Empty, "Should have no validation errors");

    Assert.That(ast, Is.Not.Null, "Should successfully create AST");
    Assert.That(ast.Name, Is.Not.Null.And.Not.Empty, "AST should have a meaningful name");

    Assert.That(complexity, Is.Not.Null, "Should provide complexity analysis");
    Assert.That(complexity.AggregationCount, Is.EqualTo(2), "Should identify 2 aggregation operations");
    Assert.That(complexity.NodeCount, Is.GreaterThanOrEqualTo(3), "Should have multiple nodes in the expression tree");

    // This confirms that the expression "avg(cpu, 1m) > 70 || avg(mem, 1m) > 80"
    // is syntactically correct and can be processed by the expression engine
    Console.WriteLine($"✅ Successfully processed expression: {targetExpression}");
    Console.WriteLine($"   - Validation: {(validation.IsValid ? "PASSED" : "FAILED")}");
    Console.WriteLine($"   - AST Name: {ast.Name}");
    Console.WriteLine($"   - Aggregations: {complexity.AggregationCount}");
    Console.WriteLine($"   - Total Nodes: {complexity.NodeCount}");
  }
}
