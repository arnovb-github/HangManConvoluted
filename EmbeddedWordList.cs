using System.Reflection;

public class EmbeddedWordList : IWordList
{
    IGameOptions _options;
    public EmbeddedWordList(IGameOptions options)
    {
        _options = options;
    }
    public string GetRandomAnswer()
    {
        List<string> wordsToChooseFrom = GetWordsFromResource().ToList();
        
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
    public IEnumerable<string> GetWordsFromResource()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(wordsResource);
        if (stream is null)
        {
            throw new FileNotFoundException($"Could not find the embedded {wordsResource} resource in the assembly.");
        }

        using StreamReader streamReader = new(stream);
        while (!streamReader.EndOfStream)
        {
            string? word = streamReader.ReadLine();
            if (_options.WordLength > 0 && word?.Length == _options.WordLength)
            {
                yield return word;
            }
            // return all words except empty ones
            if (_options.WordLength == 0 && !string.IsNullOrEmpty(word))
            {
                yield return word;
            }
        }
    }
}
