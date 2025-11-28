using System.Reflection;

namespace DragonFruit2;

public static class TypeExtensions
{
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
