# DragonFruit2.Test.Project

The purpose of this project is to provide a project that will be generated to create the code for tests of DragonFruit2behavior. These tests should be for things that can't be tested in unit tests and require the dependency on generation.

While the CLIs defined in this project will use DragonFruit2 features, this project is expected to be too chaotic to serve as an example project, and any experimentation here will break tests. At least one sample project will also be included in the repo, currently these are `SampleConsoleApp`and `SubCommandSampleApp`. 

There are significant dependencies within this project, so care is needed. If all the tests passed when you began making changes in this project, they must all pass when you are done.
