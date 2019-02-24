# Cmd.Net is archived

The project is archived due to Microsoft work in the same area. [System.CommandLine](https://github.com/dotnet/command-line-api) is an actively developing project that could be used instead of this library.

# Cmd.Net

Cmd.Net is a .Net library for building command-line applications with supporting netsh-like contexts and automatic help generation. It provides a convenient and laconic API for working with the command line. The library allows you to forget about parser code writing and to concentrate on the application logic.

## NuGet

To install Cmd.Net, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

```powershell
Install-Package Cmd.Net
```

## Features

1. One command &mdash; one method. No if-else or switch-case needed to determine what shall be done.
2. Running a single command or all commands from user input until the application is over.
3. Argument parsing customization.
4. Creating netsh-like contexts.
5. Automatic help generation.

## Compatibility

.Net Framework 3.5 and later.

## Examples

#### Example 1

Command with a description for the help generation and the id parameter custom parsing. The store parameter is optional and its default value is null.

```csharp
[Verb("copy")]
[Description("Creates a copy of the specified boot entry.")
public static void CopyBootEntry(
	[Output] TextWriter output,
	[Argument, Descritpion("Specifies the identifier of the entry to be copied."), TypeConverter(typeof(BootEntryGuidConverter))] Guid id,
	[Argument("store"), Descritpion("Specifies the description to be applied to the new entry.")] string store = null,
	[Argument("d"), Descritpion("Specifies the store to be used. If this option is not specified, the system store is used.")] string description
	)
{
}
```

```text
Creates a copy of the specified boot entry.
copy id /d:value [/store:value]
  id     Specifies the identifier of the entry to be copied.
  /d     Specifies the description to be applied to the new entry.
  /store Specifies the store to be used. If this option is not specified, the system store is used.
```

#### Example 2

Context with child commands and contexts.

```csharp
CommandContext rootContext = new CommandContext(
	"netsh",
	new Command(new Action(AddHelper)),
	new CommandContext(
		"advfirewall",
		"Changes to the 'netsh advfirewall' context.",
		new CommandContext(
			"consec",
			"Changes to the 'netsh advfirewall consec' context."
			/* ...*/
			),
		/* ...*/
		),
	/* ...*/
	);
```

```text
netsh>/?
add          Adds a configuration entry to a list of entries.
advfirewall  Changes to the 'netsh advfirewall' context.
...
netsh>advfirewall /?
consec       Changes to the `netsh advfirewall consec' context.
...
```
