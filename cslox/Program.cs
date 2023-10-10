using System.Text;

namespace cslox
{
    internal class Program
    {
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
            }

            Console.WriteLine("END;");
        }

        static void runImpl(string script)
        {
            Console.WriteLine(script);
        }
    }
}