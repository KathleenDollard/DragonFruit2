using DragonFruit2.Validators;

namespace DragonFruit2;
//

public record class Diagnostic(string Id, string Message, string? ValueName, DiagnosticSeverity Severity) { }

public record class Diagnostic<TValue>(string Id, string Message, string ValueName, DiagnosticSeverity Severity, TValue Value )
    : Diagnostic(Id, Message, ValueName, Severity)
{ }