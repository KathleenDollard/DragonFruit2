namespace DragonFruit2;

public class DataDefinition
{
    public IEnumerable<string> ReusableDefinitionsUsed = new List<string>();

    public DataDefinition(string name)
    {
        DefinitionName = name ?? throw new ArgumentNullException(nameof(name));
    }
 
    public string DefinitionName { get; } 

    public ObsoleteInfo? Obsolete { get; set; }
    public bool HiddenInCli { get; set; }
    public bool IgnoreOnGeneration { get; set; }

    public string PosixName
        => field is null
            ? DefinitionName.ToPosixName()
            : field;
    public string JsonName
        => field is null
            ? DefinitionName.ToJsonName()
            : field;
    public string XmlName
    => field is null
        ? DefinitionName.ToXmlName()
        : field;
    public string ConstantName
        => field is null
            ? DefinitionName.ToConstantName()
            : field;
    public string ConfigName
        => field is null
            ? DefinitionName.ToConfigName()
            : field;
}
