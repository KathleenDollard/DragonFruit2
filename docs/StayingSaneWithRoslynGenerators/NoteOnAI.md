# Note on AI and Roslyn incremental generators

I’m not saying you can’t create a generator with AI, but in my experience it created terrible generators. That is not really surprising because there are a lot of terrible generators, and generators that whose purpose may be fundamentally different than yours.  And, if you are not familiar with incremental generators, how will you know if it is terrible?

Where AI can help you is in individual steps, especially in how to get specific types of information from Roslyn's `SyntaxNode`s and `SemanticModel`s. You also may benefit from AI in creating the initial prototype, testing, packaging and other steps of creating a generator. Consider both code creating tools like Visual Studio's Copilot and also LLM based web search in a tool like Chrome.