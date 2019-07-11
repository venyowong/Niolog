namespace Niolog
{
    public class LogTag
    {
        public string Name{get;set;}
        public string Value{get;set;}

        public LogTag(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{this.Name}:>{this.Value}";
        }
    }
}