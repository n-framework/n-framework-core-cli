using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private static INamedTypeSymbol? getApplicationType(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol typeSymbol)
            return null;

        return hasAttribute(typeSymbol, CliApplicationAttributeName) ? typeSymbol : null;
    }

    private static CommandModel? getCommandModel(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol commandType)
            return null;

        AttributeData? commandAttribute = getAttribute(commandType, CliCommandAttributeName);
        if (commandAttribute is null)
            return null;

        INamedTypeSymbol? settingsType = getSettingsType(commandType, context.SemanticModel.Compilation);
        if (settingsType is null)
            return null;

        if (commandAttribute.ConstructorArguments[0].Value is not string commandName)
            return null;
        if (commandAttribute.ConstructorArguments[1].Value is not string commandDescription)
            return null;
        if (string.IsNullOrWhiteSpace(commandName) || string.IsNullOrWhiteSpace(commandDescription))
            return null;

        ImmutableArray<ExampleModel> examples =
        [
            .. commandType
                .GetAttributes()
                .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == CliExampleAttributeName)
                .SelectMany(createExamples),
        ];

        ImmutableArray<PropertyModel> properties =
        [
            .. settingsType
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(static property => !property.IsStatic)
                .Select(createPropertyModel)
                .Where(static property => property is not null)
                .Select(static property => property!),
        ];

        if (!hasPublicParameterlessConstructor(settingsType))
            return null;

        return new CommandModel(
            commandType.ToDisplayString(FullyQualifiedTypeFormat),
            commandType.Name,
            settingsType.ToDisplayString(FullyQualifiedTypeFormat),
            settingsType.Name,
            commandName,
            commandDescription,
            examples,
            properties
        );
    }

    private static IEnumerable<ExampleModel> createExamples(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length != 1)
            yield break;

        TypedConstant argument = attribute.ConstructorArguments[0];
        if (argument.Kind != TypedConstantKind.Array)
            yield break;

        ImmutableArray<string> values =
        [
            .. argument.Values.Select(static value => value.Value as string ?? string.Empty),
        ];
        if (values.Length > 0)
            yield return new ExampleModel(values);
    }

    private static PropertyModel? createPropertyModel(IPropertySymbol property)
    {
        AttributeData? cliArgumentAttribute = getAttribute(property, CliArgumentAttributeName);
        AttributeData? cliOptionAttribute = getAttribute(property, CliOptionAttributeName);

        if (cliArgumentAttribute is null && cliOptionAttribute is null)
            return null;

        AttributeData? descriptionAttribute = getAttribute(property, DescriptionAttributeName);
        string? description = descriptionAttribute?.ConstructorArguments[0].Value as string;

        if (cliArgumentAttribute is not null)
        {
            int position = (int)cliArgumentAttribute.ConstructorArguments[0].Value!;
            string valueName = (string)cliArgumentAttribute.ConstructorArguments[1].Value!;
            bool isRequired = getNamedArgument(cliArgumentAttribute, nameof(CliArgumentModel.IsRequired), false);

            return new PropertyModel(
                property.Name,
                property.Type.ToDisplayString(FullyQualifiedTypeFormat),
                description,
                new CliArgumentModel(position, valueName, isRequired),
                null
            );
        }

        string template = (string)cliOptionAttribute!.ConstructorArguments[0].Value!;
        bool isHidden = getNamedArgument(cliOptionAttribute, nameof(CliOptionModel.IsHidden), false);
        bool isRequiredOption = getNamedArgument(cliOptionAttribute, nameof(CliOptionModel.IsRequired), false);
        bool valueIsOptional = getNamedArgument(cliOptionAttribute, nameof(CliOptionModel.ValueIsOptional), false);

        return new PropertyModel(
            property.Name,
            property.Type.ToDisplayString(FullyQualifiedTypeFormat),
            description,
            null,
            new CliOptionModel(template, isHidden, isRequiredOption, valueIsOptional)
        );
    }

    private static bool hasPublicParameterlessConstructor(INamedTypeSymbol settingsType)
    {
        return settingsType.InstanceConstructors.Any(static constructor =>
            constructor.Parameters.Length == 0 && constructor.DeclaredAccessibility == Accessibility.Public
        );
    }

    private static INamedTypeSymbol? getSettingsType(INamedTypeSymbol commandType, Compilation compilation)
    {
        INamedTypeSymbol? asyncCliCommandInterface = compilation.GetTypeByMetadataName(
            AsyncCliCommandInterfaceMetadataName
        );
        if (asyncCliCommandInterface is null)
            return null;

        return commandType
                .AllInterfaces.FirstOrDefault(symbol =>
                    SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, asyncCliCommandInterface)
                )
                ?.TypeArguments[0] as INamedTypeSymbol;
    }

    private static AttributeData? getAttribute(ISymbol symbol, string fullyQualifiedAttributeName)
    {
        return symbol
            .GetAttributes()
            .FirstOrDefault(attribute => attribute.AttributeClass?.ToDisplayString() == fullyQualifiedAttributeName);
    }

    private static bool hasAttribute(ISymbol symbol, string fullyQualifiedAttributeName)
    {
        return getAttribute(symbol, fullyQualifiedAttributeName) is not null;
    }

    private static T getNamedArgument<T>(AttributeData attribute, string name, T fallback)
    {
        foreach (KeyValuePair<string, TypedConstant> namedArgument in attribute.NamedArguments)
        {
            if (namedArgument.Key == name && namedArgument.Value.Value is T value)
                return value;
        }

        return fallback;
    }
}
