using CommandLine;

internal sealed class Options
{
    [Verb("run", isDefault: true, HelpText = "Run program.")]
    internal class Run
    {
    }
}
