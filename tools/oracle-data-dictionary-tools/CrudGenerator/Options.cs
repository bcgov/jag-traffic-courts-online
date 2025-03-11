using CommandLine;

public class Options
{
    [Option('f', "file", Required = true, HelpText = "The configuration file")]
    public string File { get; set; } = string.Empty;

    [Option('o', "output", Required = true, HelpText = "The output path")]
    public string OutputPath { get; set; } = string.Empty;

    [Option("sid", Required = true, HelpText = "The Oracle database sid")]
    public string Sid { get; set; } = string.Empty;

    [Option("host", Required = true, HelpText = "The Oracle database host")]
    public string Host { get; set; } = string.Empty;

    [Option("username", Required = true, HelpText = "The Oracle database username")]
    public string Username { get; set; } = string.Empty;

    [Option("password", Required = true, HelpText = "The Oracle database password")]
    public string Password { get; set; } = string.Empty;

    [Option("trigger", Required = false, HelpText = "Generate triggers", SetName = "trigger")]
    public bool Triggers { get; set; } = false;

    [Option("crud", Required = false, HelpText = "Generate package for crud operations", SetName = "crud")]
    public bool Crud { get; set; } = false;

    [Option("model", Required = false, HelpText = "Generate C# models", SetName = "model")]
    public bool Model { get; set; } = false;

}
