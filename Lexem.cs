using System;
using System.Collections.Generic;
using System.Text;

namespace лб1тфияк
{
    public class Lexem
    {
        public int Type { get; }
        public string Value { get; }
        public int LineNumber { get; }
        public int StartPos { get; }
        public Token Token { get; }

        public Lexem(int type, string value, int lineNumber, int startPos, Token token)
        {
            Type = type;
            Value = value;
            LineNumber = lineNumber;
            StartPos = startPos;
            Token = token;
        }
    }
}
