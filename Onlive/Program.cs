using Microsoft.Extensions.Logging;
using Onlive;
using Onlive.Utils;

var serverIp = args.Length >= 1 ? args[0] : "127.0.0.1";
var serverPort = args.Length >= 2 ? Convert.ToInt32(args[1]) : 8001;

LogControls();

var window = new Window(serverIp, serverPort);

window.Run();

void LogControls()
{
    var controlsWriter = new StringWriter();

    controlsWriter.WriteLine("-- Controls --");
    controlsWriter.WriteLine("[Mouse Left] -- Switch either a single cell, or stashed cells");
    controlsWriter.WriteLine("[Left Shift] -- Toggle Stash mode");
    controlsWriter.WriteLine("[Arrow keys] -- Move around in the map");
    controlsWriter.WriteLine("[Mouse Scroll] -- Zoom-in or Zoom-out");

    Logging.GetLogger<Program>().LogInformation(controlsWriter.ToString());
}