namespace Reactive.Expressions.Models;

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
