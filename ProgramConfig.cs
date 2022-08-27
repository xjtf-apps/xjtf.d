public class ProgramConfig
{
    public string Path
    {
        get
        {
            ConfigFile.Refresh();
            return ConfigFile.FullName;
        }
    }

    public bool Exists
    {
        get
        {
            ConfigFile.Refresh();
            return ConfigFile.Exists;
        }
    }

    public FileInfo ConfigFile { get; init; }

    private ProgramConfig(string filePath)
    {
        var info = new FileInfo(filePath);
        if (info == null)
            throw new ArgumentException(null, nameof(filePath));

        ConfigFile = info;
    }

    public static ProgramConfig FromArgs(string[] args) => new(args[0]);
}