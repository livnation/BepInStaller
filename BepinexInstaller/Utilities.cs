using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.Net.Http.Headers;

namespace BepinexInstaller
{
    internal class Utilities
    {
        public static void UninstallBepInEx()
        {
            if (!IsBepinexInstalled())
            {
                Console.WriteLine("BepInEx not found.");
                return;
            }

            Directory.Delete($@"{Program.GAME_PATH}\BepInEx", true);

            Console.WriteLine("BepInEx uninstalled successfully.");
        }

        public static async Task InstallBepInEx()
        {
            if (IsBepinexInstalled())
            {
                Console.WriteLine("BepInEx is already installed.");
                return;
            }

            // YES i used GPT for this

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.Add(
                    new ProductInfoHeaderValue("BepInStaller", "0.1.0")); // Doesnt work without

                string url = $"https://api.github.com/repos/BepInEx/BepInEx/releases/latest";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                JObject release = JObject.Parse(json);

                var assets = release["assets"];

                var targ = release["assets"].FirstOrDefault(a =>
                    a["name"].ToString().StartsWith("BepInEx_win_x64", StringComparison.OrdinalIgnoreCase) == true);

                if (targ == null) return;

                string downloadUrl = targ["browser_download_url"].ToString();

                Console.WriteLine($"Downloading {targ["name"].ToString()}...");
                byte[] fileBytes = await client.GetByteArrayAsync(downloadUrl);

                string downloadDir = Path.Combine("downloads");
                Directory.CreateDirectory(downloadDir);
                string zipPath = Path.Combine(downloadDir, targ["name"].ToString());
                await File.WriteAllBytesAsync(zipPath, fileBytes);
  
                if (Path.GetExtension(zipPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    ZipFile.ExtractToDirectory(zipPath, $"{Program.GAME_PATH}", overwriteFiles: true);
                    Directory.CreateDirectory($@"{Program.GAME_PATH}\BepInEx\plugins");
                    Console.WriteLine($"Installed to: {$"{Program.GAME_PATH}"}");
                }
            }
        }

        public static List<string> GetPlugins()
        {
            List<string> plugins = new List<string>();

            if (!IsBepinexInstalled())
            {
                plugins.Add("BepInEx is not installed.");

                return plugins;
            }

            foreach (string file in Directory.GetFiles($@"{Program.GAME_PATH}\BepInEx\plugins", "*.dll", SearchOption.AllDirectories))
            {
                plugins.Add(Path.GetFileName(file));
            }

            return plugins;
        }

        public static bool IsBepinexInstalled()
        {
            return Directory.Exists($@"{Program.GAME_PATH}\BepInEx");
        }

        public static string GetInstalledPath()
        {
            string INITIAL_PATH = @"Program Files (x86)\Steam\steamapps\common\Gorilla Tag";

            foreach (string drive in Environment.GetLogicalDrives())
            {
                if (Directory.Exists($"{drive}{INITIAL_PATH}"))
                {
                    return $"{drive}{INITIAL_PATH}";
                }
            }

            return string.Empty;
        }
    }
}
