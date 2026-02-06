using System.CommandLine;

namespace DragonFruit2;

public static class SystemCommandLineExtensions
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

        public System.CommandLine.Command AddRange(IEnumerable<Option> options)
        {
            foreach (var option in options)
            {
                command.Add(option);
            }
            return command;
        }
        public System.CommandLine.Command AddRange(IEnumerable<Argument> arguments)
        {
            foreach (var argument in arguments)
            {
                command.Add(argument);
            }
            return command;
        }
        public System.CommandLine.Command AddRange(IEnumerable<System.CommandLine.Command> commands)
        {
            foreach (var childCommand in commands)
            {
                command.Add(childCommand);
            }
            return command;
        }
    }
}
