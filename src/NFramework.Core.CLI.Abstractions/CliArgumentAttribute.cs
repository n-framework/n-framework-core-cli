namespace NFramework.Core.CLI.Abstractions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class CliArgumentAttribute : Attribute
{
    public CliArgumentAttribute(int position, string valueName)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(position);
        if (string.IsNullOrWhiteSpace(valueName))
            throw new ArgumentException("Value name is required.", nameof(valueName));

        Position = position;
        ValueName = valueName;
    }

    public int Position { get; }

    public string ValueName { get; }

    public bool IsRequired { get; init; }
}
