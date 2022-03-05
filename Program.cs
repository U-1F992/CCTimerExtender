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
    Console.WriteLine("Start: {0}", DateTime.Now.Ticks);
}
void OnStop(object? sender, EventArgs e)
{
    Console.WriteLine("Stop: {0}", DateTime.Now.Ticks);
}