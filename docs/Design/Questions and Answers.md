# Q & A

_Feel free to add questions, even if you cannot answer them_

## Why is the default for properties an option, not an argument?

Because C# does not give meaning to the order of properties in a file, DragonFruit2 does not. This means that arguments must specify their position, and, eventually, it must be unique.

For this reason, arguments always need an attribute, and options have not similar requirement. 

## Why are subcommands derived classes?

While this may seem odd at first glance, in a well defined CLI, each subcommand specializes or narrows its preceding command. For example, `dotnet` introduces you are in the `dotnet` CLI or ecosystem, `reference` narrows this to project references, and `add` further narrows this to the process of adding: 

```dotnetcli
dotnet reference add <myproject>
```

From a practical perspective there are two significant benefits:

- It reduces the number of attributes or special knowledge needed to build a CLI
- Only leaf nodes are ever invoked/created as a parse result and making non-invokable classes abstract is natural
- Any options or arguments that apply to parent commands apply to the leaf and this makes their values naturally available
- `recursive`, which has sometimes challenged folks, is strictly an aspect of System.CommandLine and based on an attribute. It's unrelated to the Args data.
- When working with a result it can be typed to the parent and a `switch` used to determine which subcommand was executed.