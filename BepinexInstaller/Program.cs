using BepinexInstaller;

public class Program
{
    public static string GAME_PATH = Utilities.GetInstalledPath();

    static Dictionary<string, Action> commands = new Dictionary<string, Action>
    {
        { "--help", PrintHelp },
        { "--version", () => Console.WriteLine("BepInStaller 0.1.0") },
        { "--install", () => Utilities.InstallBepInEx().Wait() },
        { "--uninstall", () => Utilities.UninstallBepInEx() },
        { "--list", () => Console.WriteLine(string.Join(Environment.NewLine, Utilities.GetPlugins())) }
    };

    static void Main(string[] args)
    {
        Console.Title = "BepInStaller 0.1.0 - by Kante";

        while (true)
        {
            PrintHelp();

            Console.Write("> ");
            var input = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(input)) continue;

            if (input.ToUpper() == "EXIT") break;

            if (commands.TryGetValue(input, out var command))
                command.Invoke();
            else
                Console.WriteLine("Unknown command. Use --help to see available commands.");

            Console.ReadKey();

            Console.Clear();
        }
    }


    static void PrintHelp()
    {
        Console.WriteLine("Usage: --command");
        Console.WriteLine("Options:");
        Console.WriteLine("--help       Show the help message");
        Console.WriteLine("--version    Show the version you're running");
        Console.WriteLine("--install    Install BepInEx");
        Console.WriteLine("--uninstall  Uninstall BepInEx");
        Console.WriteLine("--list       List installed plugins");
    }
}
