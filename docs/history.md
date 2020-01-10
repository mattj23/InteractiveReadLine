# The History Feature

The history feature is meant to emulate a simple version of the typical history behavior of most shells, in which two keys (typically the up and down arrows) cycle through the list of previously entered commands.

Accomplishing this requires three things: a collection of previously entered commands, a mechanism for cycling through these commands during the read line operation, and a means of updating the collection when a new command is entered.

## Trivial Code Example
```csharp
var history = new List<string> {"these are", "past entries"};

// The ReadLineConfig.Basic defaults with the up and down arrows set to navigate
// forward and backwards through the history. They will simply be disabled
// if no history source is set in the configuration
var config = ReadLineConfig.Basic
    .SetUpdatingHistorySource(history);

var result = ConsoleReadLine.ReadLine(config);

// If the ConsoleReadLine.ReadLine method is now called again with the same config 
// object, the last entered line will be the most recent element in the history
```

## Configuring ReadLine to use a History

Like all other configuration, setting up the `ReadLine()` method to use a collection of past commands is done through the `ReadLineConfig` object.  In accordance with the three ingredients mentioned above, you will need to decide three things:

1. What values should be provided to the input handler for it to use when navigating the history?

2. What should trigger the input handler to cycle forwards and backwards through the history?

3. Should the input handler automatically update the historical collection when the current line of input is finalized?

### Setting Key Behaviors
#### How History Navigation Works
Navigating through the history is performed by two methods of the input handler, and thus to the `IKeyBehaviorTarget` given to key behaviors.  They are, simply, `HistoryPrevious()` and `HistoryNext()`.  They are wrapped for convenience by `CommonKeyBehaviors.HistoryPrevious` and `CommonKeyBehaviors.HistoryNext`, respectively.

When the input handler starts, it has an empty 'original' text buffer.  This can be thought of as the last entry in the history, and the user can navigate back to it no matter how they step through the history.

When showing the starting text buffer, `HistoryNext` will do nothing, but `HistoryPrevious` will change the contents of the visible text buffer to the last entry in the history.  Further `HistoryPrevious` actions will iterate backwards through the list until stalling at the beginning, while `HistoryNext` actions will iterate forward through the list until the last entry is reached.  After the last entry, the contents of the 'original' text buffer will be displayed in whatever state they were the last time the user navigated into the history.

#### Setting History Navigation Behaviors

##### Basic Config Case

In the simplest case, simply using `ReadLineConfig.Basic` will come with the next and previous history behaviors mapped to the down and up arrows, respectively.

##### Adding Arrow Key Navigation

Starting with an empty or custom configuration that does not have the up and down arrow keys mapped to the history navigation, they can be added with the following extension method:

```csharp
var config = ReadLineConfig.Empty
    .AddUpDownHistoryNavigation();
```

##### Adding Custom Keys

Starting from a configuration that doesn't have navigation mapped, the next/previous history navigation behaviors can be mapped using the wrappers in `CommonKeyBehaviors`.
For example, the following code maps `HistoryNext` to Ctrl+Shift+Tab, and `HistoryPrevious` to the minus character.

```csharp
var config = ReadLineConfig.Empty
    .AddKeyBehavior(ConsoleKey.Tab, true, true, false, CommonKeyBehaviors.HistoryNext)
    .AddKeyBehavior('-', CommonKeyBehaviors.HistoryPrevious);
```

### Providing History Values

The history is provided to the configuration object as any object which implements `IReadOnlyList<string>`.  The sequence of the history is assumed to be oldest values at the beginning of the list, newest values at the end of the list.  The entries are plain text, as any lexing or formatting will only be done when a particular value is loaded to the input handler's text buffer.

**For a non-updating history:**
```csharp
// For a history that is not actively updated by the input handler, any object which
// provides IReadOnlyList<string> can be used
var history = new string[] {"history entry 0", "history entry 1", "history entry 2"};

// Set the source of the history values with SetHistorySource() 
var config = ReadLineConfig.Basic.SetHistorySource(history);

var result = ConsoleReadLine.ReadLine(config);
```

The input handler can take care of the task of updating the history values when its input is finalized (typically when the enter key is pressed) just before the value is returned from the method.  However, this will obviously not be possible with an `IReadOnlyList`, so under the covers the update is performed by an `Action<string>` delegate provided to the configuration.  

> The decision to split the history into a read-only component and an `Action` to update was to allow for the user to supply more complex update behavior than simply appending an entry onto the end of a list, such as the cleaning and max size behavior demonstrated in the later tutorial.

In the case of using a simple `List<string>` to store command history, there is a shortcut method which will set the update action delegate automatically (it will point directly at the list's `.Add()` method). New entries will simply be added to the end of the list with no regard for content or history size.

**For a simple self-updating history:**
```csharp
var history = new List<string> {"history entry 0", "history entry 1", "history entry 2"};

// In this case, the history must be a List<string>, and you will use SetUpdatingHistorySource,
// which will create the update action automatically
var config = ReadLineConfig.Basic
    .SetUpdatingHistorySource(history);

var result = ConsoleReadLine.ReadLine(config);

// If the ConsoleReadLine.ReadLine method is now called again with the same config 
// object, the last entered line will be the most recent element in the history
```

## Extra Code Recipes

### A Duplicate-Removing, Max Sized, Updating History
As an example of the usefulness of splitting the history into a component that is only used for reading, and an action delegate for updating, the following configuration stores its history in a generic list, but on update will first remove duplicates of the new entry before re-adding it to the end of the list.  Additionally, the history is capped at ten values, so the oldest value is removed if the list is full.

Notice in this case there's no need to use the `SetUpdatingHistorySource()` shortcut.  

Instead, the history list is added through the `SetHistorySource()` method, which will only ever read values.  The update action is defined as a lambda which encloses `history` in its scope and performs the duplicate removal and size capping before adding the new value.

```csharp
var history = new List<string>();

var config = ReadLineConfig.Basic
    // ... other configuration here ...
    .SetHistorySource(history)
    .SetHistoryUpdateAction(s =>
    {
        // Remove all previous entries of the text
        history.RemoveAll(x => x == s);
        if (history.Count >= 10)
            history.RemoveAt(0);
        history.Add(s);
    });

var result = ConsoleReadLine.ReadLine(config);
```