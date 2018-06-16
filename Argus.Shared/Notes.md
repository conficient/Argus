# How it works

The server runs on IIS with ASP.NET Core and creates a SignalR hub.

Users go to the website and can see a list of targets they can monitor. They put the targets onto a 
dashboard where they can see the status of the targets, which is updated from SignalR.

## Ping Source

Our basic example will be a "Ping" target that shows whether a target is alive and responding. 

The source is a Ping event source that sends an update to the server at a stated interval to say it's alive.

It runs from a console app, and we set it's name, tell it the server to talk to and give it a password. It then tries to