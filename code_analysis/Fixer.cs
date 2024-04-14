namespace CodeAnalysis;

public class Fixer<TLexemeType> where TLexemeType: struct, Enum
{
    private readonly ILexemeHelper<TLexemeType> _lexemeHelper;

    public Fixer(ILexemeHelper<TLexemeType> lexemeHelper)
    {
        _lexemeHelper = lexemeHelper;
    }
    
    public string Fix(string content, IEnumerable<Error<TLexemeType>> errors)
    {
        var shift = 0;

        foreach (var error in errors)
        {
            switch (error)
            {
                case Error<TLexemeType>.LexemeExhaustedError e:
                {
                    var missingValue = error.Lexeme == null ? "" : _lexemeHelper.LexemeMissingValue(error.Lexeme.Value) + " ";
                    
                    content = content.Insert(e.Span.Start + shift, missingValue);
                    shift += missingValue.Length;

                    continue;
                }
                case Error<TLexemeType>.InvalidLexemeError e:
                {
                    content = content.Remove(e.Span.Start + shift, e.Span.Count);
                    shift -= e.Span.Count;
                    
                    continue;
                }
                case Error<TLexemeType>.LexemeNotFoundError e:
                {
                    var missingValue = error.Lexeme == null ? "" : _lexemeHelper.LexemeMissingValue(error.Lexeme.Value) + " ";

                    content = content.Insert(e.Span.Start + shift, missingValue);
                    shift += missingValue.Length;
                    
                    continue;
                }
                case Error<TLexemeType>.LexemeNotFoundImmediatelyError e:
                {
                    content = content.Remove(e.Span.Start + shift, e.Span.Count);
                    shift -= e.Span.Count;
                    
                    continue;
                }
            }
        }
        
        return content;
    }
}