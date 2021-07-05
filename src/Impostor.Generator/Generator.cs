using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using SourceCodeWriter;

namespace Impostor.Generator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public Generator()
        {
            Assembly.LoadFrom("/home/js6pak/Development/CSharp/SourceCodeWriter/SourceCodeWriter/bin/Debug/netstandard2.0/SourceCodeWriter.dll");
        }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private void GenerateEnum(GeneratorExecutionContext context, string name, string @namespace = "", bool flags = false, string baseType = null)
        {
            var dictionary = JsonSerializer.DeserializeAsync<Dictionary<string, int>>(typeof(Generator).Assembly.GetManifestResourceStream($"Impostor.Generator.Dumped.{name}.json")!).GetAwaiter().GetResult();

            var sourceCodeWriter = new SourceCodeWriter.SourceCodeWriter
            {
                UseTabs = false,
            };

            new SourceFile
            {
                Namespace = "Impostor.Api.Innersloth" + @namespace,
                Children =
                {
                    new SourceEnum(name)
                    {
                        Access = AccessModifiers.Public,
                        Flags = flags,
                        BaseType = baseType,
                        Children = dictionary!.Select(x => new SourceEnumValue(x.Key, x.Value)).Cast<SourceEnum.IChild>().ToList(),
                    },
                },
            }.Write(sourceCodeWriter);

            context.AddSource(name, sourceCodeWriter.ToString());
        }

        public void Execute(GeneratorExecutionContext context)
        {
#if DEBUG
            // SpinWait.SpinUntil(() => Debugger.IsAttached);
#endif

            GenerateEnum(context, "StringNames");
            GenerateEnum(context, "SystemTypes", baseType: "byte");
            GenerateEnum(context, "TaskTypes", baseType: "uint");
            GenerateEnum(context, "GameKeywords", flags: true, baseType: "uint");
            GenerateEnum(context, "HatType", ".Customization", baseType: "uint");
            GenerateEnum(context, "SkinType", ".Customization", baseType: "uint");
            GenerateEnum(context, "PetType", ".Customization", baseType: "uint");
            GenerateEnum(context, "ColorType", ".Customization");
        }
    }
}
