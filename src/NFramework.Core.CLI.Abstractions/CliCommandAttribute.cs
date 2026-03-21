namespace NFramework.Core.CLI.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CliCommandAttribute : Attribute
{
    public CliCommandAttribute(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Command name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Command description is required.", nameof(description));

        Name = name;
        Description = description;
    }

    public string Name { get; }

    public string Description { get; }
}
