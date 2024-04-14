namespace CodeAnalysis;

public interface ILexemeHelper<TLexemeType>
{
    TLexemeType UnexpectedSymbol();

    bool IsIgnorableLexeme(TLexemeType lexeme);

    bool IsInvalidLexeme(TLexemeType lexeme);

    string LexemeMissingValue(TLexemeType lexeme);
}