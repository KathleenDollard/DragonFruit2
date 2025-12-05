using System.CommandLine;
using System.Reflection;

namespace DragonFruit2;

public static class CliExtensions
{
    extension(System.CommandLine.Command command)
    {
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

    extension(Type type)
    {
        /// <summary>
        /// Returns public instance properties declared on <paramref name="type"/> (including inherited).
        /// </summary>
        public IEnumerable<PropertyInfo> GetPublicInstanceProperties()
        {
            ArgumentNullException.ThrowIfNull(type);
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
