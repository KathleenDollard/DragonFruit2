namespace DragonFruit2;

/// <summary>
/// This class provides semantics for the concept that a function, may have no meaningful return value.
/// 
/// This papers over the disctinction in .NET between a method that returns avalue and those that do not.
/// </summary>
/// <remarks>
/// This is used primarily for IOperate to allow a single generic declaration. Using Void as the generic
/// return type. Using object would allow a value to be returned, when that is not the intent.
/// </remarks>
public struct Void
{
}
