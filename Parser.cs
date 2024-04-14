using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace лб1тфияк
{
    public class Parser
    {
        public List<ParsingError> _parsingErrors;
        private int contentLength = 0;

        public Parser()
        {
            _parsingErrors = new List<ParsingError>();
        }

        public void OutputErr(Token? token, List<Lexem> tail,  Lexem lexem, List<Token> expectedTokens, Lexem foundLexem = null)
        {
            OutputErr(token, tail, lexem,  lexem == null ? -1 : lexem.StartPos,  lexem == null ? -1 : lexem.StartPos + lexem.Value.Length, expectedTokens, foundLexem);
        }
        
        public void OutputErr(Token? token, List<Lexem> tail,  Lexem lexem, int startPos, int endPos, List<Token> expectedTokens, Lexem foundLexem = null)
        {
            string printableTail = string.Join(", ", tail.Select(lexem => lexem.Value));

            if (token == Token.INVALID)
            {
                _parsingErrors.Add(new ParsingError
                {
                    NumberOfError = _parsingErrors.Count + 1,
                    NeedToken = token.ToString(),
                    ErrorToken = lexem.Value,
                    StartIndex = lexem.StartPos,
                    EndIndex = lexem.StartPos + lexem.Value.Length,
                    Message = "INVALID lexem",
                    //TODO
                });
                Console.WriteLine("3Unexpected symbol, tail: " + printableTail);
            }
            else
            {
                _parsingErrors.Add(new ParsingError
                {
                    NumberOfError = _parsingErrors.Count + 1,
                    NeedToken = token.ToString(),
                    ErrorToken = lexem == null ? "" : lexem.Value,
                    StartIndex =  startPos,
                    EndIndex = endPos,
                    Message = "INVALID lexem",
                    //TODO
                });
                Console.WriteLine("4Expected " + token  + (foundLexem != null ? " found " + foundLexem.Value : "") + ", tail: " + printableTail);
            }
        }

        public ParsingError OutputExpectedTokensErr(List<Token> expectedTokens, List<Lexem> tail, Lexem foundToken)
        {
            string printableTail = string.Join(", ", tail.Select(lexem => lexem.Value));
            string printableExpectedTokens = string.Join(" or ", expectedTokens);
            string firstOr = expectedTokens.Count > 1 ? "or " : "";

            Console.WriteLine("3Expected " + firstOr + printableExpectedTokens + ", tail: " + printableTail);
            
            return new ParsingError
            {
                NumberOfError = _parsingErrors.Count + 1,
                ErrorToken = foundToken == null ? "EOF" :  foundToken.Value,
                NeedToken = expectedTokens.Count > 1 ? "Expected: " + string.Join(", ", expectedTokens.Select(token => token.ToString())) : "Expected: " + expectedTokens[0],
                StartIndex = foundToken == null ? contentLength : foundToken.StartPos,
                EndIndex = foundToken == null ? contentLength : foundToken.StartPos + foundToken.Value.Length,
                Message = "INVALID lexem",
                //TODO
            };
        }
        
        public static List<Lexem> GetTail(List<Lexem> tokens, int start)
        {
            if (start >= tokens.Count) return new List<Lexem>();
            return tokens.GetRange(start, tokens.Count - start);
        }

        public void Parse(List<Lexem> tokens, State state, bool key, ParsingError? parsingError = null)
        {
            var saveIndex = 0;
            var tokensCount = 0;
            
            if (tokens.Count != 0)
            {
                contentLength = tokens.Last().StartPos + tokens.Last().Value.Length;
            }
            
            if (tokens.Count == 0 && state.IsEnd())
            {
                if (parsingError != null) _parsingErrors.Add(parsingError);
        
                return;
            }

            if (state.IsEnd())
            {
                if (parsingError != null) _parsingErrors.Add(parsingError);
                
                OutputErr(Token.END_OPERATOR, tokens,  new Lexem(0, "EOF", 0, 0, Token.INVALID), state.ExpectedTokens());

                state = new State();
                Parse(tokens.GetRange(saveIndex, tokensCount), state, false);
                
                return;
            }

            if (tokens.Count == 0)
            {
                var errorSyka = OutputExpectedTokensErr(state.ExpectedTokens(), tokens, null);
                
                if (parsingError != null) _parsingErrors.Add(parsingError);

                _parsingErrors.Add(errorSyka);
                
                state.Move();
                Parse(tokens, state, false);
                return;
            }

            if (tokens[0].Token == Token.INVALID)
            {
                if (parsingError != null) _parsingErrors.Add(parsingError);
                OutputErr(tokens[0].Token, GetTail(tokens, 1), tokens[0], state.ExpectedTokens());
                Parse(GetTail(tokens, 1), state, false);
                return;
            }
            
            var buff1 = new Lexem(0, "", 0, 0, Token.INVALID);
            
            for (int index = 0; index < tokens.Count; index++)
            {
                if (state.BoundaryToken(tokens[index].Token)) break;
                
                if (!state.TryMove(tokens[index].Token))
                {
                    buff1 = tokens[index];
                    continue;
                }
                
                if (parsingError != null)
                {
                    if (parsingError.EndIndex < tokens[index].StartPos)
                    {
                        parsingError.EndIndex = tokens[index].StartPos;
                    }
                    _parsingErrors.Add(parsingError);
                }

                List<Lexem> tail = GetTail(tokens, index + 1);

                if (index > 0 && !key)
                {
                    OutputErr(state.TokenState, tail,buff1, tokens[0].StartPos, tokens[index].StartPos, state.ExpectedTokens());
                }

                Parse(tail, state, false);
                saveIndex = index;
                tokensCount++;
                return;
            }

            var error = OutputExpectedTokensErr(state.ExpectedTokens(), tokens, tokens[0]);
            state.Move();
            if (parsingError != null) _parsingErrors.Add(parsingError);
            Parse(tokens, state, true, error);
        }


        public class State
        {
            public Token? TokenState { get; private set; }
            private bool IsInsideOfStruct { get; set; }
            public bool IsStructOrDataTypeError { get; set; } = false;

            public State(Token? tokenState = null, bool isInsideOfStruct = false)
            {
                TokenState = tokenState;
                IsInsideOfStruct = isInsideOfStruct;
            }

            public bool IsEnd()
            {
                return TokenState == Token.END_OPERATOR && !IsInsideOfStruct;
            }

            public List<Token> ExpectedTokens()
            {
                var nextStates = NextStates(this);

                return nextStates.Select(tokenState => tokenState.TokenState).Where(tokenState => tokenState != null).Cast<Token>() .ToList();
            }

            private static State[] NextStates(State state)
            {
                return state switch
                {
                    { TokenState: null, IsInsideOfStruct: false } => new []
                    {
                        new State { IsInsideOfStruct = false, TokenState = Token.ACCESS_MODIFIER}
                    },
                    { TokenState: Token.ACCESS_MODIFIER, IsInsideOfStruct: false } => new [] { new State
                    {
                        IsInsideOfStruct = false, TokenState = Token.STRUCT_KEYWORD
                    } },
                    { TokenState: Token.STRUCT_KEYWORD, IsInsideOfStruct: false } => new []
                    {
                        new State { IsInsideOfStruct = false, TokenState = Token.IDENTIFIER}
                    },
                    { TokenState: Token.IDENTIFIER, IsInsideOfStruct: false } => new []
                    {
                        new State { IsInsideOfStruct = false, TokenState = Token.OPEN_BRACKET}
                    },
                    { TokenState: Token.OPEN_BRACKET, IsInsideOfStruct: false } => new []
                    {
                        new State { IsInsideOfStruct = false, TokenState = Token.CLOSE_BRACKET},
                        new State { IsInsideOfStruct = true, TokenState = Token.ACCESS_MODIFIER}
                    },
                    { TokenState: Token.ACCESS_MODIFIER, IsInsideOfStruct: true } => new []
                    {
                        new State { IsInsideOfStruct = true, TokenState = Token.DATA_TYPE}
                    },
                    { TokenState:Token.DATA_TYPE, IsInsideOfStruct: true } => new []
                    {
                        new State { IsInsideOfStruct = true, TokenState = Token.IDENTIFIER}
                    },
                    { TokenState: Token.IDENTIFIER, IsInsideOfStruct: true } => new []
                    {
                        new State { IsInsideOfStruct = true, TokenState = Token.END_OPERATOR}
                    },
                    { TokenState:Token.END_OPERATOR, IsInsideOfStruct: true } => new []
                    {
                        new State { IsInsideOfStruct = false, TokenState = Token.CLOSE_BRACKET},
                        new State { IsInsideOfStruct = true, TokenState = Token.ACCESS_MODIFIER}
                    },
                    { TokenState:Token.CLOSE_BRACKET, IsInsideOfStruct: false } => new []
                    {
                        new State { IsInsideOfStruct = false, TokenState = Token.END_OPERATOR}
                    },
                    { TokenState:Token.END_OPERATOR, IsInsideOfStruct: false } => new []
                    {
                        new State { IsInsideOfStruct = false, TokenState = null},
                        new State { IsInsideOfStruct = false, TokenState = Token.ACCESS_MODIFIER}
                    },
                    _ => new State[]{}
                };
            }
            
            public bool BoundaryToken(Token token)
            {
                var nextStates = NextStates(this).SelectMany(NextStates);
                
                return nextStates.Any(nextState => nextState.TokenState == token);
            }

            public bool TryMove(Token token)
            {
                var nextStates = NextStates(this);

                foreach (var nextState in nextStates)
                {
                    if (nextState.TokenState != token) continue;
                    
                    TokenState = nextState.TokenState;
                    IsInsideOfStruct = nextState.IsInsideOfStruct;
                    IsStructOrDataTypeError = false;
                    return true;
                }
                
                return false;
            }

            public void Move()
            {
                var newState = NextStates(this);
                
                if (newState.Length == 0) return;
                
                TokenState = newState[0].TokenState;
                IsInsideOfStruct = newState[0].IsInsideOfStruct;
            }
        }
    }
}
    
    