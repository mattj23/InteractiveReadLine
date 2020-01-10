![](https://github.com/mattj23/InteractiveReadLine/workflows/CI%20netcore/badge.svg)
[![Coverage](https://codecov.io/gh/mattj23/InteractiveReadLine/branch/master/graph/badge.svg)](https://codecov.io/gh/mattj23/InteractiveReadLine)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

# Interactive ReadLine
An extensible, composable readline library written in pure C# for creating interactive text-based interfaces with System.Console and other console-like UI components.  Includes customizable key behaviors, formatting, auto-complete, and a navigable history, with many composable pre-made components included.

Targets .NET Standard 2.0 and has no external dependencies.

___

## Overview
This library provides a GNU Readline-like functionality for interactive C# programs that use either the `System.Console` or (in the future) a similar console-like UI component. 

```csharp
var text = ConsoleReadLine.ReadLine();
```

However, practically every aspect of the system's behavior is configurable.  Configuration falls into the following categories:
* **Key Behaviors**: *determine what happens when a key is emitted from the console, allow for all of the standard behaviors, as well as custom behaviors, to be mapped to key information.*

* **Formatters**: *allows text (in raw form or tokenized) to be intercepted before display and formatted with a prefix/suffix and foreground/background colors. In the simplest form this might simply be display a static prompt, in the most complex it could be active validation with syntax highlighting and a live help tip.*

* **Lexers**: *provide for the splitting of text into tokens. A simple, composable regex-based lexer is provided, but the system was structured to hopefully allow integration of ANTLR style lexers without excessive pain. Lexers form the basis for the autocomplete mechanism.*

* **Autocomplete**: *a way to provide suggestions for the token under the cursor which the system can cycle through, allowing you to provide context aware assistance to the user.*

All of these above configurations are done by providing `Action<..>` and `Func<..>` style delegates to a configuration object, rather than use a zoo of custom interfaces.

```csharp
var config = ReadLineConfig.Empty
    .AddStandardKeys()
    .AddTabAutoComplete()
    .AddKeyBehavior('?', CommonKeyBehaviors.WriteMessageFromTokens(WriteHelp)) 
    .SetFormatter(CommonFormatters.FixedPrompt("prompt > "))
    .SetAutoCompletion(AutoComplete)
    .SetLexer(CommonLexers.SplitOnWhitespace);

var text = ConsoleReadline.ReadLine(config);
```

## Code Examples
Currently, the library only works with a provider written to wrap the `System.Console` object. However, a provider only needs to implement three methods which consist of displaying text and reading keyboard input in order to be a usable backend (see the `IReadline` interface), so it should be straightforward to write a provider for a WinForms or WPF text box, a console in a game engine, or similar.

The most trivial version of reading a line of input from the console is:

```csharp
var text = ConsoleReadLine.ReadLine();
```
This will produce a prompt-less console input that allows for basic typing, delete, backspace, and arrow keys.

This simple example hides the fact that a handler object (`InputHandler`) is created behind the scenes in the `ConsoleReadLine.ReadLine()` method.  The `InputHandler` receives both an `IReadline` provider and a `ReadLineConfig` configuration object in its constructor. If no configuration object is provided, it creates a basic configuration (`ReadLineConfig.Basic`) behind the scenes which provides character insertion, enter, and the basic editing keys.

We can create a configuration object manually to pass in to the provider.

```csharp
var config = ReadLineConfig.Basic
    .AddKeyBehavior(ConsoleKey.DownArrow, CommonKeyBehaviors.ClearAll);

var text = ConsoleReadLine.ReadLine(config);
```
In this case we've added a new key behavior: when the down arrow key is pressed we clear the entire line of text.  This takes advantage of a pre-written behavior (the `ClearAll` method), but we could just as easily write our own.

```csharp
var config = ReadLineConfig.Basic
    .AddKeyBehavior(ConsoleKey.DownArrow, target =>
    {
        target.TextBuffer.Clear();
        target.TextBuffer.Append("I have replaced all of your text");
        target.CursorPosition = target.TextBuffer.Length;
    });

var text = ConsoleReadLine.ReadLine(config);
```
In this case, whenever the user presses the down arrow key, their text is replaced with our unhelpful message. See the [section on Key Behaviors](#key-behaviors) for more information on how they work.

Formatters are another easy example of configuration.  Formatters intercept the handler object's request to display the text and allow you to make changes to it. We can trivially use them to do things like display a prompt, or hide password characters.

```csharp
var config = ReadLineConfig.Basic
    .SetFormatter(CommonFormatters.FixedPrompt("enter text here > "))

var text = ConsoleReadLine.ReadLine(config);
```
The code above gives us a fixed prompt that appears in front of our text input.

```csharp
var config = ReadLineConfig.Basic
    .SetFormatter(CommonFormatters.PasswordBar);

var text = ConsoleReadLine.ReadLine(config);
```
There are three built in formatters intended for hiding passwords while they're being typed.  The first, `PasswordBlank`, simply displays an empty string instead of the input text.  The second `PasswordStars` replaces the password characters with stars.  The one above, `PasswordBar`, displays a bar based on the SHA256 hash of the input text.  The bar moves around as you type, revealing no information about your password or its length, but it will always produce the same visual effect for the same input text.

```csharp
var config = ReadLineConfig.Basic
    .SetFormatter(CommonFormatters.PasswordBar.WithFixedPrompt("Enter password: "));

var text = ConsoleReadLine.ReadLine(config);
```
Finally, formatters can be composed together as well. The above code produces a fixed prompt in front of the password display, and works with all of the password formatters.  See the [section on Formatters](#formatters) for more information on how they work and what can be done with them.


## Design Philosopy

### Obviousness and Correctness
The design of this library's API was based on an attempt to do two things:
1. Have one obviously correct way of doing each thing
2. Construct the API in such a way that it's difficult to produce invalid data
3. Have as few 'special' or hard-coded features as possible, rather use the same customization mechanisms to implement even the basic functionality

### Delegates instead of Objects
For the most part, this library makes every attempt to avoid creating a huge taxonomy of objects and interfaces for all of the pluggable components.  Because the various actions (like key behaviors, tokenizing, formatting, etc) are simple and have minimal inputs and outputs, this library instead favors the use of delegates. 

This approach was selected for the following benefits:
1. It allows for quicker, easier composition of code, especially via lambdas where possible
1. It discourages the preservation of state in mechanisms that should primarily exist to perform actions or transformations, but does not preclude an object from exposing a method that can be used instead while still maintaining access to the object state

### Documentation
A large part of the motivation for creating this library was my frustration with a dearth of documented alternatives.  In the absence of good documentation, it's often true that it takes less time to re-implement a piece of software than it does to understand how the original author intended it to work.

I've made it a priority to provide as many code examples as possible for all of the different library features, as well as heavily comment the internals of the library for anyone who finds themselves having to work with its guts.

### Testing
In conjunction with documentation, unit testing is a priority to ensure that the code works as intended even through changes.  Most of the complex parts of the library were developed directly through test writing, and my goal has been to cover all of the internal machinery of the library (the readline handler, the console provider, the regex lexing engine, and the token/sequence) with tests immediately.  The built-in behaviors, like the various key and formatting behaviors, will be covered by tests as time permits.

## Code Documentation

### Key Behaviors

### Formatters

### Lexers 

### Auto-Complete