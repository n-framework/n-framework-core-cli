using System.Collections.Immutable;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private sealed class ExampleModel
    {
        public ExampleModel(ImmutableArray<string> arguments)
        {
            Arguments = arguments;
        }

        public ImmutableArray<string> Arguments { get; }
    }
}
