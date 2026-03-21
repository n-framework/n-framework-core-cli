namespace NFramework.Core.CLI.Abstractions;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class CliOptionAttribute : Attribute
{
    public CliOptionAttribute(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("Template is required.", nameof(template));

        Template = template;
    }

    public string Template { get; }

    public bool IsHidden { get; init; }

    public bool IsRequired { get; init; }

    public bool ValueIsOptional { get; init; }
}
