using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace лб1тфияк
{
    public class FixErrors
    {
        public static string Fix(string code)
        {
            while (true)
            {
                Parser parser = new Parser();
                Lexer lexer = new Lexer(code);

                var lexems = lexer.Analyze();
                List<Token> buffer = new List<Token>();
                
                foreach (var lexem in lexems)
                {
                    if (lexem.Value != " Пробел ") buffer.Add(lexem);
                }
                
                var errors = parser.Parse(buffer, code);
                
                if (errors.Count == 0)
                {
                    return code;
                }
                
                code = FixParseErrors(code, errors.First());
            }
        }

        private const string access = "public";
        private const string type = "int";
        private const string name = "a";
        private const string structure = "struct";
        private const string openBrace = "{";
        private const string closeBrace = "}";
        private const string semicolon = ";";

        private static string FixParseErrors(string code, ParsingError error)
        {
            int lengthToRemove = error.EndIndex - error.StartIndex;
            if (error.StartIndex + lengthToRemove > code.Length)
            {
                lengthToRemove = code.Length - error.StartIndex;
            }
            
            var value = code.Remove(error.StartIndex , lengthToRemove);

            return error.ParseError switch
            {
                ParseError.InvalidToken => value,
                ParseError.NothingExpected => value,
                ParseError.AccessModifierExpected => value.Insert(error.StartIndex  , access),
                ParseError.StructKeywordExpected => value.Insert(error.StartIndex + 1, structure + " "),
                ParseError.StructIdentifierExpected => value.Insert(error.StartIndex + 1 , " " + name + " "),
                ParseError.OpenStructBracketExpected => value.Insert(error.StartIndex, " " + openBrace + " "),
                ParseError.FieldAccessModifierExpectedOrStructEndExpected => value.Insert(error.StartIndex, " " + closeBrace + " "),
                ParseError.FieldDataTypeExpected => value.Insert(error.StartIndex, type),
                ParseError.FieldIdentifierExpected => value.Insert(error.StartIndex + 1, name),
                ParseError.FieldEndExpected => value.Insert(error.StartIndex + 1, semicolon),
                ParseError.CloseStructBracketExpectedOrFieldAccessModifierExpected => value.Insert(error.StartIndex,
                    closeBrace),
                ParseError.EndOfStructExpected => value.Insert(error.StartIndex, " " + semicolon + " "),
                _ => value
            };
        }
    }
}