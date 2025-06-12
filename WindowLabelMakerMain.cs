using System.Net;
using Newtonsoft.Json.Linq;

namespace SourceLabelMaker
{
    internal class WindowLabelMakerMain
    {
        private static TelnetClient _telnetClient;
        private static NetAPIClient _netAPIClient;
        private static async Task Main(string[] args)
        {
            Console.Clear();
            Console.Write("Enter IP of display wall: ");
            string ip = Console.ReadLine();

            if (!IPAddress.TryParse(ip, out _))
            {
                Thread.Sleep(2500);
                Main(args);
            }

            _telnetClient = new TelnetClient(ip);
            if (_telnetClient.Connect())
            {
                _netAPIClient = new NetAPIClient(_telnetClient);
                await Run();
            }
        }

        private static async Task Run()
        {
            Console.Clear();
            Console.WriteLine("Enter \"1\": AddSourceTextOverlay\nEnter \"2\": ClearSourceOverlay\nEnter \"3\": Edit Config\nEnter \"q\": Quit\n");
            string commandChoiceInput = Console.ReadLine();
            if (!int.TryParse(commandChoiceInput, out int commandChoice))
            {
                if (commandChoiceInput.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    Environment.Exit(0);
                }
                await Run();
            }

            if (commandChoice < 1 || commandChoice > 3) await Run();

            switch (commandChoice)
            {
                case 1:
                    Console.WriteLine("Adding text overlays...");
                    await _netAPIClient.AddSourceTextOverlay();
                    await Run();
                    break;
                case 2:
                    Console.WriteLine("Removing text overlays...");
                    await _netAPIClient.ClearSourceOverlay();
                    await Run();
                    break;
                case 3:
                    Console.Clear();
                    EditConfig();
                    await Run();
                    break;
            }
        }

        private static void EditConfig()
        {
            JObject currentConfig = NetAPIClient.SourceTextOverlayConfig;
            Console.WriteLine("Enter digit of option to change: ");
            int i = 1;
            foreach (var x in currentConfig)
            {
                Console.WriteLine($"{i}: {x.Key} | Current Value: {x.Value}");
                i++;
            }
            Console.WriteLine($"q: Go back\n");
            string configChoiceInput = Console.ReadLine();
            if (int.TryParse(configChoiceInput, out int configChoice))
            {
                if (configChoiceInput.Equals("q"))
                {
                    return;
                }

                if (configChoice < 1 || configChoice > currentConfig.Count)
                {
                    EditConfig();
                }

                string configNewValue = null;
                do
                {
                    Console.WriteLine("\nEnter new value for selected option: ");
                    configNewValue = Console.ReadLine();
                } while (string.IsNullOrEmpty(configNewValue));

                if(Validate(configChoice, configNewValue))
                {
                    SaveConfig();
                    Console.Clear();
                    EditConfig();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("\nValue entered invalid...\n");
                    EditConfig();
                }
            }
        }

        private static bool Validate(int configChoice, string configNewValue)
        {
            bool validated = false;
            switch (configChoice)
            {
                case 1:
                    if (ConfigValidation.ValidateAlignment(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 2:
                case 4:
                    if (ConfigValidation.ValidateColor(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 3:
                    if (ConfigValidation.ValidateBackgroundMode(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 5:
                    if (ConfigValidation.ValidateFaceName(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 6:
                    if (ConfigValidation.ValidateFontSize(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 7:
                    if (ConfigValidation.ValidateProportionalMode(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 8:
                    if (ConfigValidation.ValidateScrollSize(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 9:
                    if (ConfigValidation.ValidateBlinking(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
                case 10:
                    if (ConfigValidation.ValidateBlendingType(configNewValue))
                    {
                        NetAPIClient.SourceTextOverlayConfig.Properties().ElementAt(configChoice - 1).Value = configNewValue;
                        validated = true;
                    }
                    break;
            }
            return validated;
        }

        private static void SaveConfig()
        {
            File.WriteAllText(NetAPIClient.ConfigFile, NetAPIClient.SourceTextOverlayConfig.ToString());
            Console.Clear();
        }
    }
}