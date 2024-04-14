using CodeAnalysis;

namespace Example.Struct;

public struct State : IState<LexemeType>
{
    public LexemeType? CurrentLexemeType { get; private set; }

    private bool IsInsideOfStruct { get; set; }

    public bool IsEnd()
    {
        return this is { CurrentLexemeType: LexemeType.EndOperator, IsInsideOfStruct: false };
    }

    public StatesCollection<LexemeType> NextStates()
    {
        return this switch
        {
            { CurrentLexemeType: null } =>
                new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = false }
                    .IntoStatesCollection(),

            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.StructKeyword, IsInsideOfStruct = false }
                    .IntoStatesCollection(),

            { CurrentLexemeType: LexemeType.StructKeyword, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.Identifier, IsInsideOfStruct = false }
                    .IntoStatesCollection(),

            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.OpenBracket, IsInsideOfStruct = false }
                    .IntoStatesCollection(),

            { CurrentLexemeType: LexemeType.OpenBracket, IsInsideOfStruct: false } =>
                new IState<LexemeType>[]
                    {
                        new State { CurrentLexemeType = LexemeType.CloseBracket, IsInsideOfStruct = false },
                        new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = true },
                    }
                    .IntoStatesCollection(0),

            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: true } =>
                new State { CurrentLexemeType = LexemeType.DataType, IsInsideOfStruct = true }
                    .IntoStatesCollection(),
            
            { CurrentLexemeType: LexemeType.DataType, IsInsideOfStruct: true } =>
                new State { CurrentLexemeType = LexemeType.Identifier, IsInsideOfStruct = true }
                    .IntoStatesCollection(),
            
            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: true } =>
                new State { CurrentLexemeType = LexemeType.EndOperator, IsInsideOfStruct = true }
                    .IntoStatesCollection(),
            
            { CurrentLexemeType: LexemeType.EndOperator, IsInsideOfStruct: true } =>
                new IState<LexemeType>[]
                {
                    new State { CurrentLexemeType = LexemeType.CloseBracket, IsInsideOfStruct = false },
                    new State { CurrentLexemeType = LexemeType.AccessModifier, IsInsideOfStruct = true }
                }
                    .IntoStatesCollection(0),
            
            { CurrentLexemeType: LexemeType.CloseBracket, IsInsideOfStruct: false } =>
                new State { CurrentLexemeType = LexemeType.EndOperator, IsInsideOfStruct = false }
                    .IntoStatesCollection(),
            _ => new StatesCollection<LexemeType>([], -1),
        };
    }

    public bool IsBoundaryLexeme(LexemeType lexemeType)
    {
        return this switch
        {
            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: false } => lexemeType ==
                LexemeType.OpenBracket,
            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: false } => lexemeType ==
                LexemeType.OpenBracket,
            { CurrentLexemeType: LexemeType.EndOperator, IsInsideOfStruct: true } => lexemeType ==
                LexemeType.CloseBracket,
            { CurrentLexemeType: LexemeType.Identifier, IsInsideOfStruct: true } => lexemeType ==
                LexemeType.EndOperator,
            { CurrentLexemeType: LexemeType.AccessModifier, IsInsideOfStruct: true } => lexemeType ==
                LexemeType.EndOperator,
            { CurrentLexemeType: LexemeType.DataType, IsInsideOfStruct: true } => lexemeType ==
                LexemeType.EndOperator,
            _ => false,
        };
    }
}