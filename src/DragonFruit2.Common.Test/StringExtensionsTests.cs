using Xunit;

namespace DragonFruit2.Common.Test;

public class StringExtensionsTests
{
    #region ToKebabCase

#pragma warning disable xUnit1012 // don't pass null to string
    [Theory]
    [InlineData("MyString", "my-string")]
    [InlineData("myString", "my-string")]
    [InlineData("HTTPServer", "http-server")]
    [InlineData("simple", "simple")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ToKebabCase_ReturnsCorrectFormat(string input, string expected)
    {
        var result = input.ToKebabCase();
        Assert.Equal(expected, result);
    }
#pragma warning restore xUnit1012

    [Theory]
    [InlineData("XMLParser", "xml-parser")]
    [InlineData("parseHTMLString", "parse-html-string")]
    [InlineData("IOError", "io-error")]
    [InlineData("MyHTTPSConnection", "my-https-connection")]
    [InlineData("ABC", "abc")]
    [InlineData("ABCDef", "abc-def")]
    [InlineData("myABCDef", "my-abc-def")]
    public void ToKebabCase_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToKebabCase();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToSnakeCase

    [Theory]
    [InlineData("MyString", "my_string")]
    [InlineData("myString", "my_string")]
    [InlineData("HTTPServer", "http_server")]
    [InlineData("simple", "simple")]
    public void ToSnakeCase_ReturnsCorrectFormat(string input, string expected)
    {
        var result = input.ToSnakeCase();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("XMLParser", "xml_parser")]
    [InlineData("parseHTMLString", "parse_html_string")]
    [InlineData("IOError", "io_error")]
    [InlineData("MyHTTPSConnection", "my_https_connection")]
    public void ToSnakeCase_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToSnakeCase();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("MyString", CasingStyle.ToUpper, "MY_STRING")]
    [InlineData("HTTPServer", CasingStyle.ToUpper, "HTTP_SERVER")]
    [InlineData("myString", CasingStyle.NoChanges, "my_String")]
    public void ToSnakeCase_RespectsCasingStyle(string input, CasingStyle casing, string expected)
    {
        var result = input.ToSnakeCase(casing);
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToPascalCase

#pragma warning disable xUnit1012
    [Theory]
    [InlineData("my-variable", "MyVariable")]
    [InlineData("my_variable_name", "MyVariableName")]
    [InlineData("myVariableName", "MyVariableName")]
    [InlineData("HTTPServer", "HttpServer")]
    [InlineData("simple", "Simple")]
    [InlineData("", "")]
    [InlineData(null, null)]
    public void ToPascalCase_ReturnsCorrectFormat(string input, string expected)
    {
        var result = input.ToPascalCase();
        Assert.Equal(expected, result);
    }
#pragma warning restore xUnit1012

    [Theory]
    [InlineData("http-server", "HttpServer")]
    [InlineData("xml-parser", "XmlParser")]
    [InlineData("io-error", "IoError")]
    [InlineData("https-connection", "HttpsConnection")]
    public void ToPascalCase_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToPascalCase();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToCamelCase

    [Theory]
    [InlineData("my-variable", "myVariable")]
    [InlineData("my_variable_name", "myVariableName")]
    [InlineData("MyVariableName", "myVariableName")]
    [InlineData("HTTPServer", "httpServer")]
    [InlineData("Simple", "simple")]
    public void ToCamelCase_ReturnsCorrectFormat(string input, string expected)
    {
        var result = input.ToCamelCase();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("XMLParser", "xmlParser")]
    [InlineData("IOError", "ioError")]
    [InlineData("HTTP-Server", "httpServer")]
    public void ToCamelCase_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToCamelCase();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToDisplayName

    [Theory]
    [InlineData("myVariableName", "My Variable Name")]
    [InlineData("my_variable_name", "My Variable Name")]
    [InlineData("my-variable-name", "My Variable Name")]
    [InlineData("HTTPServer", "Http Server")]
    [InlineData("simple", "Simple")]
    public void ToDisplayName_ReturnsReadableFormat(string input, string expected)
    {
        var result = input.ToDisplayName();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("XMLParser", "Xml Parser")]
    [InlineData("parseHTMLString", "Parse Html String")]
    [InlineData("IOError", "Io Error")]
    [InlineData("MyHTTPSConnection", "My Https Connection")]
    [InlineData("ABCDef", "Abc Def")]
    public void ToDisplayName_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToDisplayName();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToUrlSlug

    [Theory]
    [InlineData("My Blog Post Title", "my-blog-post-title")]
    [InlineData("Article_Title!", "article-title")]
    [InlineData("HTTPServer 2.0", "http-server-2-0")]
    [InlineData("hello-world", "hello-world")]
    [InlineData("simple", "simple")]
    [InlineData("", "")]
    public void ToUrlSlug_ReturnsUrlSafeFormat(string input, string expected)
    {
        var result = input.ToUrlSlug();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("XMLParser", "xml-parser")]
    [InlineData("HTTPSConnection", "https-connection")]
    [InlineData("My XML Parser 2.0", "my-xml-parser-2-0")]
    public void ToUrlSlug_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToUrlSlug();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToEnvironmentVariableName

    [Theory]
    [InlineData("MyVariable", "MY_VARIABLE")]
    [InlineData("myVarName", "MY_VAR_NAME")]
    [InlineData("config-file-path", "CONFIG_FILE_PATH")]
    [InlineData("2ndValue", "_2ND_VALUE")]
    [InlineData("simple", "SIMPLE")]
    public void ToConstantName_ReturnsUppercaseWithUnderscores(string input, string expected)
    {
        var result = input.ToConstantName();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("HTTPTimeout", "HTTP_TIMEOUT")]
    [InlineData("XMLParser", "XML_PARSER")]
    [InlineData("database-connection-string", "DATABASE_CONNECTION_STRING")]
    [InlineData("myHTTPSConnection", "MY_HTTPS_CONNECTION")]
    [InlineData("IOError", "IO_ERROR")]
    public void ToConstantName_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToConstantName();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToConfigName

    [Theory]
    [InlineData("AppSettings", "app-settings")]
    [InlineData("DatabaseConnectionString", "database-connection-string")]
    [InlineData("log_level_setting", "log-level-setting")]
    [InlineData("MyApp.Config", "my-app.config")]
    [InlineData("2ndLevelConfig", "2nd-level-config")]
    public void ToConfigName_ReturnsConfigFormat(string input, string expected)
    {
        var result = input.ToConfigName();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("HTTPTimeout", "http-timeout")]
    [InlineData("XMLParser.config", "xml-parser.config")]
    [InlineData("MyHTTPSConnection", "my-https-connection")]
    public void ToConfigName_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToConfigName();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToXmlName

    [Theory]
    [InlineData("MyFileName", "myFileName")]
    [InlineData("my file name", "myFileName")]
    [InlineData("config-file", "configFile")]
    [InlineData("XML_Config", "_xmlConfig")]
    [InlineData("simple", "simple")]
    public void ToXmlName_ReturnsXmlCompliantFormat(string input, string expected)
    {
        var result = input.ToXmlName();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToXmlName_RejectsReservedXmlPrefix()
    {
        var result = "xmlData".ToXmlName();
        Assert.StartsWith("_", result);
        Assert.Contains("xml", result);
    }

    [Fact]
    public void ToXmlName_PrefixesDigitStart()
    {
        var result = "2ndValue".ToXmlName();
        Assert.StartsWith("_", result);
    }

    [Theory]
    [InlineData("HTTPServer", "httpServer")]
    [InlineData("XMLParser", "_xmlParser")]
    [InlineData("IOError", "ioError")]
    public void ToXmlName_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToXmlName();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToJsonName

    [Theory]
    [InlineData("MyProperty", "myProperty")]
    [InlineData("my property name", "myPropertyName")]
    [InlineData("config-file", "configFile")]
    [InlineData("2ndValue", "_2ndValue")]
    public void ToJsonName_ReturnsCamelCaseFormat(string input, string expected)
    {
        var result = input.ToJsonName();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToJsonName_AllowsDollarSign()
    {
        var result = "$variable".ToJsonName();
        Assert.StartsWith("$", result);
    }

    [Theory]
    [InlineData("HTTPTimeout", "httpTimeout")]
    [InlineData("XMLParser", "xmlParser")]
    [InlineData("IOError", "ioError")]
    public void ToJsonName_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToJsonName();
        Assert.Equal(expected, result);
    }

    #endregion

    #region ToConstant

    [Theory]
    [InlineData("MyConstant", "MY_CONSTANT")]
    [InlineData("myConstantValue", "MY_CONSTANT_VALUE")]
    [InlineData("max-timeout", "MAX_TIMEOUT")]
    [InlineData("2ndValue", "_2ND_VALUE")]
    public void ToConstant_ReturnsUppercaseWithUnderscores(string input, string expected)
    {
        var result = input.ToConstantName();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ToConstant_EquivalentToEnvironmentVariableName()
    {
        var input = "MyConstantValue";
        var constant = input.ToConstantName();
        var envVar = input.ToConstantName();
        Assert.Equal(envVar, constant);
    }

    #endregion

    #region ToPosixName

    [Theory]
    [InlineData("MyFileName", "my-file-name")]
    [InlineData("config_file.txt", "config-file-txt")]
    [InlineData("HTTP Server 2.0", "http-server-2-0")]
    [InlineData("simple.txt", "simple-txt")]
    public void ToPosixName_ReturnsPosixCompliantFormat(string input, string expected)
    {
        var result = input.ToPosixName();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("XMLParser", "xml-parser")]
    [InlineData("HTTPServer.txt", "http-server-txt")]
    [InlineData("MyIOError", "my-io-error")]
    public void ToPosixName_HandlesAcronyms(string input, string expected)
    {
        var result = input.ToPosixName();
        Assert.Equal(expected, result);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void AllMethods_HandleNullInput()
    {
        string? nullInput = null;
#pragma warning disable CS8604
        Assert.Null(nullInput.ToKebabCase());
        Assert.Null(nullInput.ToSnakeCase());
        Assert.Null(nullInput.ToPascalCase());
        Assert.Null(nullInput.ToCamelCase());
        Assert.Null(nullInput.ToDisplayName());
        Assert.Null(nullInput.ToUrlSlug());
        Assert.Null(nullInput.ToConstantName());
        Assert.Null(nullInput.ToConfigName());
        Assert.Null(nullInput.ToXmlName());
        Assert.Null(nullInput.ToJsonName());
        Assert.Null(nullInput.ToConstantName());
        Assert.Null(nullInput.ToPosixName());
#pragma warning restore CS8604
    }

    [Fact]
    public void AllMethods_HandleEmptyString()
    {
        var empty = string.Empty;

        Assert.Empty(empty.ToKebabCase());
        Assert.Empty(empty.ToSnakeCase());
        Assert.Empty(empty.ToPascalCase());
        Assert.Empty(empty.ToCamelCase());
        Assert.Empty(empty.ToDisplayName());
        Assert.Empty(empty.ToUrlSlug());
        Assert.Empty(empty.ToConstantName());
        Assert.Empty(empty.ToConfigName());
        Assert.Empty(empty.ToXmlName());
        Assert.Empty(empty.ToJsonName());
        Assert.Empty(empty.ToConstantName());
        Assert.Empty(empty.ToPosixName());
    }

    [Fact]
    public void AllMethods_HandleSingleCharacter()
    {
        var single = "a";

        Assert.Equal("a", single.ToKebabCase());
        Assert.Equal("a", single.ToSnakeCase());
        Assert.Equal("A", single.ToPascalCase());
        Assert.Equal("a", single.ToCamelCase());
        Assert.Equal("A", single.ToDisplayName());
        Assert.Equal("a", single.ToUrlSlug());
        Assert.Equal("A", single.ToConstantName());
        Assert.Equal("a", single.ToConfigName());
        Assert.Equal("a", single.ToXmlName());
        Assert.Equal("a", single.ToJsonName());
        Assert.Equal("A", single.ToConstantName());
        Assert.Equal("a", single.ToPosixName());
    }

    [Theory]
    [InlineData("my---variable___name")]
    [InlineData("test__value--name")]
    [InlineData("foo___bar---baz")]
    public void HandleConsecutiveDelimiters_CollapsesToSingleDelimiter(string input)
    {
        // Methods that explicitly handle consecutive delimiters
        var urlSlug = input.ToUrlSlug();
        Assert.DoesNotContain("--", urlSlug);

        var configName = input.ToConfigName();
        Assert.DoesNotContain("--", configName);
        Assert.DoesNotContain("__", configName);

        var posixName = input.ToPosixName();
        Assert.DoesNotContain("--", posixName);
        Assert.DoesNotContain("__", posixName);

        var envVar = input.ToConstantName();
        Assert.DoesNotContain("__", envVar);

        var displayName = input.ToDisplayName();
        // DisplayName should not have consecutive spaces
        Assert.DoesNotContain("  ", displayName);
    }

    [Theory]
    [InlineData("HTTP", "http", "Http")]
    [InlineData("XML", "xml", "Xml")]
    [InlineData("API", "api", "Api")]
    public void AllMethods_HandleFullAcronyms(string input, string expectedLower, string expectedCapitalized)
{
        Assert.Equal(expectedLower, input.ToKebabCase());
        Assert.Equal(expectedLower, input.ToSnakeCase());
        Assert.Equal(expectedCapitalized, input.ToPascalCase());
        Assert.Equal(expectedLower, input.ToCamelCase());
        Assert.Equal(expectedCapitalized, input.ToDisplayName());
}

    [Theory]
    [InlineData("myHTTPSConnectionAPI", "my-https-connection-api")]
    [InlineData("parseXMLWithHTTPClient", "parse-xml-with-http-client")]
    public void ToKebabCase_HandlesComplexAcronymPatterns(string input, string expected)
    {
        var result = input.ToKebabCase();
        Assert.Equal(expected, result);
    }

    #endregion
}
