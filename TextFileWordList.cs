public class TextFileWordList : IWordList
{
    IGameOptions _options;
    
    public TextFileWordList(IGameOptions options)
    {
        _options = options;    
    }

    public string GetRandomAnswer()
    {
        var wordsToChooseFrom = GetWordsFromTextFile().ToArray();
        if (wordsToChooseFrom.Length == 0)
        {
            throw new InvalidOperationException($"No words of the specified length found in the wordlist or {_options.FilePath} is empty.");
        }
        Random rnd = new Random();
        int index = rnd.Next(0, wordsToChooseFrom.Count());
        return wordsToChooseFrom[index];
    }

    // let's go overboard and do yield instead of a simple one-liner
    // theoretically, this should improve performance with very large files
    private IEnumerable<string> GetWordsFromTextFile()
    {
        // may throw several exceptions, see https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readlines?view=net-6.0
        foreach (string line in System.IO.File.ReadLines(_options.FilePath!))
        {  
            var word = line.Trim().ToLower();
            // only return words of the specified length
            if (_options.WordLength > 0 && word.Length == _options.WordLength && !string.IsNullOrWhiteSpace(word))
            {
                yield return word;
            }
            // return all words except empty ones
            if (_options.WordLength == 0 && !string.IsNullOrWhiteSpace(word)) 
            {
                yield return word;
            }
        }  
    }
}