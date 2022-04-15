public class TextFileWordList : IWordList
{
    IGameOptions _options;
    
    public TextFileWordList(IGameOptions options)
    {
        _options = options;    
    }

    public async Task<string> GetRandomAnswerAsync()
    {
        var wordsToChooseFrom = new List<string>();
        // totally pointless to use async and IAsyncEnumerable here,
        // because all we do is build up a list that has to be complete anyway.
        // but it is an example of how to use them.
        await foreach (var word in GetWordsFromTextFileAsync())
        {
            wordsToChooseFrom.Add(word);
        }
        if (wordsToChooseFrom.Count() == 0)
        {
            throw new InvalidOperationException($"No words of the specified length found in the wordlist or {_options.FilePath} is empty.");
        }
        Random rnd = new Random();
        int index = rnd.Next(0, wordsToChooseFrom.Count());
        return wordsToChooseFrom[index];
    }

    // let's go overboard and do yield instead of a simple one-liner
    // theoretically, this should improve performance with very large files
    // also, throw in IAsyncEnumerable just for fun
    private async IAsyncEnumerable<string> GetWordsFromTextFileAsync()
    {
        // may throw several exceptions, see https://docs.microsoft.com/en-us/dotnet/api/system.io.file.readlines?view=net-6.0
        foreach (string line in await System.IO.File.ReadAllLinesAsync(_options.FilePath!))
        {  
            var word = line.Trim().ToLower();
            // only return words of the specified length
            if (_options.WordLength > 0 
                && word.Length == _options.WordLength 
                && !string.IsNullOrWhiteSpace(word))
            {
                yield return word;
            }
            // return all words except empty ones
            if (_options.WordLength == 0 
                && !string.IsNullOrWhiteSpace(word)) 
            {
                yield return word;
            }
        }  
    }
}