namespace NFramework.Core.CLI.Abstractions;

public sealed class CliCommandContext
{
    public CliCommandContext(string name, IReadOnlyList<string> arguments)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Command name is required.", nameof(name));
        ArgumentNullException.ThrowIfNull(arguments);

        Name = name;
        Arguments = arguments;
    }

    public string Name { get; }

    public IReadOnlyList<string> Arguments { get; }
}
