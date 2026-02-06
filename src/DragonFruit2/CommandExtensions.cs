using System.CommandLine;

namespace DragonFruit2;

public static class CommandExtensions
{

    extension(Command command)
    {
        public void Add(Symbol symbol)
        {
            switch (symbol)
            {
                case Option option:
                    command.Add(option);
                    break;
                case Argument argument:
                    command.Add(argument);
                    break;
                case Command subcommand:
                    command.Add(subcommand);
                    break;
                default:
                    throw new NotSupportedException($"Symbol of type {symbol.GetType().FullName} is not supported");
            }
        }

        public void AddRange(IEnumerable<Symbol> memberSymbols)
        {
            foreach (var member in memberSymbols)
            {
                command.Add(member);
            }
        }
    }
}
