using System;
using Microsoft.AspNetCore.Blazor.Browser.Interop;

namespace SignalRinterop
{
    public class SignalR
    {

        public static void Start(string user)
        {
            const string assm = "SignalRinterop.dll";
            const string cls = "SignalRinterop.SignalR";
            const string fn = "SendMessage";
            var tmp = RegisteredFunction.Invoke<bool>("SignalRinterop.SignalR.SendMessage", user);
        }
        public static void SendMessage(string user, string message)
        {
            RegisteredFunction.Invoke<bool>("SignalRinterop.SignalR.SendMessage", user, message);
        }
    }

    public class SignalRclient
    {
        public SignalRclient(string hubUrl)
        {

        }

        private string _clientKey;

        public void Start()
        {

            ReceiveMessage += (object sender, EventArgs e)=> { };
        }

        public EventHandler ReceiveMessage { get; set; }

    }
}
