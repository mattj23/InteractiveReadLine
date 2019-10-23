# Interactive Readline
An extensible, composable readline library written in pure C# for creating interactive text-base interfaces with System.Console and other console-like UI components.

Targets .NET Standard 2.0 and has no external dependencies.

___

## Overview
This library provides a GNU Readline-like functionality for interactive C# programs that use either the `System.Console` or (in the future) a similar console-like UI component. 

```csharp
var config = ReadLineConfig.Empty();
var text = ConsoleReadline.ReadLine(config);
```

However, practically every aspect of the system's behavior is configurable.  Configuration falls into the following categories:
* **Key Behaviors**: *determine what happens when a key is emitted from the console, allow for all of the standard behaviors, as well as custom behaviors, to be mapped to key information.*

* **Formatters**: *allows text (in raw form or tokenized) to be intercepted before display and formatted with a prefix/suffix and foreground/background colors. In the simplest form this might simply be display a static prompt, in the most complex it could be active validation with syntax highlighting and a live help tip.*

* **Lexers**: *provide for the splitting of text into tokens. A simple, composable regex-based lexer is provided, but the system was structured to hopefully allow integration of ANTLR style lexers without excessive pain. Lexers form the basis for the autocomplete mechanism.*

* **Autocomplete**: *a way to provide suggestions for the token under the cursor which the system can cycle through, allowing you to provide context aware assistance to the user.*

All of these above configurations are done by providing `Action<..>` and `Func<..>` style delegates to a configuration object, rather than use a zoo of custom interfaces.

```csharp
var config = ReadLineConfig.Empty()
    .AddStandardKeys()
    .AddTabAutoComplete()
    .AddKeyBehavior('?', CommonBehaviors.WriteMessageFromTokens(WriteHelp)) 
    .SetFormatter(CommonFormatters.FixedPrompt("prompt > "))
    .SetAutoCompletion(AutoComplete)
    .SetLexer(CommonLexers.SplitOnWhitespace);

var text = ConsoleReadline.ReadLine(config);
```

## Code Samples


## Design Philosopy

#### Obviousness and Correctness
The design of this library's API was based on an attempt to do two things:
1. Have one obviously correct way of doing each thing
2. Construct the API in such a way that it's difficult to produce invalid data

#### Delegates instead of Objects
For the most part, this library makes every attempt to avoid creating a huge taxonomy of objects and interfaces for all of the pluggable components.  Because the various actions (like key behaviors, tokenizing, formatting, etc) are simple and have minimal inputs and outputs, this library instead favors the use of delegates. 

This approach was selected for the following benefits:
1. It allows for quicker, easier composition of code, especially via lambdas where possible
1. It prevents a common dependency headache where a shared component would force an otherwise independent project to include a reference to this one
1. It discourages the preservation of state in mechanisms that should primarily exist to perform actions or transformations, but does not preclude an object from exposing a method that can be used instead while still maintaining access to the object state

#### Testing

#### Documentation