using CodeAnalysis;

namespace CodeAnalysis;

public readonly struct StatesCollection<TLexemeType>
    where TLexemeType : struct, Enum
{
    private readonly IReadOnlyList<IState<TLexemeType>> _states;
    
    public IEnumerable<IState<TLexemeType>> States => _states;

    private readonly int _defaultStateIndex;

    public IState<TLexemeType>? DefaultState => _defaultStateIndex < 0 ? null : _states[_defaultStateIndex];

    public StatesCollection(IState<TLexemeType> state) : this(new [] { state }, 0)
    {
    }
    
    public StatesCollection(IReadOnlyList<IState<TLexemeType>> states, int defaultStateIndex)
    {
        _states = states;
        _defaultStateIndex = defaultStateIndex;
    }

    public StatesCollection<TLexemeType> FilterByBoundaryLexeme(TLexemeType lexemeType)
    {
        return new StatesCollection<TLexemeType>
        (
            States.Where(s => !s.IsBoundaryLexeme(lexemeType)).ToArray(),
            _defaultStateIndex
        );
    }

    public IState<TLexemeType>? SelectFirstByLexeme(TLexemeType lexemeType)
    {
        return States.FirstOrDefault(state => state.CurrentLexemeType.Equals(lexemeType));
    }
}

