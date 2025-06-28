using OnliveServer;
using OnliveServer.Utils;

ServerConfiguration.SetFromCliArguments(args);

var server = new Server();

await server.StartAsync();