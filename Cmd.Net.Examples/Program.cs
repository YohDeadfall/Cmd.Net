using System;
using System.ComponentModel;

namespace Cmd.Net.Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            new CommandContext(
                "root",
                new DelegateCommand(new Action<string, Attributes, bool, bool, bool, bool, bool, string, bool, bool, bool, bool, TimeField, bool, bool, bool>(Dir)),
                new DelegateCommand(new Action<bool, string, string>(Move))
                )
                .ExecuteAll();
        }

        #region dir

        [Flags]
        enum Attributes
        {
            None = 0,

            [Flag('D')]
            [Description("Directories")]
            Directories = 1,

            [Flag('H')]
            [Description("Hidden files")]
            HiddenFiles = Directories << 1,

            [Flag('S')]
            [Description("System files")]
            SystemFiles = HiddenFiles << 1,

            [Flag('L')]
            [Description("Reparse points")]
            ReparsePoints = SystemFiles << 1,

            [Flag('R')]
            [Description("Read-only files")]
            ReadOnlyFiles = ReparsePoints << 1,

            [Flag('I')]
            [Description("Files ready for archiving")]
            FilesReadyForArchiving = ReadOnlyFiles << 1,

            [Flag('A')]
            [Description("Not content indexed files")]
            NotContentIndexedFiles = FilesReadyForArchiving << 1
        }

        enum TimeField
        {
            [Flag('C')]
            [Description("Creation")]
            Creation,

            [Flag('A')]
            [Description("Last access")]
            LastAccess,

            [Flag('W')]
            [Description("Last written")]
            LastWritten
        }

        [Verb("dir")]
        [Description("Displays a list of files and subdirectories in a directory.")]
        [Remarks(
            "Switches may be preset in the DIRCMD environment variable. Override\r\n"+
            "preset switches by prefixing any switch with - (hyphen)--for example, /-W."
            )]
        [Example("/A:D", "Displays directories.")]
        [Example("/A:H", "Displays hidden files.")]
        static void Dir(
            [Description("Specifies drive, directory, and/or files to list.")]
            string fileName = null,

            [Argument("A")]
            [Description("Displays files with specified attributes.")]
            Attributes attributes = Attributes.None,

            [Argument("B")]
            [Description("Uses bare format (no heading information or summary).")]
            bool bareFormat = false,

            [Argument("C")]
            [Description("Display the thousand separator in file sizes. This is the\r\ndefault. Use /-C to disable display of separator.")]
            bool thousandSeparator = true,

            [Argument("D")]
            [Description("Same as wide but files are list sorted by column.")]
            bool wideListFormatSorted = false,

            [Argument("L")]
            [Description("Uses lowercase.")]
            bool lowercase = false,

            [Argument("N")]
            [Description("New long list format where filenames are on the far right.")]
            bool longListFormat = false,

            [Argument("O")]
            [Description("List by files in sorted order.")]
            string sortOrder = null,

            [Argument("P")]
            [Description("Pauses after each screenful of information.")]
            bool pauses = false,

            [Argument("Q")]
            [Description("Display the owner of the file.")]
            bool displayOwner = false,

            [Argument("R")]
            [Description("Display alternate data streams of the file.")]
            bool displayAltStreams = false,

            [Argument("S")]
            [Description("Displays files in specified directory and all subdirectories.")]
            bool includeSubdirectories = false,

            [Argument("T")]
            [Description("Controls which time field displayed or used for sorting.")]
            TimeField timeField = TimeField.LastWritten,

            [Argument("W")]
            [Description("Uses wide list format.")]
            bool wideListFormat = false,

            [Argument("X")]
            [Description(
                "This displays the short names generated for non-8dot3 file\r\n" +
                "names. The format is that of /N with the short name inserted\r\n" +
                "before the long name. If no short name is present, blanks are\r\n" +
                "displayed in its place."
                )]
            bool displayShortNames = false,

            [Argument("4")]
            [Description("Displays four-digit years.")]
            bool fourDigitYears = false
            )
        {

        }

        #endregion

        #region move

        [Verb("move")]
        [Description("Moves files and renames files and directories.")]
        static void Move(
            [Argument("Y")]
            [Description(
            "Suppresses prompting to confirm you want to\r\n" +
            "overwrite an existing destination file."
            )]
            bool overWrite,

            [Argument("s")]
            [Description("Specifies the file or the directory you want to rename.")]
            string source,

            [Argument("d")]
            [Description("Specifies the new name of the file or the directory.")]
            string destination
            )
        {

        }

        #endregion
    }
}
