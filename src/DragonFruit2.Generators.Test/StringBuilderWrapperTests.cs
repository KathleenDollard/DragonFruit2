namespace DragonFruit2.Generators.Test;

public class StringBuilderWrapperTests
{
    private static readonly string NL = Environment.NewLine;

    [Fact]
    public void AppendLine_WithText_IncludesTextWithoutIndent()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.AppendLine("public class Foo");

        var result = wrapper.ToString();
        Assert.Equal($"public class Foo{NL}", result);
    }

    [Fact]
    public void AppendLine_Empty_CreatesBlankLine()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.AppendLine();

        var result = wrapper.ToString();
        Assert.Equal(NL, result);
    }

    [Fact]
    public void AppendLine_AfterOpenCurly_IncludesIndentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.AppendLine("public string Name { get; set; }");

        var result = wrapper.ToString();
        Assert.Contains($"{{{NL}", result);
        Assert.Contains($"    public string Name {{ get; set; }}{NL}", result);
    }

    [Fact]
    public void Append_WithText_AddsTextWithIndent()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.Append("return value");

        var result = wrapper.ToString();
        Assert.Contains("    return value", result);
    }

    [Fact]
    public void AppendLines_WithMultipleLines_AppendsEachWithIndentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        var lines = new[] { "int x = 1;", "int y = 2;", "int z = 3;" };
        wrapper.AppendLines(lines);

        var result = wrapper.ToString();
        Assert.Contains($"    int x = 1;{NL}", result);
        Assert.Contains($"    int y = 2;{NL}", result);
        Assert.Contains($"    int z = 3;{NL}", result);
    }

    [Fact]
    public void OpenCurly_IncrementsIndentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.AppendLine("level1");
        wrapper.OpenCurly();
        wrapper.AppendLine("level2");

        var result = wrapper.ToString();
        Assert.Contains($"    level1{NL}", result);
        Assert.Contains($"        level2{NL}", result);
    }

    [Fact]
    public void CloseCurly_DecrementsIndentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.AppendLine("level1");
        wrapper.CloseCurly();
        wrapper.AppendLine("level0");

        var result = wrapper.ToString();
        Assert.Contains($"    level1{NL}", result);
        Assert.Contains($"}}{NL}", result);
        Assert.Contains($"level0{NL}", result);
    }

    [Fact]
    public void CloseCurly_WithCloseParens_AddsClosingParenthesis()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.CloseCurly(closeParens: true);

        var result = wrapper.ToString();
        Assert.Contains($"}}){NL}", result);
    }

    [Fact]
    public void CloseCurly_WithEndStatement_AddsStatementTerminator()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.CloseCurly(endStatement: true);

        var result = wrapper.ToString();
        Assert.Contains($"}};{NL}", result);
    }

    [Fact]
    public void CloseCurly_WithBothFlags_AddsBothClosingCharacters()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.CloseCurly(closeParens: true, endStatement: true);

        var result = wrapper.ToString();
        Assert.Contains($"}});{NL}", result);
    }

    [Fact]
    public void OpenNamespace_WithValidNamespace_GeneratesProperNamespaceBlock()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenNamespace("SampleApp");
        wrapper.AppendLine("public class Foo { }");
        wrapper.CloseNamespace("SampleApp");

        var result = wrapper.ToString();
        Assert.Contains($"namespace SampleApp{NL}", result);
        Assert.Contains($"{{{NL}", result);
        Assert.Contains($"    public class Foo {{ }}{NL}", result);
        Assert.Contains($"}}{NL}", result);
    }

    [Fact]
    public void OpenNamespace_WithNullOrEmpty_SkipsNamespaceDeclaration()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenNamespace(null);
        wrapper.AppendLine("public class Foo { }");
        wrapper.CloseNamespace(null);

        var result = wrapper.ToString();
        Assert.DoesNotContain("namespace", result);
        Assert.Contains("public class Foo { }", result);
    }

    [Fact]
    public void OpenClass_WithDeclarationAndConstraints_GeneratesClassWithConstraints()
    {
        var wrapper = new StringBuilderWrapper();
        var constraints = new[] { "TArgs : Args<TArgs>" };
        wrapper.OpenClass("public abstract partial class CommandInfo<TArgs>", constraints);
        wrapper.AppendLine("// class body");
        wrapper.CloseClass();

        var result = wrapper.ToString();
        Assert.Contains($"public abstract partial class CommandInfo<TArgs>{NL}", result);
        Assert.Contains($"    where TArgs : Args<TArgs>{NL}", result);
        Assert.Contains($"{{{NL}", result);
        Assert.Contains($"    // class body{NL}", result);
    }

    [Fact]
    public void OpenConstructor_WithBaseCtor_IncludesBaseConstructorCall()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenConstructor("public Foo(string name)", "base(name)");
        wrapper.AppendLine("this.Name = name;");
        wrapper.CloseConstructor();

        var result = wrapper.ToString();
        Assert.Contains($"public Foo(string name){NL}", result);
        Assert.Contains($"    : base(name){NL}", result);
        Assert.Contains($"    this.Name = name;{NL}", result);
    }

    [Fact]
    public void OpenConstructor_WithoutBaseCtor_OmitsBaseCall()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenConstructor("public Foo()");
        wrapper.AppendLine("this.Value = 0;");
        wrapper.CloseConstructor();

        var result = wrapper.ToString();
        Assert.Contains($"public Foo(){NL}", result);
        Assert.DoesNotContain(":", result);
    }

    [Fact]
    public void OpenMethod_WithDeclarationAndConstraints_GeneratesMethodWithConstraints()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenMethod("public T GetValue<T>()", "T : new()");
        wrapper.AppendLine("return new T();");
        wrapper.CloseMethod();

        var result = wrapper.ToString();
        Assert.Contains($"public T GetValue<T>(){NL}", result);
        Assert.Contains($"      where T : new(){NL}", result);
    }

    [Fact]
    public void OpenIf_GeneratesIfBlock()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenIf("x > 0");
        wrapper.AppendLine("Console.WriteLine(\"positive\");");
        wrapper.CloseIf();

        var result = wrapper.ToString();
        Assert.Contains($"if (x > 0){NL}", result);
        Assert.Contains($"{{{NL}", result);
        Assert.Contains($"    Console.WriteLine(\"positive\");{NL}", result);
    }

    [Fact]
    public void OpenElseIfAndClosePreviousIf_TransitionsFromIfToElseIf()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenIf("x > 0");
        wrapper.AppendLine("Console.WriteLine(\"positive\");");
        wrapper.OpenElseIfAndClosePreviousIf("x < 0");
        wrapper.AppendLine("Console.WriteLine(\"negative\");");
        wrapper.CloseIf();

        var result = wrapper.ToString();
        Assert.Contains($"if (x > 0){NL}", result);
        Assert.Contains($"else if (x < 0){NL}", result);
    }

    [Fact]
    public void OpenElseAndClosePreviousIf_TransitionsFromIfToElse()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenIf("condition");
        wrapper.AppendLine("DoSomething();");
        wrapper.OpenElseAndClosePreviousIf();
        wrapper.AppendLine("DoOtherThing();");
        wrapper.CloseIf();

        var result = wrapper.ToString();
        Assert.Contains($"if (condition){NL}", result);
        Assert.Contains($"else{NL}", result);
    }

    [Fact]
    public void OpenForEach_GeneratesForEachLoop()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenForEach("var item in collection");
        wrapper.AppendLine("Process(item);");
        wrapper.CloseForEach();

        var result = wrapper.ToString();
        Assert.Contains($"foreach (var item in collection){NL}", result);
        Assert.Contains($"    Process(item);{NL}", result);
    }

    [Fact]
    public void XmlSummary_GeneratesXmlDocumentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.XmlSummary("This is a test class.");

        var result = wrapper.ToString();
        Assert.Contains($"/// <summary>{NL}", result);
        Assert.Contains($"/// This is a test class.{NL}", result);
        Assert.Contains($"/// </summary>{NL}", result);
    }

    [Fact]
    public void XmlRemarks_GeneratesXmlDocumentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.XmlRemarks("Additional remarks go here.");

        var result = wrapper.ToString();
        Assert.Contains($"/// <remarks>{NL}", result);
        Assert.Contains($"/// Additional remarks go here.{NL}", result);
        Assert.Contains($"/// </remarks>{NL}", result);
    }

    [Fact]
    public void XmlTypeParam_GeneratesTypeParameterDocumentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.XmlTypeParam("T", "The type parameter.");

        var result = wrapper.ToString();
        Assert.Contains($"""/// <typeparam name="T">The type parameter.</typeparam>{NL}""", result);
    }

    [Fact]
    public void XmlParam_GeneratesParameterDocumentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.XmlParam("value", "The input value.");

        var result = wrapper.ToString();
        Assert.Contains($"""/// <param name="value">The input value.</param>{NL}""", result);
    }

    [Fact]
    public void XmlReturns_GeneratesReturnDocumentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.XmlReturns("The computed result.");

        var result = wrapper.ToString();
        Assert.Contains($"""/// <returns>The computed result.</returns>{NL}""", result);
    }

    [Fact]
    public void XmlException_GeneratesExceptionDocumentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.XmlException("System.ArgumentNullException", "When value is null.");

        var result = wrapper.ToString();
        Assert.Contains($"""/// <exception cref="System.ArgumentNullException">When value is null.</param>{NL}""", result);
    }

    [Fact]
    public void Comment_GeneratesLineComment()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.Comment("TODO: implement validation");
        wrapper.CloseCurly();

        var result = wrapper.ToString();
        Assert.Contains($"    // TODO: implement validation{NL}", result);
    }

    [Fact]
    public void Return_WithValue_GeneratesReturnStatement()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.Return("result");
        wrapper.CloseCurly();

        var result = wrapper.ToString();
        Assert.Contains($"    return result;{NL}", result);
    }

    [Fact]
    public void Return_WithoutValue_GeneratesVoidReturn()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenCurly();
        wrapper.Return();
        wrapper.CloseCurly();

        var result = wrapper.ToString();
        Assert.Contains($"    return ;{NL}", result);
    }

    [Fact]
    public void MultipleNestingLevels_MaintainsCorrectIndentation()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.OpenNamespace("MyNamespace");
        wrapper.OpenClass("public class MyClass");
        wrapper.OpenMethod("public void MyMethod()");
        wrapper.OpenIf("x > 0");
        wrapper.AppendLine("value = 42;");
        wrapper.CloseIf();
        wrapper.CloseMethod();
        wrapper.CloseClass();
        wrapper.CloseNamespace("MyNamespace");

        var result = wrapper.ToString();

        // Verify indentation levels (should be 4, 8, 12, 16 spaces respectively)
        Assert.Contains($"    public class MyClass{NL}", result);
        Assert.Contains($"        public void MyMethod(){NL}", result);
        Assert.Contains($"            if (x > 0){NL}", result);
        Assert.Contains($"                value = 42;{NL}", result);
    }

    [Fact]
    public void ToString_ReturnsAccumulatedContent()
    {
        var wrapper = new StringBuilderWrapper();
        wrapper.AppendLine("line1");
        wrapper.AppendLine("line2");

        var result = wrapper.ToString();
        Assert.Equal($"line1{NL}line2{NL}", result);
    }

    [Fact]
    public void MultipleInstances_AreIndependent()
    {
        var wrapper1 = new StringBuilderWrapper();
        var wrapper2 = new StringBuilderWrapper();

        wrapper1.OpenCurly();
        wrapper1.AppendLine("content1");

        wrapper2.AppendLine("content2");

        var result1 = wrapper1.ToString();
        var result2 = wrapper2.ToString();

        Assert.Contains("    content1", result1);
        Assert.DoesNotContain("content2", result1);
        Assert.Contains("content2", result2);
        Assert.DoesNotContain("content1", result2);
    }
}