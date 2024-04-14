namespace CodeAnalysis;

public class ErrorsAnalyzer<TLexemeType, TState>
    where TLexemeType : struct, Enum
    where TState : IState<TLexemeType>, new()
{
    private IState<TLexemeType> _state = new TState();

    private readonly ILexemeHelper<TLexemeType> _lexemeHelper;

    private readonly List<Lexeme<TLexemeType>> _lexemes;

    private int _contentLength;

    private int _tailStart;

    private readonly List<Error<TLexemeType>> _errors;

    private readonly List<Error<TLexemeType>> _tempErrors;

    public ErrorsAnalyzer(ILexemeHelper<TLexemeType> lexemeHelper)
    {
        _lexemeHelper = lexemeHelper;
        
        _lexemes = new List<Lexeme<TLexemeType>>();
        
        _tailStart = 0;
        _contentLength = 0;
        
        _errors = new List<Error<TLexemeType>>();
        _tempErrors = new List<Error<TLexemeType>>();
    }


    public IEnumerable<Error<TLexemeType>> Analyze(IEnumerable<Lexeme<TLexemeType>> lexemes)
    {
        Init(lexemes.ToArray());
        
        while (true)
        {
            if (DoIteration() is ControlFlow.Stop) break;
        }

        return _errors;
    }
    
    
    private void Init(Lexeme<TLexemeType>[] lexemes)
    {
        _lexemes.Clear();
        _lexemes.AddRange(lexemes.Where(lexeme => !_lexemeHelper.IsIgnorableLexeme(lexeme.Type)));
        
        _tailStart = 0;
        _contentLength = _lexemes.Count > 0 ? lexemes.Last().Span.End : 0;

        _errors.Clear();
        _tempErrors.Clear();
    }

    private enum ControlFlow
    {
        Continue,
        Stop,
    }

    private void PushTempErrors()
    {
        _errors.AddRange(_tempErrors);
        _tempErrors.Clear();
    }

    private ControlFlow DoIteration()
    {
        if (IsEnd())
        {
            PushTempErrors();
            return ControlFlow.Stop;
        }

        if (AreUnprocessedLexemesRemained())
        {
            return HandleRemainedUnprocessedLexemes();
        }

        if (AreLexemesExhausted())
        {
            return HandleLexemesExhaustion();
        }

        if (IsCurrentLexemeInvalid())
        {
            return HandleInvalidLexeme();
        }

        var nextStates = _state.NextStates();

        for (var i = _tailStart; i < _lexemes.Count; i++)
        {
            var lexeme = _lexemes[i];

            if (_lexemeHelper.IsIgnorableLexeme(lexeme.Type))
            {
                continue;
            }

            nextStates = nextStates.FilterByBoundaryLexeme(lexeme.Type);
            var nextState = nextStates.SelectFirstByLexeme(lexeme.Type);
            
            if (nextState == null)
            {
                continue;
            }

            _state = nextState;
            return HandleFoundLexeme(i);
        }

        return HandleLexemeNotFound();
    }

    private void ShiftEndOfTempErrors(Lexeme<TLexemeType> lexeme)
    {
        for (var j = 0; j < _tempErrors.Count; j++)
        {
            if (lexeme.Span.Start > _tempErrors[j].Span.End)
            {
                _tempErrors[j] = _tempErrors[j].SetEnd(lexeme.Span.Start);
            }
        }
    }

    private bool IsEnd()
    {
        return _tailStart == _lexemes.Count && _state.IsEnd();
    }

    private bool AreUnprocessedLexemesRemained()
    {
        return _tailStart < _lexemes.Count && _state.IsEnd();
    }
    
    private ControlFlow HandleRemainedUnprocessedLexemes()
    {
        _state = new TState();
        return ControlFlow.Continue;
    }

    private bool AreLexemesExhausted()
    {
        return (_lexemes.Count == 0 || _tailStart == _lexemes.Count) && !_state.IsEnd();
    }

    private ControlFlow HandleLexemesExhaustion()
    {
        var nextState = _state.NextStates().DefaultState;
        
        var error = new Error<TLexemeType>.LexemeExhaustedError(nextState?.CurrentLexemeType, _contentLength);
        _errors.Add(error);
        
        _state = nextState ?? _state;
        return ControlFlow.Continue;
    }

    private bool IsCurrentLexemeInvalid()
    {
        return _lexemeHelper.IsInvalidLexeme(CurrentLexeme().Type);
    }

    private ControlFlow HandleInvalidLexeme()
    {
        var error = new Error<TLexemeType>.InvalidLexemeError(CurrentLexeme());
        _errors.Add(error);
        
        _tailStart += 1;
        return ControlFlow.Continue;
    }

    private ControlFlow HandleLexemeNotFound()
    {
        var nextState = _state.NextStates().DefaultState;
        var error = new Error<TLexemeType>.LexemeNotFoundError(CurrentLexeme(), nextState?.CurrentLexemeType);
        _tempErrors.Add(error);

        // _tailStart += 1;
        _state = nextState ?? _state;
        return ControlFlow.Continue;
    }

    private ControlFlow HandleFoundLexeme(int foundLexemeIndex)
    {
        var foundLexeme = _lexemes[foundLexemeIndex];
        
        if (foundLexemeIndex > _tailStart && _tempErrors.Count == 0)
        {
            _errors.Add(new Error<TLexemeType>.LexemeNotFoundImmediatelyError(CurrentLexeme(), foundLexeme));
        }
        
        ShiftEndOfTempErrors(foundLexeme);
        PushTempErrors();

        _tailStart = foundLexemeIndex + 1;
        return ControlFlow.Continue;
    }

    private Lexeme<TLexemeType> CurrentLexeme()
    {
        return _lexemes[_tailStart];
    }
}