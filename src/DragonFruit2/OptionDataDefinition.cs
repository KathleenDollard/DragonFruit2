namespace DragonFruit2
{
    public class OptionDataDefinition<TValue> : MemberDataDefinition<TValue>
    {
        public OptionDataDefinition(CommandDataDefinition commandDefinition, string name)
            : base(commandDefinition, name, true)
        {
        }

        public string OptionName => $"--{PosixName}";

        public bool Recursive { get; set; }
        public List<ArgumentDataDefinition<TValue>> Arguments => [];
        public List<string> CliAliases { get; } = [];
    }
}