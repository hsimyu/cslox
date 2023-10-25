using System.Text;

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
            EmitAstTree(outputDir, "Expression", new string[]
            {
                "Binary: Expression left, Token op, Expression right",
                "Ternary: Expression cond, Token op, Expression first, Expression second",
                "Grouping: Expression exp",
                "Literal: object? value",
                "Unary: Token op, Expression right",
                "Variable: Token name",
            });

            EmitAstTree(outputDir, "Stmt", new string[]
            {
                "ExpressionStmt: Expression expression",
                "PrintStmt: Expression expression",
                "VarStmt: Token name, Expression? initializer",
            });
        }

        static void EmitAstTree(string outputDir, string baseClassName, string[] syntaxList)
        {
            var code = new StringBuilder();

            code.AppendLine($"/* This is auto-generated code. Do not edit. */");
            code.AppendLine("namespace cslox");
            code.AppendLine("{");
            code.AppendLine();

            /*
             public abstract class Expression
             {
                  public interface IVisitor<R>
                  {
                      R visitBinaryExpression(Binary expression);
                      R visitGroupingExpression(Grouping expression);
                      R visitLiteralExpression(Literal expression);
                      R visitUnaryExpression(Unary expression);
                  }
                  public abstract R accept<R>(IVisitor<R> visitor);

                  ...
             }
             */
            EmitLine(code, 1, $"public abstract class {baseClassName}");
            EmitLine(code, 1, $"{{");

                EmitLine(code, 2, $"public interface IVisitor<R>");
                EmitLine(code, 2, $"{{");

                    foreach (var syntax in syntaxList)
                    {
                        var splitted = syntax.Split(':');
                        var expressionName = splitted[0].Trim();
                        EmitLine(code, 3, $"R visit{expressionName}({expressionName} {baseClassName.ToLower()});");
                    }

                EmitLine(code, 2, $"}}");
                EmitLine(code, 2, $"public abstract R accept<R>(IVisitor<R> visitor);");

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

                public override R accept<R>(IVisitor<R> visitor)
                {
                    return visitor.visitBinary(this);
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

                EmitLine(code, 2, $"public class {expressionName} : {baseClassName}");
                EmitLine(code, 2, $"{{");

                    // constructor
                    EmitLine(code, 3, $"internal {expressionName}({splitted[1].Trim()})");
                    EmitLine(code, 3, $"{{");

                        foreach(var f in fields)
                        {
                            var sub = f.Split(' ');
                            var fieldName = sub[1].Trim();
                            EmitLine(code, 4, $"this.{fieldName} = {fieldName};");
                        }

                    EmitLine(code, 3, $"}}");

                    EmitLine(code, 3, $"public override R accept<R>(IVisitor<R> visitor)");
                    EmitLine(code, 3, $"{{");
                        EmitLine(code, 4, $"return visitor.visit{expressionName}(this);");
                    EmitLine(code, 3, $"}}");

                    // members
                    foreach(var f in fields)
                    {
                        var sub = f.Split(' ');
                        var fieldType = sub[0].Trim();
                        var fieldName = sub[1].Trim();
                        EmitLine(code, 3, $"public {fieldType} {fieldName};");
                    }

                EmitLine(code, 2, $"}}");
                code.AppendLine();
            }

            EmitLine(code, 1, $"}}");
            code.AppendLine("} // namespace cslox");

            var result = code.ToString();
            var filePath = Path.Join(outputDir, $"{baseClassName}.cs");
            Console.WriteLine($"Generate: {filePath}");
            File.WriteAllText(filePath, result);
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
                case 4:
                    builder.AppendLine($"                {content}");
                    return;
                default:
                    Console.Error.WriteLine($"Unsupported indent level: {level}");
                    builder.AppendLine($"{content}");
                    return;
            }
        }
    }
}