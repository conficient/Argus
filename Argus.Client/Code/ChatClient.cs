using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Argus.Client.Code
{
    public class ChatClient : IDisposable
    {
        public ChatClient(string serverUrl, string name)
        {
            _serverUrl = serverUrl.TrimEnd('/');
            _name = name;
        }

        private readonly string _serverUrl;
        private readonly string _name;
        private HubConnection _connection;

        public event MessageReceivedEventHandler MessageReceived;

        public async Task Start()
        {
            if (_connection != null)
                throw new InvalidOperationException("Connection already started");

            string hubUrl = $"{_serverUrl}/TestHub";
            // connect to the SignalR hub 
            _connection = new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .Build();

            // handle incoming messages:
            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                // raise an event to let the owner know
                Console.WriteLine($"[from {user}]: {message}");
                // TODO: raise event
                MessageReceived(this, new MessageReceivedEventArgs(user, message));
            });


            //Start connection
            try
            {
                await _connection.StartAsync();
                Console.WriteLine("Connection started");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                throw;
            }
            finally
            {
                await _connection.DisposeAsync();
            }
        }

        public async Task SendMessage(string message)
        {
            // send a test message
            await _connection.InvokeAsync("SendMessage", _name, message);

        }

        public async Task Stop()
        {
            await _connection?.StopAsync();
            await _connection?.DisposeAsync();
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                await Stop();
            });
        }
    }

    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    public class MessageReceivedEventArgs :EventArgs
    {
        public MessageReceivedEventArgs(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public string Name { get; set; }
        public string Message { get; set; }
    }
}
