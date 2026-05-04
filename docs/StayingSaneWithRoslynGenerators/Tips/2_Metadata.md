# Tips for designing metadata

- Metadata is the model containing data for generator
- Metadata must be cacheable! Record classes are a good choice
- Metadata can be transformed with the final models 1:1 with output files
- Use the language of your domain so you can isolate thinking of the original source from the output code
- Metadata can come from several sources in addition to code
- Ensure your metadata types have deep value equality to support caching
  - Do not use types with reference equality - don't use `class` unless it is a `record class`
  - Do not pass any Roslyn instances beyond the initial provider (*)
- Where you use a type name include the namespace, possibly as a separate property

(*) Technically, you could alter the `Equals` to support caching, but it’s more work and fragile. I think a terrible idea because dealing with Roslyn `SyntaxTree` and `SemanticModel` instances is an art unto itself and best isolated.
