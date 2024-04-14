namespace лб1тфияк
{
    public class ParsingError
    {
        public int NumberOfError { get; set; }
        public string Message { get; set; } = "";
        public string ErrorToken { get; set; } = "";
        public string NeedToken { get; set; } = "";
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}