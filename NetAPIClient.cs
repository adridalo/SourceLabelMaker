using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace SourceLabelMaker
{
    internal class NetAPIClient
    {
        private TelnetClient _telnetClient;
        internal static JObject SourceTextOverlayConfig;
        internal static string ConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WindowLabelMaker", "SourceTextOverlayConfig.json");

        internal NetAPIClient(TelnetClient client)
        {
            _telnetClient = client;

            string configDir =  Path.GetDirectoryName(ConfigFile);
            if(!Directory.Exists(configDir)) 
                Directory.CreateDirectory(configDir);

            if(!File.Exists(ConfigFile))
            {
                var defaultPath = Path.Combine(AppContext.BaseDirectory, "SourceTextOverlayConfig.json");
                if(File.Exists(defaultPath)) 
                    File.Copy(defaultPath, ConfigFile);
            }

            var json =  File.ReadAllText(ConfigFile);
            SourceTextOverlayConfig =  JObject.Parse(json);
        }

        internal async Task AddSourceTextOverlay()
        {
            string currentLayout = await GetCurrentLayout();
            var sourcesList = await GetSourcesLists();
            for(int i = 0; i < sourcesList.Count; i++)
            {
                string currentSource = sourcesList[i];
                string commandString = GetCommandString(currentLayout, currentSource);
                _telnetClient.ExecuteCommand(commandString);
                Thread.Sleep(250);
            }
        }

        internal async Task ClearSourceOverlay()
        {
            var sourcesList = await GetSourcesLists();
            for (int i = 0; i < sourcesList.Count; i++)
            {
                string currentSource = sourcesList[i];
                _telnetClient.ExecuteCommand($"ClearSourceOverlay \"{currentSource}\"");
                if(_telnetClient.RetrieveResponse().Equals("Ok")) continue;
            }
        }

        private string GetCommandString(string currentLayout, string currentSource)
        {
            return $"AddSourceTextOverlay \"{currentSource}\" \"{currentSource}\" " +
                $"/A:{SourceTextOverlayConfig["Alignment"]} " +
                $"/C:{SourceTextOverlayConfig["TextColor"]} " +
                $"/BM:{SourceTextOverlayConfig["BackgroundMode"]} " +
                $"/BC:{SourceTextOverlayConfig["BackgroundColor"]} " +
                $"/F:\"{SourceTextOverlayConfig["FaceName"]}\" " +
                $"/FS:{SourceTextOverlayConfig["FontSize"]} " +
                $"/P:{SourceTextOverlayConfig["ProportionalMode"]} " +
                $"/S:{SourceTextOverlayConfig["ScrollSpeed"]} " +
                $"/BL:{SourceTextOverlayConfig["Blinking"]} " +
                $"/BT:{SourceTextOverlayConfig["BlendingType"]} ";
            //$"/DCK:{_sourceTextOverlayConfig["DestinationColorKey"]} " +
            //$"/BA:{_sourceTextOverlayConfig["BlendingAlpha"]} ";
        }

        private async Task<List<string>> GetSourcesLists()
        {
            string currentLayout = await GetCurrentLayout();
            int windowCount = await GetWindowsByLayout(currentLayout);
            List<string> sourcesList = new List<string>();

            for (int i = 0; i < windowCount; i++)
            {
                _telnetClient.ExecuteCommand($"window \"{currentLayout}\" {i}");
                string windowInfo = await _telnetClient.RetrieveResponse();
                string sourceName = GetSourceByWindowInfo(windowInfo);
                sourcesList.Add(sourceName);
            }
            return sourcesList;
        }

        private string GetSourceByWindowInfo(string windowInfo)
        {
            string source = String.Empty;
            foreach(var line in windowInfo.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if(line.StartsWith("Source:"))
                {
                    source = line.Split(":")[1].Trim();
                    return source;
                }
            }
            return string.Empty;
        }

        private async Task<int> GetWindowsByLayout(string layout)
        {
            _telnetClient.ExecuteCommand($"window \"{layout}\"");
            string windowsCountResponse = await _telnetClient.RetrieveResponse();

            var match = Regex.Match(windowsCountResponse, @"-(\d+)");
            if(match.Success)
            {
                int windowsCount = int.Parse(match.Groups[1].Value.Trim()) + 1;
                return windowsCount;
            }
            return -1;
        }

        private async Task<string> GetCurrentLayout()
        {
            _telnetClient.ExecuteCommand("status");
            string status = await _telnetClient.RetrieveResponse();
            string currentLayout = "";

            foreach (var line in status.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.StartsWith("CurrentLayout:"))
                {
                    currentLayout = line.Split(":")[1].Trim();
                    return currentLayout;
                }
            }

            return String.Empty;
        }
    }
}
