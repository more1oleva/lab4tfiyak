using CodeAnalysis;

namespace Example.Struct;

public class LexemeHelper : ILexemeHelper<LexemeType>
{
    public LexemeType UnexpectedSymbol()
    {
        return LexemeType.UnexpectedSymbol;
    }

    public bool IsIgnorableLexeme(LexemeType lexeme)
    {
        return lexeme is LexemeType.Separator;
    }

    public bool IsInvalidLexeme(LexemeType lexeme)
    {
        return lexeme is LexemeType.UnexpectedSymbol;
    }

    public string LexemeMissingValue(LexemeType lexeme)
    {
        return lexeme switch
        {
            LexemeType.AccessModifier => "public",
            LexemeType.StructKeyword => "struct",
            LexemeType.Identifier => "identifier",
            LexemeType.OpenBracket => "{",
            LexemeType.DataType => "int",
            LexemeType.EndOperator => ";",
            LexemeType.CloseBracket => "}",
            // LexemeType.UnexpectedSymbol => "",
            // LexemeType.Separator => " ",
            _ => throw new ArgumentException("LexemeType.UnexpectedSymbol and LexemeType.Separator are invalid arguments"),
        };
    }
}
