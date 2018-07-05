// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

// store connections in object
var connections = {};

Blazor.registerFunction('SignalRinterop.SignalR.Start',
    function (key, hubUrl, callbackAssembly, callbackClass, callbackMethod) {
        // key is the unique key we use to store/retrieve connections
        console.log("Connection start");

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

        console.log("Connection looks okay, adding receive");
        
        // add handler for this
        connection.on("ReceiveMessage", (...args) => {
            console.log("Connection message received for " + key);
            console.log(args);
            // invoke return method
            Blazor.invokeDotNetMethod(callback, key, "ReceiveMessage", args);
        });

        // start the connection
        connection.start();
        // store connection in our lookup object
        connections[key] = connection;
    });


Blazor.registerFunction('SignalRinterop.SignalR.Send', function (key, method, ...args) {
    console.log("Connection send request: " + method);
    console.log(args);
    var connection = connections[key];
    if (!connection) throw "Connection not found for " + key;
    console.log("Connection located");
    // send message
    connection.invoke(method, args[0], args[1]);
    // dummy
    return "ok";
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
        console.log("Connection not found for " + key);
});

