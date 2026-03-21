using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NFramework.Core.CLI.SpectreConsoleUI.Generators.Templates.Adapter;
using NFramework.Core.CLI.SpectreConsoleUI.Generators.Templates.Application;
using NFramework.Core.CLI.SpectreConsoleUI.Generators.Templates.Settings;
using Scriban;

namespace NFramework.Core.CLI.SpectreConsoleUI.Generators;

public sealed partial class CliSpectreApplicationGenerator
{
    private static void emit(
        ImmutableArray<INamedTypeSymbol> applications,
        ImmutableArray<CommandModel> commands,
        SourceProductionContext context
    )
    {
        foreach (INamedTypeSymbol application in applications)
        {
            AttributeData applicationAttribute = getAttribute(application, CliApplicationAttributeName)!;
            string applicationName = (string)applicationAttribute.ConstructorArguments[0].Value!;
            bool validateExamples = getNamedArgument(applicationAttribute, "ValidateExamples", true);

            string source = generateSource(application, applicationName, validateExamples, commands);
            context.AddSource(application.Name + ".CliApplication.g.cs", source);
        }
    }

    private static string generateSource(
        INamedTypeSymbol application,
        string applicationName,
        bool validateExamples,
        ImmutableArray<CommandModel> commands
    )
    {
        string namespaceName = application.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : application.ContainingNamespace.ToDisplayString();

        List<TemplateCommandModel> templateCommands = [.. commands.Select(createTemplateCommandModel)];

        ApplicationTemplate.Model applicationModel = new()
        {
            NamespaceName = namespaceName,
            HasNamespace = !string.IsNullOrEmpty(namespaceName),
            ApplicationTypeName = application.Name,
            ApplicationNameLiteral = toLiteral(applicationName),
            ValidateExamples = validateExamples,
            Commands = templateCommands,
        };

        return renderTemplate(ApplicationTemplate.Value, applicationModel);
    }

    private static TemplateCommandModel createTemplateCommandModel(CommandModel command, int index)
    {
        TemplateSettingsCommandModel settingsCommandModel = createTemplateSettingsCommandModel(command);
        TemplateAdapterCommandModel adapterCommandModel = createTemplateAdapterCommandModel(command);

        return new TemplateCommandModel(
            command.CommandTypeName,
            command.GeneratedAdapterTypeName,
            "commandConfigurator" + index,
            toLiteral(command.CommandName),
            toLiteral(command.Description),
            [.. command.Examples.Select(createTemplateExampleModel)],
            renderTemplate(SettingsTemplate.Value, new SettingsTemplate.Model { Command = settingsCommandModel }),
            renderTemplate(AdapterTemplate.Value, new AdapterTemplate.Model { Command = adapterCommandModel })
        );
    }

    private static TemplateExampleModel createTemplateExampleModel(ExampleModel example)
    {
        return new TemplateExampleModel(string.Join(", ", example.Arguments.Select(toLiteral)));
    }

    private static TemplateSettingsCommandModel createTemplateSettingsCommandModel(CommandModel command)
    {
        return new TemplateSettingsCommandModel(
            command.GeneratedSettingsTypeName,
            [.. command.Properties.Select(createTemplatePropertyModel)]
        );
    }

    private static TemplateAdapterCommandModel createTemplateAdapterCommandModel(CommandModel command)
    {
        return new TemplateAdapterCommandModel(
            command.GeneratedAdapterTypeName,
            command.CommandTypeName,
            command.GeneratedSettingsTypeName,
            command.SettingsTypeName,
            toLiteral(command.CommandName),
            toLiteral(DefaultCommandContextName),
            [.. command.Properties.Select(createTemplateAssignmentModel)]
        );
    }

    private static TemplatePropertyModel createTemplatePropertyModel(PropertyModel property)
    {
        string? descriptionAttribute = property.Description is null
            ? null
            : "[global::System.ComponentModel.Description(" + toLiteral(property.Description) + ")]";
        string? argumentAttribute = property.Argument is null
            ? null
            : "[global::Spectre.Console.Cli.CommandArgument("
                + property.Argument.Position
                + ", "
                + toLiteral(property.Argument.ValueName)
                + (property.Argument.IsRequired ? ", IsRequired = true" : string.Empty)
                + ")]";
        string? optionAttribute = property.Option is null
            ? null
            : "[global::Spectre.Console.Cli.CommandOption("
                + toLiteral(property.Option.Template)
                + ", "
                + (property.Option.IsHidden ? "true" : "false")
                + (property.Option.IsRequired ? ", IsRequired = true" : string.Empty)
                + (property.Option.ValueIsOptional ? ", ValueIsOptional = true" : string.Empty)
                + ")]";

        return new TemplatePropertyModel(
            descriptionAttribute,
            argumentAttribute,
            optionAttribute,
            property.TypeName,
            property.Name
        );
    }

    private static TemplateAssignmentModel createTemplateAssignmentModel(PropertyModel property)
    {
        return new TemplateAssignmentModel(property.Name);
    }

    private static string renderTemplate(string templateText, object model)
    {
        Template template = parseTemplate(templateText);
        return template.Render(model, member => member.Name);
    }

    private static string toLiteral(string value)
    {
        return SymbolDisplay.FormatLiteral(value, true);
    }

    private static Template parseTemplate(string source)
    {
        Template template = Template.Parse(source);
        if (!template.HasErrors)
            return template;

        string errors = string.Join(", ", template.Messages.Select(static message => message.Message));
        throw new InvalidOperationException("Failed to parse Scriban template: " + errors);
    }
}
