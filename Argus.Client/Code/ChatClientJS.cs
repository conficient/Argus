using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Argus.Client.Code
{
    /// <summary>
    /// Chat client that uses SignalR
    /// </summary>
    public class ChatClientJS : IDisposable
    {
        public ChatClientJS(string username)
        {
            _username = username;
            _client = new SignalRinterop.SignalRclient("/testhub");
            _client.MessageReceived += MessageReceived;
        }


        private readonly string _username;
        private readonly SignalRinterop.SignalRclient _client;

        /// <summary>
        /// Start the chat client
        /// </summary>
        public void Start()
        {
            _client.Start();
        }

        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            _client.Send("SendMessage", _username, message);
        }

        /// <summary>
        /// handle an inbound message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageReceived(object sender, SignalRinterop.MessageReceivedEventArgs e)
        {
            if (e.Name == "ReceiveMessage")
            {
                var name = (string)e.Data[0];
                var message = (string)e.Data[1];
                // raise event 
                Received?.Invoke(this, new MessageReceivedEventArgs(name, message));
            }
            else
            {
                // todo: not handled
            }
        }

        /// <summary>
        /// Stop the client
        /// </summary>
        public void Stop()
        {
            _client.Stop();
        }

        /// <summary>
        /// Free up resources
        /// </summary>
        public void Dispose()
        {
            _client.Dispose();
        }

        public event MessageReceivedEventHandler Received;
    }

    /// <summary>
    /// Delegate for the message handler
    /// </summary>
    /// <param name="sender">the ChatClientJS instance</param>
    /// <param name="e">Event args</param>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    /// <summary>
    /// Args when message received
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string name, string message)
        {
            Name = name;
            Message = message;
        }

        /// <summary>
        /// Name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public string Message { get; set; }

    }

}
