# Steps to sanity

1. Create a working prototype app, ideally write tests for it
    - Design the code that will trigger generation – hopefully via attributes
    - Handcrafted versions can have special names to avoid overwriting (not just .g.cs)
1. Design the metadata classes along with unit & Verify tests
1. Create outputting code using handcrafted metadata with unit & Verify tests
    - This should reproduce the gen’d classes of your prototype
1. Create the metadata extraction steps & unit tests for each (often just 1 step)
1. Create the generator
    - Each method call in the generator is to something you already wrote & have unit tests for
1. Harden:  create a sample app to parallel prototype, write more tests, debug
1. Create a package and handle issues like multi-targeting and marker attributes
1. Debug, iterate, and handle ancillary issues like logging

Iterate through the first three steps until you have a testable unit, an application or library, with passing tests that accomplish your goal.

Iterate through all eight steps as you maintain your application. Keeping the prototype active and making/testing all changes there first will save a massive amount of time during maintenance.
