// would like to have init instead of set, but Autofac doesn't support it yet.
public class GameOptions : IGameOptions
{
    public int NumGuesses { get; set; }
    public int WordLength { get; set; }
    public bool EasyMode { get; set; }
    public string? FilePath { get; set; }
}