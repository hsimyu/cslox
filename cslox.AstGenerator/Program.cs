﻿using System.Text;

namespace cslox.AstGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: AstGenerator.exe <output directory>");
                Environment.Exit(1);
            }

            string outputDir = args[0];
            var result = EmitAstTree("Expression", new string[]
            {
                "Binary: Expression left, Token op, Expression right"
            });

            var filePath = Path.Join(outputDir, $"Expression.cs");
            Console.WriteLine($"Generate: {filePath}");
            File.WriteAllText(filePath, result);
        }

        static string EmitAstTree(string baseClassName, string[] syntaxList)
        {
            var code = new StringBuilder();

            code.AppendLine($"/* This is auto-generated code. Do not edit. */");
            code.AppendLine("namespace cslox {");

            EmitLine(code, 1, $"abstract class {baseClassName}");
            EmitLine(code, 1, $"{{");
            EmitLine(code, 1, $"}}");
            code.AppendLine();

            /* 
            "Binary: Expression left, Token op, Expression right"

            =>

            class Binary : Expression
            {
                Binary(Expression left, Token op, Expression right)
                {
                    this.left = left;
                    this.op = op;
                    this.right = right;
                }

                Expression left;
                Token op;
                Expression right;
            }
            */

            foreach (var syntax in syntaxList)
            {
                var splitted = syntax.Split(':');
                var expressionName = splitted[0].Trim();
                var fields = splitted[1].Split(',').Select(x => x.Trim());

                EmitLine(code, 1, $"internal class {expressionName} : {baseClassName}");
                EmitLine(code, 1, $"{{");

                // constructor
                EmitLine(code, 2, $"internal {expressionName}({splitted[1].Trim()})");
                EmitLine(code, 2, $"{{");
                foreach(var f in fields)
                {
                    var sub = f.Split(' ');
                    var fieldName = sub[1].Trim();
                    EmitLine(code, 3, $"this.{fieldName} = {fieldName};");
                }

                EmitLine(code, 2, $"}}");

                // members
                foreach(var f in fields)
                {
                    var sub = f.Split(' ');
                    var fieldType = sub[0].Trim();
                    var fieldName = sub[1].Trim();
                    EmitLine(code, 2, $"{fieldType} {fieldName};");
                }

                EmitLine(code, 1, $"}}");
            }

            code.AppendLine("} // namespace cslox");
            return code.ToString();
        }

        static void EmitLine(StringBuilder builder, int level, string content)
        {
            switch (level)
            {
                case 0:
                    builder.AppendLine($"{content}");
                    return;
                case 1:
                    builder.AppendLine($"    {content}");
                    return;
                case 2:
                    builder.AppendLine($"        {content}");
                    return;
                case 3:
                    builder.AppendLine($"            {content}");
                    return;
                default:
                    Console.Error.WriteLine($"Unsupported indent level: {level}");
                    builder.AppendLine($"{content}");
                    return;
            }
        }
    }
}