public class GameOptions : IGameOptions
{
    private int numGuesses;
    public int NumGuesses
    {
        get { return numGuesses; }
        set { numGuesses = value; }
    }

    private int wordLength;
    public int WordLength
    {
        get { return wordLength; }
        set { wordLength = value; }
    }

    private bool easyMode;
    public bool EasyMode
    {
        get { return easyMode; }
        set { easyMode = value; }
    }

    private string? filePath;
    public string? FilePath
    {
        get { return filePath; }
        set { filePath = value; }
    }
}