namespace CodeAnalysis;

public delegate TLexemeType? LexemeEater<TLexemeType>(Eater eater) where TLexemeType : struct, Enum;

public class Lexer<TLexemeType> where TLexemeType : struct, Enum
{
    private readonly ILexemeHelper<TLexemeType> _lexemeHelper;
    private readonly LexemeEater<TLexemeType>[] _lexemeEaters;
    
    private Caret _caret;
    
    private readonly List<Lexeme<TLexemeType>> _lexemes;

    public Lexer(ILexemeHelper<TLexemeType> lexemeHelper, LexemeEater<TLexemeType>[] lexemeEaters)
    {
        _lexemeHelper = lexemeHelper;
        _lexemeEaters = lexemeEaters;
        _caret = new Caret("");
        _lexemes = new List<Lexeme<TLexemeType>>();
    }

    public IEnumerable<Lexeme<TLexemeType>> Scan(string content)
    {
        Init(content);
        
        while (!_caret.IsEnd())
        {
            DoIteration();
        }

        return _lexemes;
    }

    private void Init(string content)
    {
        _caret = new Caret(content);
        _lexemes.Clear();
    }

    private void DoIteration()
    {
        foreach (var lexemeEaterFunc in _lexemeEaters)
        {
            var eater = _caret.StartEating();
            if (lexemeEaterFunc(eater) is not { } lexemeType) continue;

            var span = _caret.FinishEating(eater).NewSpan;
            var lexeme = lexemeType.IntoLexeme(span);
            _lexemes.Add(lexeme);

            return;
        }

        var invalidLexeme = _lexemeHelper.UnexpectedSymbol().IntoLexeme(_caret.Span().ShiftEnd(1));
        _lexemes.Add(invalidLexeme);
        _caret.Move();
    }
}