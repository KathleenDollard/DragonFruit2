namespace DragonFruit2;

public class CommandDataDefinition
{
    public required  string FullName { get; set; }
    public string Name => FullName.Split('.').Last();
    public string PosixName
    => field is null
        ? Name.ToKebabCase()
        : field;
}
