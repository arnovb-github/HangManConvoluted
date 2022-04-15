// this acts as the Program.cs, really.
// Program is now really just an entrypoint where DI setup takes place
// In this class we'll parse the command line arguments and start the game.

using System.CommandLine; // command line parser
public class Application : IApplication
{
    private IGame _game;
    private IGameOptions _options;
    public Application(IGame game, IGameOptions options)
    {
        _game = game;
        _options = options;
    }
    public async Task RunAsync(string[] args)
    {
        // Create some options:
        // we also define the defaults here
        var numGuesses = new Option<int>(
                aliases: new string[] {"--num-guesses", "-n"},
                getDefaultValue: () => 10,
                description: "Number of guesses.");
        var wordLength = new Option<int>(
                aliases: new string[] {"--word-length", "-l"},
                getDefaultValue: () => 0,
                description: "Wordlength, 0 for random length.");
        var boolOption = new Option<bool>(
                aliases: new string[] {"--easy", "-e"},
                // no need to specify a default value because a bool is false by default
                "Easy mode remembers characters you already guessed");
        var fileOption = new Option<FileInfo>(
                aliases: new string[] {"--wordfile", "-f"},
                 "Path to text-file with words to use. Separate words with newlines.");

        // Add the options to a root command:
        var rootCommand = new RootCommand("Hangman as a C# console app, it will change the world!")
        {
            numGuesses,
            wordLength,
            boolOption,
            fileOption
        };
        
        rootCommand.SetHandler( async (int i, int l, bool b, FileInfo f) =>
        {
            // apparently what we do here is to wire up a method that actually starts the progran
            _options.FilePath = f?.FullName;
            _options.NumGuesses = i;
            _options.EasyMode = b;
            _options.WordLength = l;
            await _game.RunAsync(); // we can finally run the actual game!
        }, numGuesses, wordLength, boolOption, fileOption);

        // Invoke root command, i.e., start the program!
        await rootCommand.InvokeAsync(args); // this invokes the method in the SetHandler method.
    }
}