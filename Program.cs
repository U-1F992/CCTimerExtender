using (var app = new CCTimer())
{
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