using System.Text;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private static string sanitize(string value)
    {
        StringBuilder builder = new(value.Length);
        foreach (char character in value)
        {
            if (char.IsLetterOrDigit(character) || character == '_')
                _ = builder.Append(character);
            else
                _ = builder.Append('_');
        }

        return builder.ToString();
    }
}
