namespace Niolog
{
    public class LogTag
    {
        private string name;
        private string value;

        public LogTag(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public override string ToString()
        {
            return $"{this.name}:>{this.value}";
        }
    }
}