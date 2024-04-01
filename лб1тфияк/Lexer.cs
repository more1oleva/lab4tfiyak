using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace лб1тфияк
{
    public enum LexemeType
    {
        Keyword,
        Identifier,
        Operator,
        Separator,
        Invalid
    }
    
    public class Lexer
    {
        private readonly string _inputText;
        private int _currentIndex;
        public readonly List<Token> Tokens;

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
            Tokens = new List<Token>();
        }

        public List<Token> Analyze()
        {
            while (_currentIndex < _inputText.Length)
            {
                var currentChar = _inputText[_currentIndex];

                if (char.IsWhiteSpace(currentChar))
                {
                    _currentIndex++;

                    Tokens.Add(new Token
                    (
                        GetCode(" Пробел ", LexemeType.Separator),
                        " Пробел ",
                        0,
                        _currentIndex
                    ));

                    continue;
                }
                if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    AnalyzeIdentifier();
                }
                else if (currentChar == '{' || currentChar == '}' || currentChar == ';' || currentChar == ' ')
                {
                    AnalyzeOperator();
                }
                else
                {
                    AddToken(LexemeType.Invalid, currentChar.ToString());
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

        private LexemeType GetIdentifierType(string identifier)
        {
            if (conditionalCodes.ContainsKey(identifier))
            {
                return LexemeType.Keyword;
            }
            else if (char.IsDigit(identifier[0]))
            {
                return LexemeType.Invalid;
            }
            else
            {
                return LexemeType.Identifier;
            }
        }

        private void AnalyzeOperator()
        {
            var startIndex = _currentIndex;
            _currentIndex++;
            AddToken(LexemeType.Operator, _inputText.Substring(startIndex, 1));
        }

        private void AddToken(LexemeType type, string value)
        {
            Tokens.Add(new Token
            (
                GetCode(value, type),
                value,
                0,
                _currentIndex - value.Length + 1
            ));
        }

        private int GetCode(string value, LexemeType type)
        {
            if (type == LexemeType.Identifier) return 12;

            if (conditionalCodes.ContainsKey(value))
            {
                return Convert.ToInt32(conditionalCodes[value]);
            }

            return -1;
        }
    }
}

