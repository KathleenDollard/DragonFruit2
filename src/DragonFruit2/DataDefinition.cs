namespace DragonFruit2;

public class DataDefinition
{
    public IEnumerable<string> ReusableDefinitionsUsed = new List<string>();

    public DataDefinition(string fullName)
    {
        FullName = fullName;
    }

    public string FullName { get; }
    public string Name => FullName.Split('.').Last();

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
