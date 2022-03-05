var sw = new System.Diagnostics.Stopwatch();
using (var app = new CCTimerExtender())
{
    // Ctrl+C で強制終了された際にCCTimerを解放する
    Console.CancelKeyPress += ((object? sender, ConsoleCancelEventArgs e) => {app.Dispose();});
    
    app.OnCountUpTimerStart += OnStart;
    app.OnCountUpTimerStop += OnStop;
    
    app.WaitForExit();
}

void OnStart(object? sender, EventArgs e)
{
    sw.Start();
}
void OnStop(object? sender, EventArgs e)
{
    sw.Stop();
    Console.WriteLine(sw.ElapsedMilliseconds);
    sw.Reset();
}
