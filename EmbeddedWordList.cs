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
    // we dont use a simple ReadAllLines because that's no fun
    // we go overboard and use a yield return to return each line as it is read.
    // it actually has its benefits, but it is really overkill.
    // In cases where a length is specified, it has the benefit of not reading in all words.
    // and then filter them afterwards. Not sure where the yield comes in though.
    // I am guessing it has to do with not having to create two lists? I have no idea.
    // it builds my list for me in an easy way
    // 'Add the things to an IEnumerable, I'll deal with them when you're done'.
    // I dunno, I just like it I suppose.
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
            if (_options.WordLength > 0 && word?.Length == _options.WordLength && !string.IsNullOrWhiteSpace(word))
            {
                yield return word;
            }
            // return all words except empty ones
            if (_options.WordLength == 0 && !string.IsNullOrEmpty(word) && !string.IsNullOrWhiteSpace(word))
            {
                yield return word;
            }
        }
    }
}