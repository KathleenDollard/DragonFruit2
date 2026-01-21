# TEsting Strategy

Several kinds of testing are needed for DragonFruit2:

- Unit testing of individual methods used to support code generation
- Reading and evaluating the input source to create the interim data for code generation (`CommandInfo`)
  - All attributes, impactful keywords, and key methods
  - Use Verify
- Generating code from `CommandInfo`
  - All variations in the smallest, best named tests (many)
  - Use Verify
- Integration tests based on several test projects
  - Several, not extensive as just generating correctly is the goal
  - Ideally, this would be done by a package created during testing
- Unit tests of the behavior of projects
  - Probably using projects created during integration testing, although this might introduce an excess of verification