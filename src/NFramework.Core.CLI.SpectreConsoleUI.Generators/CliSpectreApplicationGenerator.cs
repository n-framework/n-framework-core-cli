using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

[Generator]
public sealed partial class CliSpectreApplicationGenerator : IIncrementalGenerator
{
    private const string CliApplicationAttributeName = "NFramework.Core.CLI.Abstractions.CliApplicationAttribute";
    private const string CliCommandAttributeName = "NFramework.Core.CLI.Abstractions.CliCommandAttribute";
    private const string CliExampleAttributeName = "NFramework.Core.CLI.Abstractions.CliExampleAttribute";
    private const string CliArgumentAttributeName = "NFramework.Core.CLI.Abstractions.CliArgumentAttribute";
    private const string CliOptionAttributeName = "NFramework.Core.CLI.Abstractions.CliOptionAttribute";
    private const string DescriptionAttributeName = "System.ComponentModel.DescriptionAttribute";
    private const string AsyncCliCommandInterfaceMetadataName = "NFramework.Core.CLI.Abstractions.IAsyncCliCommand`1";
    private const string DefaultCommandContextName = "__default_command";

    private static readonly SymbolDisplayFormat FullyQualifiedTypeFormat = new(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
            | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<INamedTypeSymbol> applicationTypes = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                static (generatorContext, _) => getApplicationType(generatorContext)
            )
            .Where(static symbol => symbol is not null)!
            .Select(static (symbol, _) => symbol!);

        IncrementalValuesProvider<CommandModel> commandModels = context
            .SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                static (generatorContext, _) => getCommandModel(generatorContext)
            )
            .Where(static model => model is not null)!
            .Select(static (model, _) => model!);

        IncrementalValueProvider<ImmutableArray<INamedTypeSymbol>> collectedApplications = applicationTypes
            .Collect()
            .Select(
                static (symbols, _) =>
                    symbols.Distinct(SymbolEqualityComparer.Default).Cast<INamedTypeSymbol>().ToImmutableArray()
            );

        IncrementalValueProvider<ImmutableArray<CommandModel>> collectedCommands = commandModels
            .Collect()
            .Select(
                static (models, _) =>
                    models
                        .Distinct()
                        .OrderBy(static model => model.CommandName, System.StringComparer.Ordinal)
                        .ToImmutableArray()
            );

        context.RegisterSourceOutput(
            collectedApplications.Combine(collectedCommands),
            static (productionContext, pair) => emit(pair.Left, pair.Right, productionContext)
        );
    }
}
