namespace DragonFruit2
{
    public class OptionDataDefinition<TValue> : MemberDataDefinition<TValue>
    {
        public OptionDataDefinition(Type argsType, string name)
            : base(argsType, name, true)
        {
        }

        public string OptionName => $"--{PosixName}";

        public bool Recursive { get; set; }
        public List<ArgumentDataDefinition<TValue>> Arguments => [];
        public List<string> CliAliases { get; } = [];
    }
}