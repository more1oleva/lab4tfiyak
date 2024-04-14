namespace CodeAnalysis;

public enum ErrorKind
{
    LexemeExpected,
    InvalidLexeme,
}

public abstract class Error<TLexemeType> where TLexemeType : struct, Enum
{
    public Span Span { get; private set; }

    public int TailStart { get; private init; }

    public TLexemeType? Lexeme { get; private init; }

    private Error()
    {
    }

    internal Error<TLexemeType> SetEnd(int end)
    {
        Span = new Span(Span.Start, end);
        return this;
    }

    internal sealed class LexemeExhaustedError : Error<TLexemeType>
    {
        public LexemeExhaustedError(TLexemeType? nextLexemeType, int contentLength)
        {
            Span = new Span(contentLength, contentLength);
            Lexeme = nextLexemeType;
            TailStart = contentLength;
            // ErrorKind = ErrorKind.LexemeExpected;
        }
    }

    internal sealed class InvalidLexemeError : Error<TLexemeType>
    {
        public InvalidLexemeError(Lexeme<TLexemeType> invalidLexeme)
        {
            Span = invalidLexeme.Span;
            Lexeme = invalidLexeme.Type;
            TailStart = Span.End;
            // ErrorKind = ErrorKind.InvalidLexeme;
        }
    }

    internal sealed class LexemeNotFoundError : Error<TLexemeType>
    {
        public LexemeNotFoundError(Lexeme<TLexemeType> currentLexeme, TLexemeType? expectedLexemeType)
        {
            Span = currentLexeme.Span;
            // Span = new Span(currentLexeme.Span.Start, currentLexeme.Span.Start);
            Lexeme = expectedLexemeType;
            TailStart = Span.Start;
            // ErrorKind = ErrorKind.LexemeExpected;
        }
    }

    internal sealed class LexemeNotFoundImmediatelyError : Error<TLexemeType>
    {
        public LexemeNotFoundImmediatelyError(Lexeme<TLexemeType> currentLexeme, Lexeme<TLexemeType> foundLexeme)
        {
            Span = new Span(currentLexeme.Span.Start, foundLexeme.Span.Start);
            Lexeme = foundLexeme.Type;
            TailStart = foundLexeme.Span.End;
            // ErrorKind = ErrorKind.LexemeExpected;
        }
    }
}