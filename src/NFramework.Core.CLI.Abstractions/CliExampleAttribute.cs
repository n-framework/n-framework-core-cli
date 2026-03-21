namespace NFramework.Core.CLI.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class CliExampleAttribute : Attribute
{
    public CliExampleAttribute(params string[] arguments)
    {
        if (arguments.Length == 0)
            throw new ArgumentException("At least one example argument is required.", nameof(arguments));

        Arguments = arguments;
    }

    public IReadOnlyList<string> Arguments { get; }
}
