namespace DragonFruit2
{
    public class OptionDataDefinition : MemberDataDefinition
    {
        public OptionDataDefinition(Type argsType, string name)
            : base(argsType, name)
        {
        }

        public string OptionName => $"--{PosixName}";

        public bool Recursive { get; set; }
        public List<ArgumentDataDefinition> Arguments => [];
        public List<string> CliAliases { get; } = [];
    }
}