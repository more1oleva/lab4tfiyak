using System.Collections;
using System.Diagnostics;
using System.Reflection.Metadata;
using CodeAnalysis;

namespace CodeAnalysis;

public interface IState<TLexemeType> where TLexemeType : struct, Enum
{
    TLexemeType? CurrentLexemeType { get; }

    bool IsEnd();

    bool IsBoundaryLexeme(TLexemeType lexemeType);

    StatesCollection<TLexemeType> NextStates();
}

public static class StateExtensions
{
    public static StatesCollection<TLexemeType> IntoStatesCollection<TLexemeType>(this IState<TLexemeType> state)
        where TLexemeType : struct, Enum
    {
        return new StatesCollection<TLexemeType>(state);
    }

    public static StatesCollection<TLexemeType> IntoStatesCollection<TLexemeType>(this IState<TLexemeType>[] states,
        int defaultStateIndex)
        where TLexemeType : struct, Enum
    {
        return new StatesCollection<TLexemeType>(states, defaultStateIndex);
    }
}