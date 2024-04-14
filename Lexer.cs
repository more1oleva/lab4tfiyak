using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace лб1тфияк
{
    public enum Token
    {
        KEYWORD, //
        IDENTIFIER,
        SEPARATOR, //
        DATA_TYPE,
        ACCESS_MODIFIER ,
        STRUCT_KEYWORD,
        OPEN_BRACKET,
        CLOSE_BRACKET,
        END_OPERATOR,
        INVALID,
    }
    
    public class Lexer
    {
        private readonly string _inputText;
        private int _currentIndex;
        public readonly List<Lexem> Tokens;

        private readonly Dictionary<string, string> conditionalCodes = new Dictionary<string, string>
        {
            {"public", "1"},
            {"private", "2"},
            {"protected", "3"},
            {"struct", "4"},
            {"int", "5"},
            {"bool", "6"},
            {"char", "7"},
            {"string", "8"},
            {"true", "9"},
            {"false", "10"},
            {"{", "13"},
            {"}", "14"},
            {" Пробел ", "11"},
            {";", "15"}
        };

        public Lexer(string inputText)
        {
            _inputText = inputText;
            _currentIndex = 0;
            Tokens = new List<Lexem>();
        }

        public List<Lexem> Analyze()
        {
            while (_currentIndex < _inputText.Length)
            {
                var currentChar = _inputText[_currentIndex];

                if (char.IsWhiteSpace(currentChar))
                {
                    _currentIndex++;

                    Tokens.Add(new Lexem
                    (
                        GetCode(" Пробел ", Token.SEPARATOR),
                        " Пробел ",
                        0,
                        _currentIndex,
                        Token.SEPARATOR
                    ));

                    continue;
                }
                if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    AnalyzeIdentifier();
                }
                else if (currentChar == '{' || currentChar == '}' || currentChar == ';' || currentChar == ' ')
                {
                    AnalyzeOperator(currentChar);
                }
                else
                {
                    AddToken(Token.INVALID, currentChar.ToString());
                    _currentIndex++;
                }
            }

            return Tokens;
        }

        private void AnalyzeIdentifier()
        {
            var startIndex = _currentIndex;
            while (_currentIndex < _inputText.Length && (char.IsLetterOrDigit(_inputText[_currentIndex]) || _inputText[_currentIndex] == '_'))
            {
                _currentIndex++;
            }

            var identifier = _inputText.Substring(startIndex, _currentIndex - startIndex);

            var identifierType = GetIdentifierType(identifier);
            AddToken(identifierType, identifier);
        }

        private Token GetIdentifierType(string identifier)
        {
            if (conditionalCodes.ContainsKey(identifier))
            {
                if (identifier == "public" || identifier == "private" || identifier == "protected")
                {
                    return Token.ACCESS_MODIFIER;
                }
                else if (identifier == "struct")
                {
                    return Token.STRUCT_KEYWORD;
                }
                else if (identifier == "bool" || identifier == "int" || identifier == "char" || identifier == "string")
                {
                    return Token.DATA_TYPE;
                }
                else
                {
                    return Token.KEYWORD;
                }
            }
            else if (char.IsDigit(identifier[0]))
            {
                return Token.INVALID;
            }
            else
            {
                return Token.IDENTIFIER;
            }
        }

        private void AnalyzeOperator(char currentChar)
        {
            var startIndex = _currentIndex;
            _currentIndex++;
            
            if (currentChar == '{') AddToken(Token.OPEN_BRACKET, "{");
            else if (currentChar == '}') AddToken(Token.CLOSE_BRACKET, "}");
            else if (currentChar == ';') AddToken(Token.END_OPERATOR, ";");
        }

        private void AddToken(Token type, string value)
        {
            Tokens.Add(new Lexem
            (
                GetCode(value, type),
                value,
                0,
                _currentIndex - value.Length + 1,
                type
            ));
        }

        private int GetCode(string value, Token type)
        {
            if (type == Token.IDENTIFIER) return 12;

            if (conditionalCodes.ContainsKey(value))
            {
                return Convert.ToInt32(conditionalCodes[value]);
            }

            return -1;
        }
    }
}

