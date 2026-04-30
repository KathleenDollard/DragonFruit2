# Tips for creating a generator

In the generator class’s Initialize method you a chain of providers that are used as needed during generation.

- Use generator class only for flow
- Separately create and test all called methods
- Clarify IncrementalValueProvider/IncrementalValuesProvider with variable naming
- You can use multiple input sources, different code, compilation, etc.
  - The spec is the best source for transformation operations
- The SyntaxProvider `Transform` always runs, keep it simple
- Do not report errors in user code, use a parallel analyzer