// Source - https://stackoverflow.com/a
// Posted by Matthew Watson
// Retrieved 2025-11-26, License - CC BY-SA 4.0

namespace System.Runtime.CompilerServices
{
#if !NET7_0_OR_GREATER

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class RequiredMemberAttribute : Attribute { }

#endif // !NET7_0_OR_GREATER
}

