using System.Text;
using System.Runtime.InteropServices;

public static class HandleExtension
{
    // http://tarukichi.chu.jp/codetips/sendkeys.html
    [DllImport("user32.dll")]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string? lpszClass, string? lpszWindow);
    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, StringBuilder lParam);
    [DllImport("user32.dll")]
    private static extern int VkKeyScan(char ch);

    public static IntPtr GetChildHandle(this IntPtr hwndParent, string? lpszClass, string? lpszWindow)
    {
        var ret = FindWindowEx(hwndParent, IntPtr.Zero, lpszClass, lpszWindow);
        if (ret == IntPtr.Zero) throw new Exception(String.Format("No such child. {0}, {1}", lpszClass, lpszWindow));
        return ret;
    }

    public static void LeftClick(this IntPtr handle)
    {
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        Task.Run(() =>
        {
            PostMessage(handle, WM_LBUTTONDOWN, 1, 0);
            Thread.Sleep(50);
            PostMessage(handle, WM_LBUTTONUP, 1, 0);
        });
    }

    public static string GetText(this IntPtr handle)
    {
        const int WM_GETTEXT = 0x000D;
        StringBuilder sb = new StringBuilder(256);
        // 本当はいろいろある
        // ポインタの渡し方としては乱暴すぎる
        // https://ikorin2.hatenablog.jp/entry/2020/03/10/181417
        SendMessage(handle, WM_GETTEXT, sb.Capacity, sb);
        return sb.ToString();
    }
}