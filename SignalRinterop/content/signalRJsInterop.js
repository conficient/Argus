// This file is to show how a library package may provide JavaScript interop features
// wrapped in a .NET API

// import * as SignalR from 'signalr'; // we will use static loading?

// store connection 
var connection;

Blazor.registerFunction('SignalRinterop.SignalR.Start', function (callbackAssembly, callbackClass, callbackMethod) {

    // set up callback for received messages
    var callback = {
        type: { assembly: callbackAssembly, name: callbackClass },
        method: { name: callbackMethod }
    };

    // create a client
    console.log("Connection being started");
    connection = new signalR.HubConnectionBuilder()
        .withUrl("/testhub")
        .build();
    console.log("Connection looks okay");

    connection.on("ReceiveMessage", data => {
        console.log("Connection message received");
        console.log(data);
        // invoke return method
        Blazor.invokeDotNetMethod(callback, "user", data);
    });

    connection.start()
        .then(() => connection.invoke("send", "Hello"));

});


Blazor.registerFunction('SignalRinterop.SignalR.SendMessage', function (user, message) {
    console.log("Connection send request");
    if (connection)
        connection.invoke("SendMessage", user, message);
    else
        alert("Connection is not started");
});

Blazor.registerFunction('SignalRinterop.SignalR.Stop', function () {
        console.log("Connection stop request");
    if (connection) {
        console.log("Connection stopped");
        connection.stop();
        connection = null;
    }
    else
        alert("Connection is not started");
});

//Blazor.registerFunction('');
