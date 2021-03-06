public class Game : IGame
{
    private  IGameOptions _options;

    private IAnimation _animation;
    // this is the word to guess, as char array, because why not? They're faster...
    private char[] _answer = null!;
    private readonly char _placeHolder = '_';
    private char[] _guess = null!;

    private List<char> _guessedLetters = new List<char>();

    public Game(IGameOptions options, IAnimation animation)
    {
        _options = options;
        _animation = animation;
    }

    // RunAsync is the entry point for the game.
    // There is no point in having it be a Task,
    // because it is a turn-based game that cannot start after anything
    // that could be considered async has finished.
    // Thast async stuff is total overkill,
    // All it does is create a list of words asynchronously(!),
    // then pick a single random one from that list.
    // There is absolutely no need for any of this to be async.
    // What is does illustrate is that once there's async somewhere in the code,
    // it spreads all over the place.
    public async Task RunAsync()
    {
        try 
        {
            _answer = await SelectRandomAnswerAsync();
            _guess = CreateGuess();
        }
        // SelectRandomAnswerAsync() may throw several exceptions
        // we could catch them all separately, but I prefer to catch them all at once,
        // all we do is display the error message anyway.
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return;
        }

        #region Actual game loop
        uint guessCounter = 0;
        string msg = string.Empty;
        while (guessCounter < _options.NumGuesses)
        {
            Console.Clear();
            Console.WriteLine(_animation.Frames[guessCounter]);
            Console.WriteLine(msg);
            Console.WriteLine($"{new string(_guess)} ({_options.NumGuesses - guessCounter} guesses left.)");
            Console.WriteLine("Type a letter to guess");
            msg = string.Empty; // reset or it will be printed again
            ConsoleKey key = Console.ReadKey(true).Key;
            if (key is ConsoleKey.Escape)
            {
                Console.Clear();
                Console.Write("Hangman was closed.");
                return;
            }
            Console.WriteLine();
            
            if (!IsValid(key))
            {
                msg = "Invalid guess.";
                continue;
            }
            var g = (char)(key - ConsoleKey.A + 'a'); // no idea why this works or what it even does. We get the correct character though.
            if (_options.EasyMode && _guessedLetters.Contains(g))
            {
                msg = "You already guessed that letter.";
                continue;
            }

            StoreGuessedLetter(g);
            if (!IsMatch(g)) // no match
            {
                msg = "No match.";
            }
            else
            {
                UpdateGuess(g, _guess); // we can argue over this all day. what is the best way to update the guess?
                if (IsSolved())
                {
                    Console.Clear();
                    Console.WriteLine($"You won in {guessCounter + 1} attempts.");
                    return;
                }
                continue;
            }
            guessCounter++;
        }
        #endregion
        Console.Clear();
        // we introduce a method that also writes to the console,
        // and on top of that introduces a delay!
        // we use it method primarily as an excuse to bring in Async/Await, really.
        // It has no real use in this project, but it nicely fits the need for having an
        // essentially synchronous game be async.
        await PlayDeathAnimation(); 
        Console.WriteLine($"You lose! The answer was '{new string(_answer)}'. Better luck next time!");
        return;
    }

    private async Task PlayDeathAnimation()
    {
        for (int i = 0; i < _animation.DeathFrames.Length; i++)
        {
            Console.Clear();
            Console.WriteLine(_animation.DeathFrames[i]);
            await Task.Delay(TimeSpan.FromMilliseconds(150));
        }
    }

    private void StoreGuessedLetter(char c)
    {
        if (!_guessedLetters.Contains(c)) // only store unique letters
        {
            _guessedLetters.Add(c);
        }
    }

    private bool IsSolved()
    {
        for(int i=0;i<_answer.Length;i++){
            if(_guess[i] != _answer[i]) { return false; } 
        }
        return true;
    }

    private bool IsValid(ConsoleKey ck)
    {
        return ck >= ConsoleKey.A && ck <= ConsoleKey.Z;
    }
    private async Task<char[]> SelectRandomAnswerAsync()
    {
        if (string.IsNullOrEmpty(_options.FilePath)) // there was no filename specified, use built-in list
        {
            return (await SelectAnswerFromResource()).ToCharArray();
        }
        else
        {
            return (await SelectAnswerFromTextFile()).ToCharArray();
        }
    }

    private async Task<string> SelectAnswerFromTextFile()
    {
        var tfwl = new TextFileWordList(_options); // No DI for this yet.
        return await tfwl.GetRandomAnswerAsync();
    }

    private async Task<string> SelectAnswerFromResource()
    {
        var ewl = new EmbeddedWordList(_options); // No DI for this yet.
        return await ewl.GetRandomAnswerAsync();
    }

    private bool IsMatch(char c)
    {
        return _answer.Contains(c);
    }

    // you can have a discussion about this:
    // should it take in guess as an argument and return it?
    // it is not needed since _guess lives outside the scope of the method,
    // but it 'feels' cleaner.
    // also perhaps we should make it more clear that _answer is by definition equal length to _guess?
    private char[] UpdateGuess(char c, char[] guess)
    {
        for (int i = 0; i < _answer.Length; i++)
        {
            if (_answer[i] == c)
            {
                guess[i] = c;
            }
        }
        return guess;
    }

    // populate guess with placeholder characters
    private char[] CreateGuess()
    {
        var guess = new char[_answer.Length];
        for (int i = 0; i < _answer.Length; i++)
        {
            guess[i] = _placeHolder;
        }
        return guess;
    }   
}