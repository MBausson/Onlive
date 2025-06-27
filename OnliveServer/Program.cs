using OnliveServer;

var serverIp = args.Length >= 1 ? args[0] : "127.0.0.1";
var serverPort = args.Length >= 2 ? Convert.ToInt32(args[1]) : 8001;

var server = new Server(serverIp, serverPort);

await server.StartAsync();
