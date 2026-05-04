# Overall tips

- Your generator can target any version of C# and .NET
- Your generator must be implemented with .NET 4.8 (full framework)
  - The version of C# is up to you: 7.3 is supported, most folks use latest
- The application must ref the generator & you may need a `Common` project for shared features such as marker attributes
- Your generator must be fast to run in design time build
  - It will affect IDE performance for all editing operations just by adding your package
  - The level of impact will increase with user’s project/solution size
  - You are unlikely to find performance issues during testing
  - Alternatively, run in the build phase
- Generators should not generally report issues – pair with analyzers
- Can’t access compiled version of code you’re compiling, such as attributes
- Refer to Andrew Locke’s posts for details that I can’t fit into this talk
  - Debugging, caching, logging, marker attributes, interceptors, etc.
- Web search for how to do things in Roslyn has been great  
  - Generating generators has gone badly as there are insufficient good generators

