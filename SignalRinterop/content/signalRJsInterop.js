// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

// store connections in object
var connections = {};

Blazor.registerFunction('SignalRinterop.SignalR.Start',
    function (key, hubUrl, callbackAssembly, callbackClass, callbackMethod) {

        // key is the unique key we use to store/retrieve connections

        // set up callback for received messages
        var callback = {
            type: { assembly: callbackAssembly, name: callbackClass },
            method: { name: callbackMethod }
        };

        // create a client
        console.log("Connection being started for " + hubUrl);
        var connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl)
            .build();

        console.log("Connection looks okay");

        connection.on("ReceiveMessage", data => {
            console.log("Connection message received");
            console.log(data);
            // invoke return method
            Blazor.invokeDotNetMethod(callback, "message name here?", data);
        });

        // start the connection
        connection.start();
        // store connection in our lookup object
        connections[key] = connection;
    });


Blazor.registerFunction('SignalRinterop.SignalR.SendMessage', function (key, name, message) {
    console.log("Connection send request: " + name);
    var connection = connections[key];
    if (!connection) throw "Connection not found for " + key;
    console.log("Connection located");
    // send message
    connection.invoke(name, message);
});

Blazor.registerFunction('SignalRinterop.SignalR.Stop', function (key) {
    console.log("Connection stop request: " + key);
    var connection = connections[key];
    if (connection) {
        connection.stop();
        console.log("Connection stopped");
        // remove refs
        delete connections[key];
        connection = null;
    }
    else
        alert("Connection not found for " + key);
});

//Blazor.registerFunction('');
