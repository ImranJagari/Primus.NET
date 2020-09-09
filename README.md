# Primus.NET
Rewrite of primus server in .NET Core

Original repository : https://github.com/primus/primus

## How to use Primus.NET in a ASP.Net Core project

1) First of all, add a reference of the lib to your ASP.Net Core project.

2) Add this line in your `ConfigureServices` method in `Startup.cs`.  It permits to load the native routes of primus inside the lib
```csharp
services.AddMvc().AddApplicationPart(typeof(PrimusController).Assembly).AddControllersAsServices();
```

3) Add this line in your `Configure` method in `Startup.cs`. for loading the handlers which is for the data received from the websocket in
```csharp
MessageParser.Initialize();
```

4) Add those lines in your `Configure` method in `Startup.cs` above `app.UseRouting();`. for initiating the websocket manager from ASP.Net Core
```csharp
app.UseWebSockets(new WebSocketOptions()
{
    //KeepAliveInterval is not necessary, can be removed or modified
    KeepAliveInterval = TimeSpan.FromSeconds(120)
});
```

5) Add those lines in your `Configure` method in `Startup.cs` above `app.UseRouting();` and after `app.UseWebSockets();`. to handle a WebsocketRequest because the communication is initiating in a Task
```csharp
///--------------- Handle 101 http response on task ----------------------///
app.UseMiddleware<WebSocketMiddleware>();
///--------------- Handle 101 http response on task ----------------------///
```

6) Enjoy !

## Events

### OnClientCreated

In the `ClientManager.cs` class there is a static event called OnClientCreated, you can add your method to the event to get all `PrimusClient` object created.

### OnClientDisconnected

In the `ClientManager.cs` class there is a static event called OnClientDisconnected, you can add your method to the event to get all `PrimusClient` object disconnected.

### DataReceived

In the `PrimusClient.cs` class there is an event to handle the data received from the webSocket per client.
