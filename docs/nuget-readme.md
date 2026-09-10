# Interactive ReadLine

An extensible, composable readline library written in pure C# for creating interactive text-based
interfaces with `System.Console` and other console-like UI components. Includes customizable key
behaviors, formatting, auto-complete, navigable history, and many composable ready-made components.

Targets .NET Standard 2.0 and .NET 10 and has no external runtime dependencies.

> The [project page on GitHub](https://github.com/mattj23/InteractiveReadLine) has the full
> documentation, including animated examples of each feature and a runnable demo program.

## Getting Started

The simplest way to read a line of input is:

```csharp
var text = ConsoleReadLine.ReadLine();
```

This creates a promptless input with typing, Delete, Backspace, the arrow keys, and Ctrl+C to abandon
the line. Practically every aspect of the behavior is configurable through a `ReadLineConfig`:

```csharp
var config = ReadLineConfig.Empty
    .AddStandardKeys()
    .AddCtrlNavKeys()
    .AddTabAutoComplete()
    .SetLexer(CommonLexers.SplitOnWhitespace)
    .SetAutoCompletion(tokens => Suggestions(tokens))
    .SetFormatter(CommonFormatters.FixedPrompt("> "));

var text = ConsoleReadLine.ReadLine(config);
```

Configuration uses delegates such as `Action<T>` and `Func<T, TResult>` and falls into five categories:

* **Key behaviors** determine what happens when a key is pressed, covering both the standard editing
  commands and any custom behavior you bind.
* **Formatters** intercept the text on its way to the display so it can be given a prompt, a suffix,
  and per-character colors. They never alter the text the user is editing.
* **Lexers** split the line into tokens used by auto-complete and token-based formatters. A composable
  regular-expression lexer is included.
* **Auto-complete** provides suggestions for the token under the cursor. The user cycles through the
  suggestions with Tab.
* **History** supplies previously entered lines for the user to navigate with the arrow keys.

## Ending a Line

A user can finish a line with Enter, abandon it with Ctrl+C, or signal that there is no more input
with Ctrl+D on an empty line. `ReadLine()` converts these outcomes to the entered text, an empty
string, and `null`, respectively. An ordinary read loop therefore terminates on Ctrl+D:

```csharp
string? line;
while ((line = ConsoleReadLine.ReadLine(config)) != null)
{
    Execute(line);
}
```

Use `Read()` when you must distinguish an abandoned line from an entered empty line. It returns a
`ReadLineResult` containing the text and information about how the interaction ended.

Note that while a line is being read, Ctrl+C abandons the line rather than terminating the process.
The previous console behavior is restored when the read finishes.

## Canceling and Async

Every read method accepts a `CancellationToken` and has an asynchronous counterpart:

```csharp
var line = await ConsoleReadLine.ReadLineAsync(config, cancellationToken);
```

Canceling through the token raises an `OperationCanceledException`. A user abandoning a line is
ordinary input and produces a result. The asynchronous versions wait for a keypress without
occupying a thread, allowing other work to continue while the prompt remains open.

## Requirements

Reading individual keypresses requires an interactive console. The `ConsoleReadLine` constructor
throws an `InvalidOperationException` when standard input is redirected from a pipe or file. Use
`Console.ReadLine()` for redirected input or supply your own `IReadLineProvider`.

## License

MIT. See the [repository](https://github.com/mattj23/InteractiveReadLine) for the full text.
