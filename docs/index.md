# InteractiveReadLine

An extensible, composable readline library written in pure C# for creating interactive text-based
interfaces with `System.Console` and other console-like UI components.

Targets .NET Standard 2.0 and .NET 10 and has no external runtime dependencies.

```csharp
var text = ConsoleReadLine.ReadLine();
```

## Documentation

* The **[API Reference](api/InteractiveReadLine.yml)** documents the public API directly from the
  source.
* The **[History](history.md)** article explains the history feature, from basic setup through custom
  update behavior.
* The [repository README](https://github.com/mattj23/InteractiveReadLine) has the full guide,
  including animated examples of each feature.
* Run the repository's [demo program](https://github.com/mattj23/InteractiveReadLine/tree/master/InteractiveReadLine.Demo)
  to try each feature interactively.

## Starting points in the API

| Type | Purpose |
| --- | --- |
| @InteractiveReadLine.ConsoleReadLine | Reads lines from `System.Console` and provides the usual entry point |
| @InteractiveReadLine.ReadLineConfig | Configures keys, formatting, lexing, auto-complete, and history |
| @InteractiveReadLine.ReadLineResult | Distinguishes a finished line from an abandoned line or the end of input |
| @InteractiveReadLine.KeyBehaviors.CommonKeyBehaviors | The ready-made editing behaviors |
| @InteractiveReadLine.KeyBehaviors.BehaviorExtensionMethods | The ready-made key bindings |
| @InteractiveReadLine.Formatting.CommonFormatters | The ready-made display formatters |
| @InteractiveReadLine.Tokenizing.CommonLexers | Ready-made lexers and components for building custom lexers |
| @InteractiveReadLine.IReadLineProvider | Defines a backend for input sources other than the console |

## Installing

```
dotnet add package InteractiveReadLine
```
