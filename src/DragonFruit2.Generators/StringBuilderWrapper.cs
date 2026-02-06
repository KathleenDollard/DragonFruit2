using System.Text;

namespace DragonFruit2.Generators;

public class StringBuilderWrapper
{
    private readonly int indentSize = 4;
    private string indent = "";

    private readonly StringBuilder _sb = new();
    public void AppendLine(string line)
    {
        _sb.AppendLine($"{indent}{line}");
    }
    public void AppendLine()
    {
        _sb.AppendLine();
    }
    public void Append(string line)
    {
        _sb.Append($"{indent}{line}");
    }

    public void AppendLines(IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            AppendLine(line);
        }
    }

    public override string ToString()
    {
        return _sb.ToString();
    }

    public void OpenCurly()
    {
        AppendLine($$"""{""");
        indent += new string(' ', indentSize);
    }

    public void CloseCurly(bool closeParens = false, bool endStatement = false)
    {
        indent = indent.Substring(indentSize);
        AppendLine($$"""}{{(closeParens ? ")" : "")}}{{(endStatement ? ";" : "")}}""");
    }

    public void OpenNamespace(string? namespaceName)
    {
        AppendLine();
        if (!string.IsNullOrEmpty(namespaceName))
        {
            AppendLine($"namespace {namespaceName}");
            OpenCurly();
        }
    }
    public void CloseNamespace(string? namespaceName)
    {
        if (!string.IsNullOrEmpty(namespaceName))
        {
            CloseCurly();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="classDeclaration"></param>
    /// <param name="bases"></param>
    /// <param name="constraints">Should not include `where`, for example T : new()</param>
    public void OpenClass(string classDeclaration, string[]? constraints = null)
    {
        AppendLine(classDeclaration);
        if (constraints is not null && constraints.Any())
        {
            foreach (var constraint in constraints)
            {
                AppendLine($"    where {constraint}");
            }
        }
        OpenCurly();
    }
    public void CloseClass()
        => CloseCurly();

    public void OpenConstructor(string declaration, string? baseCtor = null)
    {
        AppendLine();
        AppendLine(declaration);
        if (baseCtor is not null)
        {
            AppendLine($"    : {baseCtor}");
        }
        OpenCurly();
    }
    public void CloseConstructor()
       => CloseCurly();

    public void OpenMethod(string declaration, string? constraints = null)
    {
        AppendLine();
        AppendLine(declaration);
        if (constraints is not null)
        {
            AppendLine($"      where {constraints}");
        }
        OpenCurly();
    }
    public void CloseMethod()
       => CloseCurly();

    internal void OpenIf(string condition)
    {
        AppendLine($"if ({condition})");
        OpenCurly();
    }
    internal void CloseIf() => CloseCurly();
    internal void OpenElseIfAndClosePreviousIf(string condition)
    {
        CloseCurly();
        AppendLine($"else if ({condition})");
        OpenCurly();
    }
    internal void OpenElseAndClosePreviousIf()
    {
        CloseCurly();
        AppendLine($"else");
        OpenCurly();
    }

    internal void OpenForEach(string loopString)
    {
        AppendLine($"foreach ({loopString})");
        OpenCurly();
    }
    internal void CloseForEach() => CloseCurly();

    internal void XmlSummary(string summary)
    {
        AppendLine("/// <summary>");
        AppendLine($"/// {summary}");
        AppendLine("/// </summary>");
    }
    internal void XmlRemarks(string remarks)
    {
        AppendLine("/// <remarks>");
        AppendLine($"/// {remarks}");
        AppendLine("/// </remarks>");
    }
    internal void XmlTypeParam(string name, string text)
    {
        AppendLine($"""/// <typeparam name="{name}">{text}</typeparam>""");
    }
    internal void XmlParam(string name, string text)
    {
        AppendLine($"""/// <param name="{name}">{text}</param>""");
    }
    internal void XmlReturns(string text)
    {
        AppendLine($"""/// <returns>{text}</returns>""");
    }
    internal void XmlException(string exceptionTypeName, string text)
    {
        AppendLine($"""/// <exception cref="{exceptionTypeName}">{text}</param>""");
    }
    internal void XmlBreak()
    {
        AppendLine("/// <br/>");
    }

    internal void Comment(string line)
    {
        AppendLine($"// {line}");
    }

    internal void Return(string? returnValue = null, bool noSemicolon = false)
    {
        AppendLine($"""return {returnValue}{(noSemicolon ? "" : ";")}""");
    }

    internal string CSharpString<T>(T input)
    {
        return input switch
        {
            null => "null",
            string s => $"@\"{s.Replace("\"", "\"\"")}\"",
            char c => $"'{c}'",
            bool b => b ? "true" : "false",
            Enum e => $"{e.GetType().FullName}.{e}",
            _ when input.GetType().IsPrimitive => input.ToString()!,
            _ => throw new NotSupportedException($"Type {typeof(T).FullName} is not supported for C# literal conversion"),
        };
    }
}