using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Antlr4.Runtime;

namespace лб1тфияк
{
    enum State
    {
        Start,
        StructAccessModifier,
        Struct,
        StructIdentifier,
        OpenStruct,
        FiledAccessModifier,
        FieldDataType,
        FieldIdentifier,
        EndField,
        CloseStruct,
        End
    }

    public enum ParseError
    {
        AccessModifierExpected ,
        StructKeywordExpected,
        StructIdentifierExpected,
        OpenStructBracketExpected,
        FieldAccessModifierExpectedOrStructEndExpected,
        FieldDataTypeExpected,
        FieldIdentifierExpected,
        FieldEndExpected,
        CloseStructBracketExpectedOrFieldAccessModifierExpected,
        EndOfStructExpected,
        InvalidToken,
        NothingExpected
    }
    
    public class Parser
    {
        private List<ParsingError> _errors;
        private List<String> _accessModified = new List<String>()
        {
            "private",
            "public",
            "protected",
        };
        private State _currentState = State.Start;

        public List<ParsingError> Parse(List<Token> tokens, string code)
        {
            _errors = new List<ParsingError>();

            foreach (var token in tokens)
            {
                var error = ParseAccessModifier(token);
                if (error != null)
                {
                    var errorBuff = new ParsingError
                    {
                        ErrorToken = token.Value, 
                        Message = error.Value.ToString(),
                        NeedToken = error.Value.ToString() switch
                        {
                            "AccessModifierExpected" => "Access Modifier (public, private, protected)",
                            "StructKeywordExpected" => "'struct' Keyword",
                            "StructIdentifierExpected" => "Struct Identifier",
                            "OpenStructBracketExpected" => "'{'",
                            "FieldAccessModifierExpectedOrStructEndExpected" => "Field Access Modifier or '}'",
                            "FieldDataTypeExpected" => "Field Data Type",
                            "FieldIdentifierExpected" => "Field Identifier",
                            "FieldEndExpected" => "';'",
                            "CloseStructBracketExpectedOrFieldAccessModifierExpected" => "'}' or Field Access Modifier",
                            "EndOfStructExpected" => "';'",
                            "NothingExpected" => "End of struct",
                            "InvalidToken" => "Invalid Token",
                            _ => ""
                        },
                        StartIndex = token.StartPos - 1,
                        EndIndex = token.StartPos + token.Value.Length - 1,
                        NumberOfError = _errors.Count + 1,
                        ParseError = error.Value
                    };

                    switch (error.Value.ToString())
                    {
                        case "AccessModifierExpected":
                            _currentState = State.StructAccessModifier;
                            break;
                        case "StructKeywordExpected":
                            _currentState = State.Struct;
                            break;
                        case "StructIdentifierExpected":
                            _currentState = State.StructIdentifier;
                            break;
                        case "OpenStructBracketExpected":
                            _currentState = State.OpenStruct;
                            break;
                        case "FieldAccessModifierExpectedOrStructEndExpected":
                            _currentState = State.CloseStruct;
                            break;
                        case "FieldDataTypeExpected":
                            _currentState = State.FieldDataType;
                            break;
                        case "FieldIdentifierExpected":
                            _currentState = State.FieldIdentifier;
                            break;
                        case "FieldEndExpected":
                            _currentState = State.EndField;
                            break;
                        case "CloseStructBracketExpectedOrFieldAccessModifierExpected":
                            _currentState = State.CloseStruct;
                            break;
                        case "EndOfStructExpected":
                            _currentState = State.End;
                            break;
                        case "NothingExpected":
                            _currentState = State.End;
                            break;
                        case "InvalidToken":
                            break;
                    }
                    
                    _errors.Add(errorBuff);
                }
            }
            
            var errorLast = ParseAccessModifier(null);
            if (errorLast != null)
            {
                _errors.Add(new ParsingError
                {
                    ErrorToken = tokens.Last().Value, 
                    Message = errorLast.Value.ToString(),
                    NeedToken = ";", 
                    StartIndex = code.Length - 1,
                    EndIndex = code.Length - 1,
                    ParseError = errorLast.Value
                });
            }

            return _errors;
        }
        
        private ParseError? ParseAccessModifier(Token? token)
        {
            if (token?.Type == -1)
            {
                return ParseError.InvalidToken;
            }
            
            if (token != null && _currentState == State.Start && _accessModified.Contains(token.Value))
            {
                _currentState = State.StructAccessModifier;
                return null;
            }
            
            if (_currentState == State.Start)
            {
                return ParseError.AccessModifierExpected;
            }

            return ParseStruct(token);
        }
        
        private ParseError? ParseStruct(Token? token)
        {
            if (token != null && _currentState == State.StructAccessModifier && token.Value == "struct")
            {
                _currentState = State.Struct;
                return null;
            }

            if (_currentState == State.StructAccessModifier)
            {
                return ParseError.StructKeywordExpected;
            }
            
            return ParseStructIdentifier(token);
        }
        
        private ParseError? ParseStructIdentifier(Token? token)
        {
            if (token != null && _currentState == State.Struct && token.Type == 12)
            {
                _currentState = State.StructIdentifier;
                return null;
            }

            if (_currentState == State.Struct)
            {
                return ParseError.StructIdentifierExpected;
            }
            
            return ParseOpenStruct(token);
        }
        
        private ParseError? ParseOpenStruct(Token? token)
        {
            if (token != null && _currentState == State.StructIdentifier && token.Value == "{")
            {
                _currentState = State.OpenStruct;
                return null;
            }

            if (_currentState == State.StructIdentifier)
            {
                return ParseError.OpenStructBracketExpected;
            }
            
            return ParseFieldAccessModifierOrStructEnd(token);
        }
        
        private ParseError? ParseFieldAccessModifierOrStructEnd(Token? token)
        {
            if (token != null && _currentState == State.OpenStruct && _accessModified.Contains(token.Value))
            {
                _currentState = State.FiledAccessModifier;
                return null;
            }

            if (token != null && _currentState == State.OpenStruct && token.Value == "}")
            {
                _currentState = State.CloseStruct;
                return null;
            }
            
            if (_currentState == State.OpenStruct)
            {
                return ParseError.FieldAccessModifierExpectedOrStructEndExpected;
            }
            
            return ParseFieldDataType(token);
        }
        
        private ParseError? ParseFieldDataType(Token? token)
        {
            if (token != null && _currentState == State.FiledAccessModifier && (token.Type == 5 || token.Type == 6 || token.Type == 7 || token.Type == 8))
            {
                _currentState = State.FieldDataType;
                return null;
            }

            if (_currentState == State.FiledAccessModifier)
            {
                return ParseError.FieldDataTypeExpected;
            }
            
            return ParseFieldIdentifier(token);
        }
        
        private ParseError? ParseFieldIdentifier(Token? token)
        {
            if (token != null && _currentState == State.FieldDataType && token.Type == 12)
            {
                _currentState = State.FieldIdentifier;
                return null;
            }

            if (_currentState == State.FieldDataType)
            {
                return ParseError.FieldIdentifierExpected;
            }
            
            return ParseEndField(token);
        }
        
        private ParseError? ParseEndField(Token? token)
        {
            if (token != null && _currentState == State.FieldIdentifier && token.Value == ";")
            {
                _currentState = State.EndField;
                return null;
            }
            
            if (_currentState == State.FieldIdentifier)
            {
                return ParseError.FieldEndExpected;
            }
            
            return ParseCloseStructOrFieldAccessModifier(token);
        }

        private ParseError? ParseCloseStructOrFieldAccessModifier(Token? token)
        {
            if (token != null && _currentState == State.EndField && _accessModified.Contains(token.Value))
            {
                _currentState = State.FiledAccessModifier;
                return null;
            }
            
            if (token != null && _currentState == State.EndField && token.Value == "}")
            {
                _currentState = State.CloseStruct;
                return null;
            }

            if (_currentState == State.EndField)
            {
                return ParseError.CloseStructBracketExpectedOrFieldAccessModifierExpected;
            }
            
            return ParseStructEnd(token);
        }
        
        private ParseError? ParseStructEnd(Token? token)
        {
            if (token != null && _currentState == State.CloseStruct && token.Value == ";")
            {
                _currentState = State.End;
                return null;
            }
            
            if (_currentState == State.CloseStruct)
            {
               return ParseError.EndOfStructExpected;
            }
            
            return ParseEnd(token);
        }
        
        private ParseError? ParseEnd(Token? token)
        {
            if (_currentState == State.End && token == null)
            {
                return null;
            }
            
            if (_currentState == State.End)
            {
                return ParseError.NothingExpected;
            }
            
            return null;
        }
    }
}
    
    