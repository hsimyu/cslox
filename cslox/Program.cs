using System.Text;

namespace cslox
{
    internal class Program
    {
        static bool hadError = false;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: cslox [script/file/path.cslox]");
                Environment.Exit(1);
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        static void runFile(string filename)
        {
            // TODO: 存在チェック
            var script = File.ReadAllText(filename);
            runImpl(script);

            if (hadError) Environment.Exit(1);
        }

        static void runPrompt()
        {
            // 入力を受け取る
            var input = Console.In;

            while (true)
            {
                Console.Write("> ");
                var line = input.ReadLine();
                if (line == null)
                    break;
                runImpl(line);
                hadError = false;
            }
        }

        static void runImpl(string script)
        {
            var scanner = new Scanner(script);
            var tokens = scanner.scanTokens();

            var parser = new Parser(tokens);
            Expression? expr = parser.parse();

            // 構文エラーがあれば停止
            if (hadError || expr == null) return;

            Console.WriteLine(new AstPrinter().print(expr));
        }

        public static void error(int line, string message)
        {
            errorAt(line, "", message);
        }

        public static void error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                errorAt(token.line, " at end", message);
            }
            else
            {
                errorAt(token.line, $" at '{token.lexeme}'", message);
            }
        }

        public static void errorAt(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }
    }
}