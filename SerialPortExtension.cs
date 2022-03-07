using System.IO.Ports;

public static class SerialPortExtension
{
    public static void Down(this SerialPort serialPort)
    {
        serialPort.Write("d");
    }
    public static void Up(this SerialPort serialPort)
    {
        serialPort.Write("u");
    }
}