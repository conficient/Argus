﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Blazor.Browser.Interop;

namespace SignalRinterop
{

    /// <summary>
    /// Generic client class that interfaces .NET Standard/Blazor with SignalR Javascript client
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public class SignalRclient : IDisposable
    {

        #region static methods
        
        /// <summary>
        /// internal dictionary of SignalRclients by Key
        /// </summary>
        private static Dictionary<string, SignalRclient> _clients = new Dictionary<string, SignalRclient>();


        /// <summary>
        /// Receive a message from a client
        /// </summary>
        /// <param name="key"></param>
        /// <param name="message">*might need to be a params here?*</param>
        public static void ReceiveMessage(string key, string name, object message)
        {
            if (_clients.ContainsKey(key))
            {
                var client = _clients[key];
                client.HandleMessage(name, message);
            } else
            {
                // unable to match the message to a client
            }
        }

        #endregion

        /// <summary>
        /// Ctor: create a new client for the given hub URL
        /// </summary>
        /// <param name="hubUrl"></param>
        public SignalRclient(string hubUrl)
        {
            // save the hub url
            _hubUrl = hubUrl ?? throw new ArgumentNullException(nameof(hubUrl));
            // create a unique key for this client
            _key = new Guid().ToString();
            // add myself to the list of clients
            _clients.Add(_key, this);
        }

        /// <summary>
        /// The Hub URL for this client
        /// </summary>
        private readonly string _hubUrl;

        /// <summary>
        /// Our unique key for this client instance
        /// </summary>
        /// <remarks>
        /// We cannot pass JS objects to Blazor/C# so we use a unique key
        /// to reference each instance. The JS client will store the object
        /// under our key so we can reference it
        /// </remarks>
        private readonly string _key;

        /// <summary>
        /// Flag to show if started
        /// </summary>
        private bool _started = false;

        /// <summary>
        /// Start the SignalR client on JS
        /// </summary>
        public void Start()
        {
            if (!_started)
            {
                const string callbackAssembly = "SignalRinterop.dll";
                const string callbackClass = "SignalRinterop.SignalRclient";
                const string callbackMethod = "ReceiveMessage";
                // invoke the JS interop start client method
                var tmp = RegisteredFunction.Invoke<bool>("SignalRinterop.SignalR.Start", _key, _hubUrl,
                    callbackAssembly, callbackClass, callbackMethod);
                _started = true;
            }
        }
        
        /// <summary>
        /// Handle an inbound message from a hub
        /// </summary>
        /// <param name="name">event name</param>
        /// <param name="message">message content</param>
        private void HandleMessage(string name, object message)
        {
            // raise an event to subscribers
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(name, message));
        }

        /// <summary>
        /// Event that is raised when this client receives a message
        /// </summary>
        /// <remarks>
        /// Consuming classes should subscribe to this event
        /// </remarks>
        public event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// Send a message to the hub
        /// </summary>
        /// <param name="name">Method to call on the hub</param>
        /// <param name="message">[optional] message body</param>
        public void SendMessage(string name, object message = null)
        {
            // check we are connected
            if (!_started)
                throw new InvalidOperationException("Client not started");
            // send the message
            var tmp = RegisteredFunction.Invoke<bool>("SignalRinterop.SignalR.SendMessage", _key, name, message);
        }

        /// <summary>
        /// Stop the client (if started)
        /// </summary>
        public void Stop()
        {
            if (_started)
            {
                // disconnect the client
                var tmp = RegisteredFunction.Invoke<bool>("SignalRinterop.SignalR.Stop", _key);
                _started = false;
            }
        }

        /// <summary>
        /// Dispose of client
        /// </summary>
        public void Dispose()
        {
            // ensure we stop if connected
            Stop();

            // remove this key from the list of clients
            if (_clients.ContainsKey(_key))
                _clients.Remove(_key);
        }
    }

    /// <summary>
    /// Delegate for the message handler
    /// </summary>
    /// <param name="sender">the SignalRclient instance</param>
    /// <param name="e">Event args</param>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    /// <summary>
    /// Args when message received
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(string name, object message)
        {
            Name = name;
            Message = message;
        }

        /// <summary>
        /// Name of the message/event
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Message body
        /// </summary>
        public object Message { get; set; }

    }

}
