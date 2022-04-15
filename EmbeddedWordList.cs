using System.Reflection;

public class EmbeddedWordList : IWordList
{
    IGameOptions _options;
    public EmbeddedWordList(IGameOptions options)
    {
        _options = options;
    }

    // It is utterly pointless to use async and IAsyncEnumerable here, 
    // because all we do is build up a list that has to be complete anyway.  
    // But it is an example of how to use them.
    public async Task<string> GetRandomAnswerAsync()
    {
        var wordsToChooseFrom = new List<string>();
        await foreach(var word in GetWordsFromResourceAsync())
        {
            wordsToChooseFrom.Add(word);
        }
        if (wordsToChooseFrom.Count() == 0)
        {
            throw new InvalidOperationException("No words of the specified length found in the wordlist");
        }
        Random rnd = new Random();
        int index = rnd.Next(0, wordsToChooseFrom.Count);
        return wordsToChooseFrom[index];
    }


    // It is very important that you specify the correct name for the resource file
    // The general format is [assembly name].[directory].[file name].
    // It is case-sensitive!
    const string wordsResource = "HangManConvoluted.wordlist.txt";
    // also, let's make things overly complicated by yielding an IAsyncEnumerable.
    public async IAsyncEnumerable<string> GetWordsFromResourceAsync()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(wordsResource);
        if (stream is null)
        {
            throw new FileNotFoundException($"Could not find the embedded {wordsResource} resource in the assembly.");
        }

        using StreamReader streamReader = new(stream);
        while (!streamReader.EndOfStream)
        {
            string? word = await streamReader.ReadLineAsync();
            if (_options.WordLength > 0 
                && !string.IsNullOrWhiteSpace(word)
                && word.Length == _options.WordLength)
            {
                yield return word.Trim().ToLower();
            }
            // return all words except empty ones
            if (_options.WordLength == 0 
                && !string.IsNullOrWhiteSpace(word))
            {
                yield return word.Trim().ToLower();
            }
        }
    }
}