// Source - https://stackoverflow.com/a
// Posted by Matthew Watson
// Retrieved 2025-11-26, License - CC BY-SA 4.0

namespace System.Diagnostics.CodeAnalysis
{
#if !NET7_0_OR_GREATER
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class SetsRequiredMembersAttribute : Attribute
    {
        public SetsRequiredMembersAttribute()
        {
        }
    }
#endif
}

