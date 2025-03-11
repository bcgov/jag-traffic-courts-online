using CommandLine;

public class Options
{
    [Option("path", Required = true, HelpText = "The path to search for files with create or replace statements. Only used if SVN keyword $HeadURL: $ is not found")]
    public string Path { get; set; } = string.Empty;

    [Option("sid", Required = true, HelpText = "The Oracle database sid")]
    public string Sid { get; set; } = string.Empty;

    [Option("host", Required = true, HelpText = "The Oracle database host")]
    public string Host { get; set; } = string.Empty;

    [Option("username", Required = true, HelpText = "The Oracle database username")]
    public string Username { get; set; } = string.Empty;

    [Option("password", Required = true, HelpText = "The Oracle database password")]
    public string Password { get; set; } = string.Empty;
}
