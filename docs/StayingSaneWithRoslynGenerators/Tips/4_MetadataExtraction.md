# Tips for extracting and transforming metadata

You're designing metadata extraction to work well in the generator that you'll create in the next step. You can extract from multiple sources, although only the SyntaxValueProvider is discussed here. See the [spec](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md#incrementalvaluesprovidert) for others.

The steps to metadata extraction from the `SyntaxValueProvider` are:

- Find the correct `SyntaxNode`s
- Transform to the initial metadata shape
- Optionally, perform further transforms

As you map your metadata to your input code, you might find that you need multiple steps to create the metadata you want for outputting code. You can extract multiple metadata models and combine them, or have them serve different purposes. You can combine metadata classes much more efficiently than you can extract relationships from the semantic model. 

For example, DragonFruit2 needs to all the derived classes for each `CommandClass`. Initially, it did this by searching the SemanticModel in the initial metadata creation, which was inefficient and would have blocked DragonFruit2 from running in design time compilation. This was redesigned to pull all the information need from the `CommandClass` `ClassDeclarationSyntax`, and then create the hierarchy of commands in an additional transform using the base class of the original metadata.

## Tips

- Metadata extraction is done one SyntaxNode at a time
- Map your metadata classes and properties your generator's _input code_, and then consider how you'll find those `SyntaxNode`s
- If information is based on the relationship between classes, including knowing a base class or how a method is resolved, - you will need the `SemanticModel`
- If you have never used Roslyn internals before, this can be an overwhelming step, and may be the most difficult
  - That is why I so strongly encourage writing and testing this code in isolation
  - AI search for what you want to accomplish can be very helpful
  - Techniques are the same as for analyzers, which are better documented
- Use unit tests if you perform additional transforms on your initial metadata
- Use resources such as DragonFruit2 for to create Verify tests
  - Example: creating a compilation with the correct dependencies can be a pain, copy the common techniques