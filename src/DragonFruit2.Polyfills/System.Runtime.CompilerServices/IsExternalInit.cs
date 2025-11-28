// Source - https://stackoverflow.com/a
// Posted by Matthew Watson
// Retrieved 2025-11-26, License - CC BY-SA 4.0


// Source - https://stackoverflow.com/a
// Posted by Matthew Watson
// Retrieved 2025-11-26, License - CC BY-SA 4.0

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
#if !NET5_0_OR_GREATER

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IsExternalInit { }
#endif // !NET5_0_OR_GREATER

}



