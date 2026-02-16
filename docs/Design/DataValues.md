# Data values

Data values can be retrieved from any source by specifying a `DataProvider`. When more than one `DataProvider` is supplied, the first `DataProvider` to provide a value for a property is used, and the remaining `DataProviders` are ignored for that property. There will often be only one data provider.

This "first one wins" approach reduces work for the system while allowing flexible fallback values. For example, values the user enters via an option or argument is almost always the first data provider (and effort is required to use a different first provider), which could be followed by getting a value from a configuration file, for example.

The `DataValue` type includes both the value and whether it has been set. This is important because the constructor **must** not set the value to the types default if no value has been specified. This is to allow property initialization to work correctly. Property (and field) initialization is done prior to the constructor, does not set the value to something else unless data value's `IsSet` property is `true`.