# Design drawings

## Sequence

`Args` is the programmer defined class that the implementing programmer will use after parsing. 

```mermaid
sequenceDiagram
    actor program as Program.cs
    %%participant Alice@{ "type" : "collections" }
    participant cli as Generated Cli.cs
    participant builder as Builder
    participant rootArgs as RootCommandDef
    participant providers as DataProviders
    participant env as System.Environment
    participant result as Result<TRoot>
    participant activeArgs as Active Args instance

    program->>cli: ParseArgs()
    cli->>cli: CreateBuilder() with RootCommandDef
    cli->>builder: ParseArgs()
    builder->>env: GetCommandLineArgs()
    env->>builder: 
    builder->>providers: Initialize(this, RootCommandDef)
    builder->>result: new Result<TRootArgs>(commandLineArgs)
    result->>builder:
    builder->>providers: GetActiveCommandDef()
    note right of providers: Also sets Result.DataValues
    providers->>builder:
    builder->>providers: SetDataValues(provider, result)
    note right of providers: Uses callback to maintain strong typing
    providers->>builder:
    builder->>builder: CheckRequired()
    builder->>dataValues: CreateInstance()
    note right of dataValues: Creates Args instance
    dataValues->>builder:
    builder->>result: Set result.Args
    builder->>dataValues:
    builder->>dataValue: Validate()
    builder->>cli: return result






    %%rect rgb(191, 223, 255)
    %%note right of Alice: Alice calls John.
    %%Alice->>+John: Hello John, how are you?
    %%rect rgb(200, 150, 255)
    %%Alice->>+John: John, can you hear me?
    %%John-->>-Alice: Hi Alice, I can hear you!
    %%end
    %%John-->>-Alice: I feel great!
    %%end
    %%Alice ->>+ John: Did you want to go to the game tonight?
    %%John -->>- Alice: Yeah! See you there.
```

## Phases

```mermaid
---
config:
  title: Phases
---
flowchart TD
  classDef blocks stroke:#333,stroke-width:2px,stroke-dasharray: 5 5;
  cliEntry[Program calls a DragonFruit2 entry point<br/>ParseArgs, etc.]
  cliEntry ==>findArgsBuilder[Find ArgsBuilder for requested root]
  findArgsBuilder ==> init[Initialize data providers]
  init ==> getActive[Get active ArgsBuilderValues from DataProviders]
  getActive ==> getData[Get data for ArgBuilderValues from DataProviders<br/>including configuration defaults]
  getData ==> getProgramDefaults[Get programmatic defaults<br/>includes dependent defaults]
  getProgramDefaults ==> createArgs[Create Args instance]
  createArgs ==> validate[Validate Args instance]
  validate ==> returnResult[Return result]
```

## `DataProvider` flow

```mermaid
---
config:
  title: DataProviders
---
flowchart TD
  classDef mvp stroke:#333,stroke-width:2px,stroke-dasharray: 5 5;
  Cli("CliDataProvider(*)")
  Cli --> JsonScript("JsonDataProvider(*)"):::mvp
  JsonScript --> Config("Configuration"):::mvp
  Config --> Prompt("PromptForMissingRequired (allows multiple)"):::mvp
  Prompt --> Defaults("Defaults (declared/isolated)"):::mvp
   A@{ shape: comment, label: "(*) These providers can provide `ActiveArgBuilder`s" }
```

## Logic flow

```mermaid
graph TD
  Cli[Cli.ParseArgs]
  Cli --> Builder
  subgraph Builder[Builder.ParseArgs]
  direction TB
  InitCli[InitializeCli]
  InitCli --> GetActive["DataProviders.GetActiveArgsBuilder()"]
  GetActive --> CreateArgsCall["activeArgsBuilder.CreateArgs()"]
  end
  CreateArgsCall --> ArgsBuilder
  subgraph ArgsBuilder[ArgsBuilder.CreateArgs]
  direction TB
  CreateInstance
  CreateInstance --> Validate
  end
```