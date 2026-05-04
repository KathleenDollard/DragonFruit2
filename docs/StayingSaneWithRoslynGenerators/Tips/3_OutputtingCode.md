# Tips for outputting code

- You’ll change these files often
- They combine the logic of outputting with the logic of the output code
  - Searchability is hard and key to reducing frustration
  - Getting details like matching curly brackets can be make you a bit crazy
- Organize outputting code so it is simple to find the output of a particular thing

There are two approaches I would recommend:

- Raw string literals
  - Benefit: Great for simple code when indenting is not an issue
  - Drawback: Can become convoluted for not trivial scenarios
- Custom StringBuilder wrapper
  - Benefits: Automates code details and removes interleaved code
  - Drawback: Not standardized

A custom string builder is my preferred approach. I have written a _lot_ of generators over the last 25 years and this is the only approach that allows me to find what generates a particular block of code easily and (most importantly) spend almost no time matching open/close constructs (curly brackets).

While you will get more matches if you are searching for code with raw string literals, in non-trivial source output, you can never rely on searching for code as it is often broken up. I prefer to rely on navigation to the correct location rather than searching.

Let me know what you think of that approach in the [discussion](https://github.com/KathleenDollard/DragonFruit2/discussions/categories/roslyn-incremental-source-generators). If other folks like it I may release it as a separate library, especially if anyone is interested in helping to maintain it.
