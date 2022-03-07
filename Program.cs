using System.Diagnostics;
using System.IO.Ports;

var serialPort = new SerialPort("COM6", 9600);
serialPort.Open();
var stopwatch = new Stopwatch();

using (var app = new CCTimerExtender())
{
    // Ctrl+C で強制終了された際にCCTimerを解放する
    Console.CancelKeyPress += ((object? sender, ConsoleCancelEventArgs e) => 
    {
        serialPort.Up();
        serialPort.Close();
        serialPort.Dispose();
        
        app.Dispose();
    });
    
    app.OnCountUpTimerStart += OnStart;
    app.OnCountUpTimerStop += OnStop;
    
    do
    {
        Console.WriteLine("Enter to press down...");
        Console.ReadLine();
        serialPort.Down();
    } while (!app.HasExited);
}

serialPort.Close();
serialPort.Dispose();

void OnStart(object? sender, EventArgs e)
{
    serialPort.Up();

    stopwatch.Start();
}
void OnStop(object? sender, EventArgs e)
{
    serialPort.Up();

    stopwatch.Stop();
    Console.WriteLine(stopwatch.ElapsedMilliseconds);
    stopwatch.Reset();
}
