using System.Diagnostics;
using System.Runtime.InteropServices;

public class CCTimer : IDisposable
{
    [DllImport("user32.dll")]
    static extern IntPtr GetDesktopWindow();

    const string CLASSNAME_APP = "WindowsForms10.Window.8.app.0.34f5582_r7_ad1";
    const string CLASSNAME_SYSTABCONTROL32 = "WindowsForms10.SysTabControl32.app.0.34f5582_r7_ad1";
    const string CLASSNAME_STATIC = "WindowsForms10.STATIC.app.0.34f5582_r7_ad1";
    const string CLASSNAME_BUTTON = "WindowsForms10.BUTTON.app.0.34f5582_r7_ad1";

    /// <summary>
    /// CCTimer.exe
    /// </summary>
    Process? _process;
    /// <summary>
    /// CCTimer GUI root
    /// </summary>
    IntPtr _application;
    /// <summary>
    /// CCTimer > 0:00
    /// </summary>
    IntPtr _txtTimer;
    /// <summary>
    /// CCTimer > CountUpTimer > Start/Cancel
    /// </summary>
    IntPtr _btnCountUpTimerStartCancel;

    CancellationTokenSource _cts;
    public event EventHandler? OnCountUpTimerStart;
    public event EventHandler? OnCountUpTimerStop;

    public CCTimer() : this(Path.Join(AppContext.BaseDirectory, "CCTimer", "CCTimer.exe")) {}
    public CCTimer(string path)
    {
        // CCTimer.exeを起動する
        _process = Process.Start(new ProcessStartInfo(path));
        if (_process == null) throw new Exception("CCTimer.exe dose not start.");
        
        // 要素のハンドルを掴めるようになるまで待機する
        // _process.MainWindowHandle と実際のウィンドウハンドルが対応しないタイプのアプリケーションらしい
        // 各要素のハンドルを掴めるようになるまでラグがある
        while (true)
        {
            try
            {
                GetDesktopWindow().GetChildHandle(CLASSNAME_APP, "CCTimer").GetChildHandle(CLASSNAME_APP, "").GetChildHandle(CLASSNAME_STATIC, "0:00");
                GetDesktopWindow().GetChildHandle(CLASSNAME_APP, "CCTimer").GetChildHandle(CLASSNAME_SYSTABCONTROL32, "").GetChildHandle(CLASSNAME_APP, "CountUpTimer").GetChildHandle(CLASSNAME_BUTTON, "Start");
                break;
            }
            catch
            {
                // Console.WriteLine("Waiting for controls to become operational.: {0}", DateTime.Now.Ticks);
            }
        }
        
        // 各要素のハンドルを掴む
        try
        {
            _application = GetDesktopWindow().GetChildHandle(CLASSNAME_APP, "CCTimer");
            _txtTimer = _application
                .GetChildHandle(CLASSNAME_APP, "")
                .GetChildHandle(CLASSNAME_STATIC, "0:00");
            _btnCountUpTimerStartCancel = _application
                .GetChildHandle(CLASSNAME_SYSTABCONTROL32, "")
                .GetChildHandle(CLASSNAME_APP, "CountUpTimer")
                .GetChildHandle(CLASSNAME_BUTTON, "Start");
        }
        catch
        {
            _process.Kill(true);
            _process.Dispose();
            throw;
        }

        _cts = new CancellationTokenSource();
        Task.Run(() => EventLoop(_cts.Token));
    }

    private void EventLoop(CancellationToken ct)
    {
        bool busy = false;

        while (!ct.IsCancellationRequested)
        {
            var state = (txt: _txtTimer.GetText(), btn: _btnCountUpTimerStartCancel.GetText());

            if (!busy && state != ("0:00", "Start") && state.btn == "Cancel")
            {
                busy = true;
                if (OnCountUpTimerStart != null) OnCountUpTimerStart(this, EventArgs.Empty);
            }
            else if (busy && state == ("0:00", "Start"))
            {
                busy = false;
                if (OnCountUpTimerStop != null) OnCountUpTimerStop(this, EventArgs.Empty);
            }
        }
    }

    public void WaitForExit()
    {
        _process?.WaitForExit();
    }
    public bool HasExited
    {
        get { return _process == null ? true : _process.HasExited; }
    }

    // IDisposable
    private bool _disposed = false;
    public void Dispose()
    {
        Dispose(true);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_process != null)
                {
                    _process.Kill(true);
                    _process.Dispose();
                    _process = null;
                }
                _cts.Cancel();
            }
            _disposed = true;
        }
    }
}