namespace DragonFruit2.Generators
{
    /// <summary>
    /// Info for the generator about defaults based on attributes, registered defaults are created at runtime via the Register method
    /// </summary>
    public class DefaultInfo
    {
        /// <summary>
        /// The name of the attribute used
        /// </summary>
        public required string AttributeName { get; init; }

        /// <summary>
        /// Name of the type of the default definition to use
        /// </summary>
        public required string DefaultTypeName { get; init; }

        /// <summary>
        /// The arguments entered for this default attribute
        /// </summary>       
        public required ArgumentInfo[] DefaultArguments { get; init; }

    }

    /// <summary>
    /// Info for the generator about default arguments
    /// </summary>
    public class DefaultArgumentInfo
    {
        /// <summary>
        /// The argument's name
        /// </summary>       
        public required string Name { get; init; }
        /// <summary>
        /// The type of the paramter in the Default class's ctor
        /// </summary>
        public required string DefaultParameterTypeName { get; init; }
        /// <summary>
        /// The type of the argument in the attribute
        /// </summary>     
        public required string AttributeArgumentTypeName { get; init; }
        /// <summary>
        /// The value of the argument, as a string. 
        /// This will appear in generated code, so should be parsable to the correct type. 
        /// Per generator guidelines, this should be checked in an analyzer, not in the genertor.
        /// </summary>        
        public required string Value { get; init; }
    }
}