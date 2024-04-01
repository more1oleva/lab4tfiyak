using System;
using System.Collections.Generic;
using System.Text;

namespace лб1тфияк
{
    public class Token
    {
        public int Type { get; }
        public string Value { get; }
        public int LineNumber { get; }
        public int StartPos { get; }

        public Token(int type, string value, int lineNumber, int startPos)
        {
            Type = type;
            Value = value;
            LineNumber = lineNumber;
            StartPos = startPos;
        }
    }
}
