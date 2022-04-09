    public interface IGameOptions
    {
        int NumGuesses { get; set; }
        int WordLength { get; set; }
        bool EasyMode { get; set; }
        string? FilePath { get; set; }
    }