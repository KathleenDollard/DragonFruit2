using Microsoft.CodeAnalysis;

namespace DragonFruit2.Generators;

    public static class Helpers
{
    extension(Accessibility accessibility)
    {
        public string ToCSharpString()
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return "private";
                case Accessibility.Protected:
                    return "protected";
                case Accessibility.Internal:
                    return "internal";
                case Accessibility.Public:
                    return "public";
                case Accessibility.ProtectedOrInternal:
                    // This corresponds to 'protected internal' in C#
                    return "protected internal";
                case Accessibility.ProtectedAndInternal:
                    // This is a rare/internal accessibility level, typically handled as 'private protected' in C# syntax
                    return "private protected";
                case Accessibility.NotApplicable:
                default:
                    return string.Empty; // For namespaces, modules, etc.
            }
        }
    }
}
