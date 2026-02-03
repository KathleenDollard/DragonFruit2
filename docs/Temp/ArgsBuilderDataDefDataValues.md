# Why have three classes to support Args classes?

`ArgsBuilder`: Does the work
`DataDefinition`: Holds the definition of the Args -no data
`DataValues`: Holds the values and does not exist at initialization; it is instantiated when data gathering commences. Also, this is available in the `Result` for debugging after DragonFruit2's entry call returns.

So `DataValues` has a different lifetime.

But, do we need both an `ArgsBuilder` and a `DataDefinition`?