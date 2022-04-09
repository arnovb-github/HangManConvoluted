public class Game : IGame
{
    private  IGameOptions _options;
    // this is the word to guess, as char array
    private char[] _answer = null!;
    private readonly char _placeHolder = '_';
    private char[] _guess = null!;

    private List<char> _guessedLetters = new List<char>();

    public Game(IGameOptions options)
    {
        _options = options;
    }

    public void Run()
    {
        try 
        {
            _answer = SelectRandomAnswer().ToCharArray();
            _guess = CreateGuess();
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return;
        }

        // main loop
        #region Actual game loop
        var numGuesses = _options.NumGuesses;
        while (numGuesses > 0)
        {
            Console.WriteLine($"You have {numGuesses} guesses left.");
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
                Console.WriteLine("Invalid guess.");
                continue;
            }
            var g = (char)(key - ConsoleKey.A + 'a'); // no idea why this works or what it even does. We get the correct character though.
            if (_options.EasyMode && _guessedLetters.Contains(g))
            {
                Console.WriteLine("You already guessed that letter.");
                continue;
            }

            StoreGuessedLetter(g);
            if (IsMatch(g))
            {
                UpdateGuess(g, _guess); // inefficient
                if (IsSolved())
                {
                    Console.WriteLine($"You win! The answer was '{new string(_answer)}'.");
                    return;
                }
                Console.WriteLine(_guess);
            }
            numGuesses--;
        }
        #endregion
        Console.WriteLine($"You lose! The answer was '{new string(_answer)}'. Better luck next time!");
        return;
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
    private string SelectRandomAnswer()
    {
        if (string.IsNullOrEmpty(_options.FilePath)) // there was no filename specified
        {
            return SelectAnswerFromResource();
        }
        else
        {
            return SelectAnswerFromTextFile();
        }
    }

    private string SelectAnswerFromTextFile()
    {
        var tfwl = new TextFileWordList(_options);
        return tfwl.GetRandomAnswer();
    }

    private string SelectAnswerFromResource()
    {
        var dwl = new EmbeddedWordList(_options);
        return dwl.GetRandomAnswer();
    }

    private bool IsMatch(char c)
    {
        return _answer.Contains(c);
    }

    // you can have a discussion about this:
    // should it take in guess as an argument and return it?
    // it is not needed since _guess lives outside the scope of the method,
    // but it 'feels' cleaner.
    private char[] UpdateGuess(char c, char[] _guess)
    {
        for (int i = 0; i < _answer.Length; i++)
        {
            if (_answer[i] == c)
            {
                _guess[i] = c;
            }
        }
        return _guess;
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