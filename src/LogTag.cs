namespace Niolog
{
    public class LogTag
    {
        public string Name{get;set;}
        public string Value{get;set;}

        public override string ToString()
        {
            return $"{this.Name}:>{this.Value}";
        }
    }
}