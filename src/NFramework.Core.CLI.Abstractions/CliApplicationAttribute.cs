namespace NFramework.Core.CLI.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CliApplicationAttribute : Attribute
{
    public CliApplicationAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Application name is required.", nameof(name));

        Name = name;
    }

    public string Name { get; }

    public bool ValidateExamples { get; init; } = true;
}
