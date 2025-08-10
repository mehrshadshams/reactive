using Antlr4.Runtime;

namespace Reactive.Expressions.Parser;

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
