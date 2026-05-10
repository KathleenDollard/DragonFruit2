# Tips for designing metadata and creating the classes

You may find it convenient to design metadata by building the classes you think you need. 

- Metadata is the model containing data for generator
- Metadata must be cacheable! Record classes are a good choice
- Metadata can be transformed with the final models 1:1 with output files
- Use the language of your domain for naming so you can isolate thinking of the original source from the output code
- Metadata can come from several sources in addition to code
- Ensure your metadata types have deep value equality to support caching
  - Do not use types with reference equality - don't use `class` unless it is a `record class`
  - Do not pass any Roslyn instances beyond the initial provider (*)
  - Test for value equality, otherwise you cannot be sure you got it right
- Where you use a type name include the namespace, possibly as a separate property

(*) Technically, you could alter the `Equals` to support caching, but it’s more work and fragile. I think a terrible idea because dealing with Roslyn `SyntaxTree` and `SemanticModel` instances is an art unto itself and best isolated.

Before going on to outputting code, you'll need to create the metadata classes and have handcrafted instances to mentally validate. It's convenient to go ahead and create `Theory` classes (XUnit) or similar for your testing platform. You'll use this to test code output in the next step. But don't go overboard as it's highly likely you'll iterate to refine your metadata classes.
