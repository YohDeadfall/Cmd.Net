<h1>Cmd.Net</h1>

<p>Cmd.Net is a .Net library for building command-line applications with supporting netsh-like contexts and automatic help generation. It provides a convenient and laconic API for working with the command line. The library allows you to forget about parser code writing and to concentrate on the application logic.</p>

<h2>Features</h2>

<ol>
	<li>One command &mdash; one method. No if-else or switch-case needed to determine what shall be done.</li>
	<li>Running a single command or all commands from user input until the application is over.</li>
	<li>Argument parsing customization.</li>
	<li>Creating netsh-like contexts.</li>
	<li>Automatic help generation.</li>
</ol>

<h2>Compatibility</h2>

<p>.Net Framework 3.5 and later.</p>

<h2>Examples</h2>

<h4>Example 1</h4>

<p>Command with a description for the help generation and the id parameter custom parsing. The store parameter is optional and its default value is null.</p>

<pre><code>[Verb("copy")]
[Description("Creates a copy of the specified boot entry.")
public static void CopyBootEntry(
	[Output] TextWriter output,
	[Argument, Descritpion("Specifies the identifier of the entry to be copied."), TypeConverter(typeof(BootEntryGuidConverter))] Guid id,
	[Argument("store"), Descritpion("Specifies the description to be applied to the new entry.")] string store = null,
	[Argument("d"), Descritpion("Specifies the store to be used. If this option is not specified, the system store is used.")] string description
	)
{
}
</code></pre>

<pre><code>Creates a copy of the specified boot entry.
copy id /d:value [/store:value]
  id     Specifies the identifier of the entry to be copied.
  /d     Specifies the description to be applied to the new entry.
  /store Specifies the store to be used. If this option is not specified, the system store is used.
</code></pre>

<h4>Example 2</h4>

<p>Context with child commands and contexts.</p>

<pre><code>CommandContext rootContext = new CommandContext(
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
</code></pre>

<pre><code>netsh&gt;/?
add          Adds a configuration entry to a list of entries.
advfirewall  Changes to the 'netsh advfirewall' context.
...
netsh&gt;advfirewall /?
consec       Changes to the `netsh advfirewall consec' context.
...
</code></pre>