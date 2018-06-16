using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace PingSource
{
    class Program
    {

        private static string _serverUrl;

        private static string _clientName;

        static async Task Main(string[] args)
        {
            Console.WriteLine("PingSource console app");

            if(args.Length != 2)
            {
                Console.WriteLine("Invalid arguments. Usage:");
                Console.WriteLine("pingsource.exe [serverURL] [clientName]");
                Console.ReadKey();
                return;
            }

            // save args
            _serverUrl = args[0].TrimEnd('/');
            _clientName = args[1];

            Console.WriteLine("(waiting 3 seconds for server to load)");
            await Task.Delay(3000);

            try
            {
                await RunSignalRasync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: {e.Message}");
            }
            Console.WriteLine("\n-- press any key --");
            Console.ReadKey();
        }

        private static async Task RunSignalRasync()
        {

            string hubUrl = $"{_serverUrl}/TestHub";
            // register the source with the SignalR hub and start sending a ping out
            // at the set interval
            var connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            // handle incoming messages:
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"[from {user}]: {message}");
            });

            //Start connection
            try
            {
                Console.WriteLine("Connection starting...");
                await connection.StartAsync();
                Console.WriteLine("Connection started");
                Console.WriteLine("\nType a message or just a blank line to stop");
                do
                {
                    var input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                        break;
                    // send a test message
                    await connection.InvokeAsync("SendMessage", _clientName, input);

                } while (true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                await connection.DisposeAsync();
            }

            Console.WriteLine("Connection closed");
        }
    }
}
