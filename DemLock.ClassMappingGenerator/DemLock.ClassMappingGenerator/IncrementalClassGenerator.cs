﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;

namespace DemLock.ClassMappingGenerator;

[Generator]
public class IncrementalClassGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor UnkownGeneratorError = new DiagnosticDescriptor(id: "DEMLOCK_001",
                                                                                                  title: "Failed to generate due to an unhandled exception",
                                                                                                  messageFormat: "exception '{0}'.",
                                                                                                  category: "DemlockClassMappingGenerator",
                                                                                                  DiagnosticSeverity.Error,
                                                                                                  isEnabledByDefault: true);
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<AdditionalText> textFiles =
            context.AdditionalTextsProvider.Where(file => file.Path.EndsWith(".class.json"));

        IncrementalValuesProvider<string> namesAndContent =
            textFiles.Select((text, cancellationToken) => text.GetText(cancellationToken)!.ToString());

        context.RegisterSourceOutput(namesAndContent, (spc, content) =>
        {
            var def = GenerateClass(content);
            try
            {
                spc.AddSource($"{def.Item1}.g.cs", def.Item2);
            }
            catch (Exception ex)
            {
                spc.ReportDiagnostic(Diagnostic.Create(UnkownGeneratorError, Location.None, ex.ToString()));
            }

        });

        // Add the marker attribute to the compilation.
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "BaseEntity.g.cs",
            SourceText.From(BaseEntity(), Encoding.UTF8)));
    }

    public (string, string) GenerateClass(string text)
    {
        var classDef = JsonConvert.DeserializeObject<ClassDefinition>(text.ToString());

        StringBuilder sb = new StringBuilder();

        //sb.AppendLine("// <auto-generated/>");
        sb.AppendLine($"public partial class {classDef.ClassName}: BaseEntity");
        sb.AppendLine($"{{");

        StringBuilder scb = new StringBuilder();
        scb.AppendLine("public override void UpdateProperty(ReadOnlySpan<int> path, object value)");
        scb.AppendLine("{");
        scb.AppendLine("switch (path[0])");
        scb.AppendLine("{");
        foreach (var v in classDef.Fields)
        {
            if (v.Value.Children == null && IsPrimitiveType(v.Value?.Type ?? ""))
            {
                sb.AppendLine($"\tpublic {v.Value.Type} {v.Key} {{ get; set; }}");

                scb.AppendLine($"\t\tcase {v.Value.Path}:");
                scb.AppendLine($"\t\t\t{v.Key} = ({v.Value.Type})value;");
                scb.AppendLine($"\t\t\tbreak;");
                continue;
            }

            if (v.Value.Children == null && v.Value.Type.StartsWith("List"))
            {
                
            }
        }

        scb.AppendLine("default:");
        scb.AppendLine("break;");
        scb.AppendLine("}");
        scb.AppendLine("}");
        sb.AppendLine(scb.ToString());
        sb.AppendLine($"}}");
        // Build up the source code.
        string source = CodeTemplates.WithNamespace(sb.ToString(), "DemLock.Entities.Generated");

        return ($"{classDef.ClassName}", source);
    }

    public string BaseEntity()
    {
        StringBuilder baseEntity = new StringBuilder();

        baseEntity.AppendLine("public abstract partial class BaseEntity {");
        baseEntity.AppendLine("public abstract void UpdateProperty(ReadOnlySpan<int> path, object value);");
        baseEntity.AppendLine("}");

        return CodeTemplates.WithNamespace(baseEntity.ToString(), "DemLock.Entities.Generated");
    }

    public bool IsPrimitiveType(string typeName)
    {
        return new string[]
        {
            "Int32",
            "float"
        }.Contains(typeName);
    }

    public string MapType(string typeName) => typeName switch
    {
        "Int32" => "int"

    };
}