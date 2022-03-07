using System.Diagnostics;
using System.IO.Ports;

ConsoleApp.Run(args, 
(
    [Option("p", "Port name connected.")] string port
) =>
{
    var serialPort = new SerialPort(port, 9600);
    serialPort.Open();
    var stopwatch = new Stopwatch();

    using (var app = new CCTimerExtender())
    {
        // Ctrl+C で強制終了された際にCCTimerを解放する
        Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) => 
        {
            serialPort.Close();
            serialPort.Dispose();
            app.Dispose();
        };
        
        app.OnCountUpTimerStart += (object? sender, EventArgs e) =>
        {
            serialPort.Up();
            stopwatch.Start();
        };
        app.OnCountUpTimerStop += (object? sender, EventArgs e) =>
        {
            serialPort.Up();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
        };
        
        do
        {
            Console.WriteLine("Enter to press down...");
            Console.ReadLine();
            serialPort.Down();
        } while (!app.HasExited);
    }

    serialPort.Close();
    serialPort.Dispose();
});

public static class SerialPortExtension
{
    public static void Down(this SerialPort serialPort) { serialPort.Write("d"); }
    public static void Up(this SerialPort serialPort) { serialPort.Write("u"); }
}
