using PrimS.Telnet;

namespace SourceLabelMaker
{
    internal class TelnetClient
    {
        private string _ip;
        private Client _client;

        internal TelnetClient(string ip)
        {
            _ip = ip;
        }

        internal bool Connect()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            try { _client = new Client(_ip, 23, cancellationTokenSource.Token); } catch (Exception ex) { return false; }

            Console.WriteLine("Connected!");
            return true;
        }

        internal void ExecuteCommand(string command)
        {
            _client.WriteLineAsync(command + "\n\r");
        }

        internal async Task<string> RetrieveResponse()
        {
            string response = await _client.ReadAsync(TimeSpan.FromMilliseconds(500));
            return response;
        }

        
    }
}
