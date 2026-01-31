# Target complexity for DragonFruit2

The spectrum of complexity targeted by DragonFruit2 is from people new to .NET with consideration of the complexity of the .NET CLI.

Whether DragonFruit2 is ever used for the .NET CLI is up to the team. However, it benefits DragonFruit2 to consider its complexity. This was the approach of System.CommandLine design, which was developed independently and later incorporated as the .NET CLI parser. The challenges of the .NET CLI include a good deal of legacy, multiple teams, some dynamic components, defaults that depend on the environment, symbols that appear on different commands that should remain fully consistent, `Hidden` items to support legacy user scripts for obsolete syntax, etc. In addition, it provides a framework for people considering migrating complex multi-tiered CLIs.

Tracking the .NET CLI is intended to inspire design to be flexible in the right places.