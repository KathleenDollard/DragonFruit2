using DragonFruit2.Validators;

namespace DragonFruit2;
//

public record class Diagnostic(string Id, DiagnosticSeverity Severity, string? ValueName = null, string? Message = null) { }

public record class Diagnostic<TValue>(string Id, DiagnosticSeverity Severity, string ValueName, TValue Value, string? Message = null)
    : Diagnostic(Id, Severity, ValueName, Message)
{ }