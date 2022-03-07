using System.Diagnostics;
using System.IO.Ports;

/// <summary>
/// CCTimerの計測区間と3DSを同期させる
/// 
/// Enter入力でArduinoのアームを3DSのタッチパネルに接触させておき、
/// CountUpTimerの開始時と停止時(Cancel|Submitによるカウントダウン終了)に、タッチパネルから離す
/// </summary>
/// <param name="port">シリアルポート名</param>
ConsoleApp.Run(args, 
(
    [Option("p", "Port name connected.")] string port
) =>
{
    var serialPort = new SerialPort(port, 9600);
    serialPort.Open();
    var stopwatch = new Stopwatch();

    // {basedir}/CCTimer/CCTimer.exe
    using (var app = new CCTimerExtender())
    {
        // Ctrl+C で強制終了された際にCCTimerとSerialPortを解放する
        Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs e) => 
        {
            serialPort.Close();
            serialPort.Dispose();
            app.Dispose();
        };
        
        // 開始時と終了時にタッチパネルからアームを離す
        app.OnCountUpTimerStart += (object? sender, EventArgs e) =>
        {
            serialPort.PullUp();
            stopwatch.Start();
        };
        app.OnCountUpTimerStop += (object? sender, EventArgs e) =>
        {
            serialPort.PullUp();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            stopwatch.Reset();
        };
        
        do
        {
            // Enter入力の度にタッチパネルを押下する
            Console.WriteLine("Enter to press down...");
            Console.ReadLine();
            serialPort.PressDown();
        } while (!app.HasExited);
    }

    serialPort.Close();
    serialPort.Dispose();
});

public static class SerialPortExtension
{
    /// <summary>
    /// firmware.inoを参照
    /// </summary>
    public static void PressDown(this SerialPort serialPort) { serialPort.Write("d"); }
    /// <summary>
    /// firmware.inoを参照
    /// </summary>
    public static void PullUp(this SerialPort serialPort) { serialPort.Write("u"); }
}
