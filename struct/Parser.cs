using CodeAnalysis;

namespace Example.Struct;

public class Parser() : Parser<LexemeType, State>(new LexemeHelper(), LexemeEaters.Eaters);