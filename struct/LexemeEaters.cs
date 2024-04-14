using System;
using CodeAnalysis;

namespace Example.Struct;

public static class LexemeEaters
{
    public static LexemeEater<LexemeType>[] Eaters =
    [
        TryEatAccessModifier,
        TryEatStructKeyword,
        TryEatDataType,
        TryEatIdentifier,
        TryEatOpenBracket,
        TryEatCloseBracket,
        TryEatEndOperator,
        TryEatSeparator,
    ];

    private static LexemeType? TryEatIdentifier(Eater eater)
    {
        if (!eater.Eat(IsIdentifierHead)) return null;
        eater.EatWhile(IsIdentifierTail);

        return LexemeType.Identifier;
    }

    private static bool IsIdentifierHead(char sym)
    {
        return char.IsLetter(sym) || sym == '_';
    }

    private static bool IsIdentifierTail(char sym)
    {
        return char.IsLetterOrDigit(sym) || sym == '_';
    }

    private static LexemeType? TryEatDataType(Eater eater)
    {
        return eater.Eat("int") || eater.Eat("char") || eater.Eat("string") || eater.Eat("bool")
            ? LexemeType.DataType
            : null;
    }

    private static LexemeType? TryEatAccessModifier(Eater eater)
    {
        return eater.Eat("private") || eater.Eat("public")
            ? LexemeType.AccessModifier
            : null;
    }

    private static LexemeType? TryEatStructKeyword(Eater eater)
    {
        return eater.Eat("struct")
            ? LexemeType.StructKeyword
            : null;
    }

    private static LexemeType? TryEatOpenBracket(Eater eater)
    {
        return eater.Eat('{') ? LexemeType.OpenBracket : null;
    }

    private static LexemeType? TryEatCloseBracket(Eater eater)
    {
        return eater.Eat('}') ? LexemeType.CloseBracket : null;
    }

    private static LexemeType? TryEatEndOperator(Eater eater)
    {
        return eater.Eat(';') ? LexemeType.EndOperator : null;
    }

    private static LexemeType? TryEatSeparator(Eater eater)
    {
        return eater.EatWhile(IsSeparator) ? LexemeType.Separator : null;
    }

    private static bool IsSeparator(char sym, char? nextSym)
    {
        return char.IsSeparator(sym)
               || sym == '\n'  
               || (nextSym is { } n && (sym == '\r' && n == '\n'));
    }
}