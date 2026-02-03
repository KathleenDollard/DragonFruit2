namespace DragonFruit2
{
    public class OptionDataDefinition : MemberDataDefinition
    {
        public OptionDataDefinition(string fullName) 
            : base(fullName)
        {
        }

        public bool Recursive { get; set; }
        public List<ArgumentDataDefinition> Arguments => [];
        public List<string> CliAliases { get; } = [];
    }
}