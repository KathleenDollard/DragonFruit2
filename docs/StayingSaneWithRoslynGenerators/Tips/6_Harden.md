# Tips for hardening

## Parallel sample project

## Debugging

There are two approaches to debugging that you'll use at different times:

- Debug via tests
  - Just set a breakpoint and debug your test
  - If you don't have a test to debug, should you create one?
  - I use this approach at least 80% of the time

- Manual debugging
  - Have an example project to generate that has the issue
  - Set `launchSettings.json` in your generator project to point to that `.csproj` file
  - Set your generator as the startup project
  - Set a breakpoint in your generator
  - F5 (Start debugging)
  - I use this approach when my generator is not producing output to determine which step failed

Generator debugging used to be a pain, it’s now quite friendly!

## Testing generators

Like any project, expect your generators to evolve.

Several kinds of tests are essential for a robust generator.

- Unit test as much as is practical
  - All helper methods, such as string helpers
  - Transformations between metadata classes
- Test metadata creation without the generator
  - Combine small targeted tests with snapshot tests (Verify) on simple and full example
- Test source output during initial implementation
  - Snapshot testing on methods called, use the same simple and full example
  - At some point after generator is operational, drop these tests if they are redundant with end to end testing
- Test source after generator is operation
  - Snapshot test generator output, useful for debugging also (see resources & DragonFruit2)
  - Do not overuse snapshot tests as maintenance is tedious
- Functional tests on what you are generating
  - Create a sample project and test the functionality of the generated code and package creation
  - Perhaps multiple projects (like simple and complex), but overusing snapshots becomes tedious
  - If a library, these can also be sample projects

