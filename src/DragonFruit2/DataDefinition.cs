namespace DragonFruit2;

public class DataDefinition
{
    public IEnumerable<string> ReusableDefinitionsUsed = new List<string>();

    public DataDefinition(string? name = null)
    {
        ArgsType = this.GetType();
        name = name is null && this is ArgumentDataDefinition
            ? ArgsType.Name
            : name;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Type ArgsType { get; }
    public string Name { get; } 

    public ObsoleteInfo? Obsolete { get; set; }
    public bool HiddenInCli { get; set; }
    public bool IgnoreOnGeneration { get; set; }

    public string PosixName
        => field is null
            ? Name.ToPosixName()
            : field;
    public string JsonName
        => field is null
            ? Name.ToJsonName()
            : field;
    public string XmlName
    => field is null
        ? Name.ToXmlName()
        : field;
    public string ConstantName
        => field is null
            ? Name.ToConstantName()
            : field;
    public string ConfigName
        => field is null
            ? Name.ToConfigName()
            : field;
}
