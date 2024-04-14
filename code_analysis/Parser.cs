namespace CodeAnalysis;

public class Parser<TLexemeType, TState>
    where TLexemeType : struct, Enum
    where TState : IState<TLexemeType>, new()
{
    private readonly Lexer<TLexemeType> _lexer;

    private readonly ErrorsAnalyzer<TLexemeType, TState> _errorsAnalyzer;

    private readonly Fixer<TLexemeType> _fixer;

    public Parser(ILexemeHelper<TLexemeType> lexemeHelper, LexemeEater<TLexemeType>[] lexemeEaters)
    {
        _lexer = new Lexer<TLexemeType>(lexemeHelper, lexemeEaters);
        _errorsAnalyzer = new ErrorsAnalyzer<TLexemeType, TState>(lexemeHelper);
        _fixer = new Fixer<TLexemeType>(lexemeHelper);
    }

    public string Fix(string content)
    {
        return _fixer.Fix(content, _errorsAnalyzer.Analyze(_lexer.Scan(content)));
    }

    public IEnumerable<Error<TLexemeType>> Analyze(string content)
    {
        return _errorsAnalyzer.Analyze(_lexer.Scan(content));
    }

    private IEnumerable<Lexeme<TLexemeType>> Scan(string content)
    {
        return _lexer.Scan(content);
    }
}