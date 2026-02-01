using System.Text;

namespace DragonFruit2;

public static class StringExtensions
{
    /// <summary>
    /// Converts a Pascal case or camel case string to kebab case.
    /// </summary>
    /// <param name="input">The input string in Pascal or camel case.</param>
    /// <returns>The string converted to kebab case (lowercase with hyphens).</returns>
    /// <example>
    /// "MyString".ToKebabCase() returns "my-string"
    /// "myString".ToKebabCase() returns "my-string"
    /// "HTTPServer".ToKebabCase() returns "h-t-t-p-server"
    /// </example>
    public static string ToKebabCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                if (i > 0)
                    result.Append('-');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a Pascal case or camel case string to snake case.
    /// </summary>
    /// <param name="input">The input string in Pascal or camel case.</param>
    /// <returns>The string converted to snake case (lowercase with underscores).</returns>
    /// <example>
    /// "MyString".ToSnakeCase() returns "my_string"
    /// "myString".ToSnakeCase() returns "my_string"
    /// "HTTPServer".ToSnakeCase() returns "h_t_t_p_server"
    /// </example>
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsUpper(input[i]))
            {
                if (i > 0)
                    result.Append('_');
                result.Append(char.ToLower(input[i]));
            }
            else
            {
                result.Append(input[i]);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Converts a string to POSIX-compliant portable filename format.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <returns>The string converted to POSIX format (lowercase with hyphens, alphanumeric and safe characters only).</returns>
    /// <remarks>
    /// POSIX portable filenames use only: a-z, A-Z, 0-9, ., -, _
    /// This method converts to lowercase, removes unsafe characters, and uses hyphens as separators.
    /// </remarks>
    /// <example>
    /// "MyFileName".ToPosixName() returns "my-file-name"
    /// "config_file.txt".ToPosixName() returns "config-file.txt"
    /// "HTTP Server 2.0".ToPosixName() returns "http-server-2-0"
    /// </example>
    public static string ToPosixName(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // First, use ToKebabCase to handle camelCase/PascalCase conversions
        var kebabized = input.ToKebabCase();

        var result = new StringBuilder();
        bool lastWasDelimiter = false;

        for (int i = 0; i < kebabized.Length; i++)
        {
            char c = kebabized[i];

            if (char.IsLetterOrDigit(c))
            {
                result.Append(c);
                lastWasDelimiter = false;
            }
            else if (c == '.' || c == '_' || c == '-')
            {
                // Preserve dots, underscores, and hyphens
                if (!lastWasDelimiter)
                {
                    result.Append(c);
                    lastWasDelimiter = true;
                }
            }
            else if (char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c))
            {
                // Replace other special characters with hyphens, avoiding consecutive delimiters
                if (!lastWasDelimiter && result.Length > 0)
                {
                    result.Append('-');
                    lastWasDelimiter = true;
                }
            }
            // Skip all other characters
        }

        // Remove trailing hyphens/underscores
        return result.ToString().TrimEnd('-', '_');
    }

    /// <summary>
    /// Converts a string to valid XML element name format.
    /// </summary>
    /// <param name="input">The input string in any format.</param>
    /// <returns>The string converted to XML name format (camelCase, alphanumeric with hyphens, underscores, and periods).</returns>
    /// <remarks>
    /// XML names must start with a letter or underscore and can contain letters, digits, hyphens, underscores, periods, and colons.
    /// Names starting with "xml" (case-insensitive) are reserved and will be prefixed with an underscore.
    /// This method converts to camelCase and removes invalid characters for compatibility.
    /// </remarks>
    /// <example>
    /// "MyFileName".ToXmlName() returns "myFileName"
    /// "my file name".ToXmlName() returns "myFileName"
    /// "config-file".ToXmlName() returns "configFile"
    /// "XML_Config".ToXmlName() returns "_xmlConfig"
    /// </example>
    public static string ToXmlName(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();
        bool nextCharIsUpper = false;
        bool isFirstChar = true;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (char.IsLetter(c))
            {
                if (isFirstChar)
                {
                    result.Append(char.ToLower(c));
                    isFirstChar = false;
                }
                else if (nextCharIsUpper)
                {
                    result.Append(char.ToUpper(c));
                    nextCharIsUpper = false;
                }
                else
                {
                    result.Append(char.ToLower(c));
                }
            }
            else if (char.IsDigit(c))
            {
                result.Append(c);
                isFirstChar = false;
                nextCharIsUpper = false;
            }
            else if (c == '.' || c == '_' || c == '-' || c == ':')
            {
                // Preserve valid XML delimiters
                result.Append(c);
                nextCharIsUpper = true; // Capitalize next letter after delimiter
            }
            else if (char.IsWhiteSpace(c))
            {
                // Convert spaces to camelCase transition
                nextCharIsUpper = true;
            }
            // Skip all other characters
        }

        // Remove trailing delimiters
        var final = result.ToString().TrimEnd('.', '_', '-', ':');

        // Ensure it starts with a letter or underscore (not a digit)
        if (final.Length > 0 && char.IsDigit(final[0]))
            final = "_" + final;

        // XML names cannot start with "xml" (case-insensitive) as they are reserved
        if (final.Length >= 3 && final.Substring(0, 3).Equals("xml", System.StringComparison.OrdinalIgnoreCase))
            final = "_" + final;

        return final;
    }

    /// <summary>
    /// Converts a string to valid JSON property name format.
    /// </summary>
    /// <param name="input">The input string in any format.</param>
    /// <returns>The string converted to JSON name format (camelCase, valid identifier characters only).</returns>
    /// <remarks>
    /// JSON property names should use camelCase for JavaScript interoperability.
    /// Valid characters are letters, digits, underscores (_), and dollar signs ($).
    /// Names must start with a letter, underscore, or dollar sign.
    /// </remarks>
    /// <example>
    /// "MyProperty".ToJsonName() returns "myProperty"
    /// "my property name".ToJsonName() returns "myPropertyName"
    /// "config-file".ToJsonName() returns "configFile"
    /// "2ndValue".ToJsonName() returns "_2ndValue"
    /// </example>
    public static string ToJsonName(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();
        bool nextCharIsUpper = false;
        bool isFirstChar = true;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (char.IsLetter(c))
            {
                if (isFirstChar)
                {
                    result.Append(char.ToLower(c));
                    isFirstChar = false;
                }
                else if (nextCharIsUpper)
                {
                    result.Append(char.ToUpper(c));
                    nextCharIsUpper = false;
                }
                else
                {
                    result.Append(char.ToLower(c));
                }
            }
            else if (char.IsDigit(c))
            {
                result.Append(c);
                isFirstChar = false;
                nextCharIsUpper = false;
            }
            else if (c == '_' || c == '$')
            {
                // Preserve underscores and dollar signs (valid JSON identifier characters)
                result.Append(c);
                nextCharIsUpper = true; // Capitalize next letter after delimiter
            }
            else if (c == '-' || c == '.' || char.IsWhiteSpace(c))
            {
                // Convert common delimiters to camelCase transition
                nextCharIsUpper = true;
            }
            // Skip all other characters
        }

        var final = result.ToString();

        // Ensure it starts with a letter, underscore, or dollar sign (not a digit)
        if (final.Length > 0 && char.IsDigit(final[0]))
            final = "_" + final;

        return final;
    }

    /// <summary>
    /// Converts a string to valid environment variable name format.
    /// </summary>
    /// <param name="input">The input string in any format.</param>
    /// <returns>The string converted to environment variable format (UPPERCASE with underscores).</returns>
    /// <remarks>
    /// Environment variable names should use UPPERCASE with underscores (screaming snake case).
    /// Valid characters are letters, digits, and underscores.
    /// Names must start with a letter or underscore and cannot start with a digit.
    /// This format is portable across most operating systems and shells.
    /// </remarks>
    /// <example>
    /// "MyVariable".ToEnvironmentVariableName() returns "MY_VARIABLE"
    /// "myVarName".ToEnvironmentVariableName() returns "MY_VAR_NAME"
    /// "config-file-path".ToEnvironmentVariableName() returns "CONFIG_FILE_PATH"
    /// "2ndValue".ToEnvironmentVariableName() returns "_2ND_VALUE"
    /// </example>
    public static string ToEnvironmentVariableName(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // First, use ToSnakeCase to handle camelCase/PascalCase conversions
        var snakized = input.ToSnakeCase();

        var result = new StringBuilder();
        bool lastWasDelimiter = false;

        for (int i = 0; i < snakized.Length; i++)
        {
            char c = snakized[i];

            if (char.IsLetterOrDigit(c))
            {
                result.Append(char.ToUpper(c));
                lastWasDelimiter = false;
            }
            else if (c == '_')
            {
                // Preserve underscores
                if (!lastWasDelimiter)
                {
                    result.Append(c);
                    lastWasDelimiter = true;
                }
            }
            else if (char.IsWhiteSpace(c) || c == '-' || char.IsSymbol(c) || char.IsPunctuation(c))
            {
                // Replace other special characters with underscores, avoiding consecutive delimiters
                if (!lastWasDelimiter && result.Length > 0)
                {
                    result.Append('_');
                    lastWasDelimiter = true;
                }
            }
            // Skip all other characters
        }

        var final = result.ToString().TrimEnd('_');

        // Ensure it starts with a letter or underscore (not a digit)
        if (final.Length > 0 && char.IsDigit(final[0]))
            final = "_" + final;

        return final;
    }

        /// <summary>
        /// Converts a string to valid configuration file name format.
        /// </summary>
        /// <param name="input">The input string in any format.</param>
        /// <returns>The string converted to configuration format (lowercase with hyphens or dots).</returns>
        /// <remarks>
        /// Configuration file names typically use lowercase with hyphens or dots as separators.
        /// Valid characters are letters, digits, hyphens, dots, and underscores.
        /// This format is commonly used in application settings files (appsettings.json, app.config, etc.).
        /// Consecutive delimiters are collapsed to avoid malformed names.
        /// </remarks>
        /// <example>
        /// "AppSettings".ToConfigName() returns "app-settings"
        /// "DatabaseConnectionString".ToConfigName() returns "database-connection-string"
        /// "log_level_setting".ToConfigName() returns "log-level-setting"
        /// "MyApp.Config".ToConfigName() returns "my-app.config"
        /// "2ndLevelConfig".ToConfigName() returns "2nd-level-config"
        /// </example>
        public static string ToConfigName(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // First, use ToKebabCase to handle camelCase/PascalCase conversions
            var kebabized = input.ToKebabCase();

            var result = new StringBuilder();
            bool lastWasDelimiter = false;

            for (int i = 0; i < kebabized.Length; i++)
            {
                char c = kebabized[i];

                if (char.IsLetterOrDigit(c))
                {
                    result.Append(c);
                    lastWasDelimiter = false;
                }
                else if (c == '.' || c == '_' || c == '-')
                {
                    // Preserve dots, underscores, and hyphens (common config delimiters)
                    if (!lastWasDelimiter)
                    {
                        result.Append(c);
                        lastWasDelimiter = true;
                    }
                }
                else if (char.IsWhiteSpace(c) || char.IsSymbol(c) || char.IsPunctuation(c))
                {
                    // Replace other special characters with hyphens, avoiding consecutive delimiters
                    if (!lastWasDelimiter && result.Length > 0)
                    {
                        result.Append('-');
                        lastWasDelimiter = true;
                    }
                }
                // Skip all other characters
            }

            // Remove trailing delimiters
            var final = result.ToString().TrimEnd('-', '_', '.');

                        return final;
                    }

                /// <summary>
                /// Converts a string to Pascal case (PascalCase) format.
                /// </summary>
                /// <param name="input">The input string in any format.</param>
                /// <returns>The string converted to Pascal case (uppercase first letter, then camelCase).</returns>
                /// <remarks>
                /// Pascal case is the C# standard for class names, method names, and public properties.
                /// Each word starts with an uppercase letter, with no separators.
                /// </remarks>
                /// <example>
                /// "my-variable".ToPascalCase() returns "MyVariable"
                /// "my_variable_name".ToPascalCase() returns "MyVariableName"
                /// "myVariableName".ToPascalCase() returns "MyVariableName"
                /// "HTTPServer".ToPascalCase() returns "HttpServer"
                /// </example>
                public static string ToPascalCase(this string input)
                {
                    if (string.IsNullOrEmpty(input))
                        return input;

                    var result = new StringBuilder();
                    bool nextCharIsUpper = true;

                    for (int i = 0; i < input.Length; i++)
                    {
                        char c = input[i];

                        if (char.IsLetter(c))
                        {
                            if (nextCharIsUpper)
                            {
                                result.Append(char.ToUpper(c));
                                nextCharIsUpper = false;
                            }
                            else
                            {
                                result.Append(char.ToLower(c));
                            }
                        }
                        else if (char.IsDigit(c))
                        {
                            result.Append(c);
                            nextCharIsUpper = false;
                        }
                        else if (c == '_' || c == '-' || c == '.' || char.IsWhiteSpace(c))
                        {
                            // Delimiters trigger uppercase for next letter
                            nextCharIsUpper = true;
                        }
                        // Skip all other characters
                    }

                    return result.ToString();
                }

                /// <summary>
                /// Converts a string to camel case format.
                /// </summary>
                /// <param name="input">The input string in any format.</param>
                /// <returns>The string converted to camel case (lowercase first letter, then PascalCase).</returns>
                /// <remarks>
                /// Camel case is used for C# local variables, method parameters, and private fields.
                /// The first letter is lowercase, with subsequent words starting with uppercase.
                /// </remarks>
                /// <example>
                /// "my-variable".ToCamelCase() returns "myVariable"
                /// "my_variable_name".ToCamelCase() returns "myVariableName"
                /// "MyVariableName".ToCamelCase() returns "myVariableName"
                /// "HTTPServer".ToCamelCase() returns "httpServer"
                /// </example>
                public static string ToCamelCase(this string input)
                {
                    if (string.IsNullOrEmpty(input))
                        return input;

                    var result = new StringBuilder();
                    bool nextCharIsUpper = false;
                    bool isFirstChar = true;

                    for (int i = 0; i < input.Length; i++)
                    {
                        char c = input[i];

                        if (char.IsLetter(c))
                        {
                            if (isFirstChar)
                            {
                                result.Append(char.ToLower(c));
                                isFirstChar = false;
                            }
                            else if (nextCharIsUpper)
                            {
                                result.Append(char.ToUpper(c));
                                nextCharIsUpper = false;
                            }
                            else
                            {
                                result.Append(char.ToLower(c));
                            }
                        }
                        else if (char.IsDigit(c))
                        {
                            result.Append(c);
                            isFirstChar = false;
                            nextCharIsUpper = false;
                        }
                        else if (c == '_' || c == '-' || c == '.' || char.IsWhiteSpace(c))
                        {
                            // Delimiters trigger uppercase for next letter
                            nextCharIsUpper = true;
                        }
                        // Skip all other characters
                    }

                    return result.ToString();
                }

                /// <summary>
                /// Converts a string to human-readable display name format.
                /// </summary>
                /// <param name="input">The input string in any format.</param>
                /// <returns>The string converted to title case with spaces (e.g., "My Variable Name").</returns>
                /// <remarks>
                /// Display names are used for UI labels, form headers, and human-readable output.
                /// Each word is capitalized and separated by spaces. Special characters are removed.
                /// </remarks>
                /// <example>
                /// "myVariableName".ToDisplayName() returns "My Variable Name"
                /// "my_variable_name".ToDisplayName() returns "My Variable Name"
                /// "my-variable-name".ToDisplayName() returns "My Variable Name"
                /// "HTTPServer".ToDisplayName() returns "H T T P Server"
                /// </example>
                public static string ToDisplayName(this string input)
                {
                    if (string.IsNullOrEmpty(input))
                        return input;

                    var result = new StringBuilder();
                    bool nextCharIsUpper = true;
                    bool lastWasDelimiter = false;

                    for (int i = 0; i < input.Length; i++)
                    {
                        char c = input[i];

                        if (char.IsLetter(c))
                        {
                            if (lastWasDelimiter && result.Length > 0)
                            {
                                result.Append(' ');
                            }

                            if (nextCharIsUpper)
                            {
                                result.Append(char.ToUpper(c));
                                nextCharIsUpper = false;
                            }
                            else
                            {
                                result.Append(char.ToLower(c));
                            }
                            lastWasDelimiter = false;
                        }
                        else if (char.IsDigit(c))
                        {
                            if (lastWasDelimiter && result.Length > 0)
                            {
                                result.Append(' ');
                            }
                            result.Append(c);
                            lastWasDelimiter = false;
                        }
                        else if (c == '_' || c == '-' || char.IsWhiteSpace(c))
                        {
                            // Delimiters trigger space and uppercase
                            nextCharIsUpper = true;
                            lastWasDelimiter = true;
                        }
                        // Skip all other characters
                    }

                    return result.ToString();
                }

                /// <summary>
                /// Converts a string to URL-safe slug format.
                /// </summary>
                /// <param name="input">The input string.</param>
                /// <returns>The string converted to URL slug format (lowercase with hyphens, URL-safe).</returns>
                /// <remarks>
                /// URL slugs are used in web routes and are SEO-friendly. Only lowercase alphanumeric characters and hyphens are preserved.
                /// Consecutive hyphens are collapsed, and trailing hyphens are removed.
                /// </remarks>
                /// <example>
                /// "My Blog Post Title".ToUrlSlug() returns "my-blog-post-title"
                /// "Article_Title!".ToUrlSlug() returns "article-title"
                /// "HTTPServer 2.0".ToUrlSlug() returns "http-server-2-0"
                /// "hello-world".ToUrlSlug() returns "hello-world"
                /// </example>
                public static string ToUrlSlug(this string input)
                {
                    if (string.IsNullOrEmpty(input))
                        return input;

                    var result = new StringBuilder();
                    bool lastWasDelimiter = false;

                    for (int i = 0; i < input.Length; i++)
                    {
                        char c = input[i];

                        if (char.IsLetterOrDigit(c))
                        {
                            result.Append(char.ToLower(c));
                            lastWasDelimiter = false;
                        }
                        else if (char.IsWhiteSpace(c) || c == '_' || c == '-' || char.IsSymbol(c) || char.IsPunctuation(c))
                        {
                            // Replace all delimiters and special characters with hyphens
                            if (!lastWasDelimiter && result.Length > 0)
                            {
                                result.Append('-');
                                lastWasDelimiter = true;
                            }
                        }
                        // Skip all other characters
                    }

                    // Remove trailing hyphens
                    return result.ToString().TrimEnd('-');
                }

                /// <summary>
                /// Converts a string to constant name format (SCREAMING_SNAKE_CASE).
                /// </summary>
                /// <param name="input">The input string in any format.</param>
                /// <returns>The string converted to constant format (UPPERCASE with underscores).</returns>
                /// <remarks>
                /// Constant names in C# are conventionally uppercase with underscores (SCREAMING_SNAKE_CASE).
                /// This is equivalent to environment variable naming but emphasizes C# constant intent.
                /// </remarks>
                /// <example>
                /// "MyConstant".ToConstant() returns "MY_CONSTANT"
                /// "myConstantValue".ToConstant() returns "MY_CONSTANT_VALUE"
                /// "max-timeout".ToConstant() returns "MAX_TIMEOUT"
                /// "2ndValue".ToConstant() returns "_2ND_VALUE"
                /// </example>
                public static string ToConstant(this string input)
                {
                    // ToConstant is an alias for ToEnvironmentVariableName with the same implementation
                    return input.ToEnvironmentVariableName();
                }
            }
