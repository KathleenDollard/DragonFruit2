namespace DragonFruit2.Generators.Test;

public class StringBuilderWrapperTests
{
    [Fact]
    public void AppendLineWithTextIncludesNewLine()
    {
        var s = "Hello";
        StringBuilderWrapper sb = new();

        sb.AppendLine(s);

        Assert.Equal($"Hello{Environment.NewLine}", sb.ToString());
    }

    [Fact]
    public void AppendLineEmptyTextIsJustNewLine()
    {
        var s = "";
        StringBuilderWrapper sb = new();

        sb.AppendLine(s);

        Assert.Equal($"{Environment.NewLine}", sb.ToString());
    }

    [Fact]
    public void AppendLineNUllTextIsJustNewLine()
    {
        string? s = null;
        StringBuilderWrapper sb = new();

        sb.AppendLine(s);

        Assert.Equal($"{Environment.NewLine}", sb.ToString());
    }

    [Fact]
    public void AppendWithTextIsJustText()
    {
        string? s = "Hello";
        StringBuilderWrapper sb = new();

        sb.AppendLine(s);

        Assert.Equal($"Hello", sb.ToString());
    }

    [Fact]
    public void AppendLinesWithMultipleLines()
    {
        string[] s = ["Hello", "World", "and", "universe"];
        StringBuilderWrapper sb = new();
        var expected = """
            Hello
            World
            and
            universe
            """;

        sb.AppendLines(s);

        Assert.Equal(expected, sb.ToString());
    }


    [Fact]
    public void AppendLinesWithOneLine()
    {
        string[] s = ["Hello"];
        StringBuilderWrapper sb = new();
        var expected = """
            Hello
            """;

        sb.AppendLines(s);

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void AppendLinesWithMultipleLinesAndNulls()
    {
        string?[] s = ["Hello", "World", null, "and", "universe"]; ;
        StringBuilderWrapper sb = new();
        var expected = """
            Hello
            World

            and
            universe
            """;

        sb.AppendLines(s);

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void AppendLinesWithNulls()
    {
        string?[] s = ["Hello", "World", null, "and", "universe"]; ;
        StringBuilderWrapper sb = new();
        var expected = """
            Hello
            World

            and
            universe
            """;

        sb.AppendLines(s);

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void AppendLinesWithNullTextIsJustNewLine()
    {
        string?[] s = null; ;
        StringBuilderWrapper sb = new();
        var expected = """

            """;

        sb.AppendLines(s);

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void OpenCurlyIsCurlyAndNewLine()
    {
        StringBuilderWrapper sb = new();
        var expected = """
            {
            """;

        sb.OpenCurly();

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void CloseCurlyIsCurlyAndNewLine()
    {
        StringBuilderWrapper sb = new();
        var expected = """
            }
            """;

        sb.CloseCurly();

        Assert.Equal(expected, sb.ToString());
    }


    [Fact]
    public void OpenNamespaceIsNameCurlyAndNewLine()
    {
        var nspaceName = "MyNamespace";
        StringBuilderWrapper sb = new();
        var expected = $"""
            namespace {nspaceName}
            """;

        sb.OpenNamespace(nspaceName);

        Assert.Equal(expected, sb.ToString());
    }

    [Fact]
    public void CloseNamespaceIsCurlyAndNewLine()
    {
        var nspaceName = "MyNamespace";
        StringBuilderWrapper sb = new();
        var expected = $"""
            namespace {nspaceName}
            """;

        sb.CloseNamespace(nspaceName);

        Assert.Equal(expected, sb.ToString());
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

    internal void Comment(string line)
    {
        AppendLine($"// {line}");
    }

    internal void Return(string? returnValue = null)
    {
        AppendLine($"return {returnValue};");
    }
}
