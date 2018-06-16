using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace PingSource
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("This is the ping source console app");
            Console.WriteLine("waiting 3 seconds for server to load");

            await Task.Delay(3000);
            Console.WriteLine("starting...");

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
            const string hubUrl = "http://localhost:25869/TestHub";
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
                await connection.StartAsync();
                Console.WriteLine("Connection started");
                Console.WriteLine("Type a message or hit enter to stop");
                do
                {
                    Console.Write("message: ");
                    var input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                        break;
                    // send a test message
                    await connection.InvokeAsync("SendMessage", "consoleapp", input);

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

            Console.WriteLine("Connection closed - press any key");
            Console.ReadKey();
        }
    }
}
