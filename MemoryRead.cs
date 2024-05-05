using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class MemoryRead
{
    const int PROCESS_ALL_ACCESS = 0x1F0FFF;
    //const int PROCESS_WM_READ = 0x0010;

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteProcessMemory(int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(int hProcess, long lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);



    public static int ReadMemory(int processHandle, long addr, ref byte[] buffer)
    {
        int bytesRead = 0;
        bool SUCCESS = ReadProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesRead);


        return bytesRead;
    }
    public static int WriteMemory(int processHandle, long addr, byte[] buffer)
    {
        int bytesWritten = 0;
        WriteProcessMemory((int)processHandle, addr, buffer, buffer.Length, ref bytesWritten);
        return bytesWritten;
    }


    public static float ReadFloat(int processHandle, long addr)
    {
        byte[] buffer = new byte[4];
        ReadMemory(processHandle, addr, ref buffer);
        return BitConverter.ToSingle(buffer, 0);
    }
    public static void WriteFloat(int processHandle, long addr, float value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        WriteMemory(processHandle, addr, buffer);
    }
    public static int ReadInt(int processHandle, long addr)
    {
        byte[] buffer = new byte[4];
        ReadMemory(processHandle, addr, ref buffer);
        return BitConverter.ToInt32(buffer, 0);
    }
    public static void WriteInt(int processHandle, long addr, int value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        WriteMemory(processHandle, addr, buffer);
    }
    public static short ReadShort(int processHandle, long addr)
    {
        byte[] buffer = new byte[2];
        ReadMemory(processHandle, addr, ref buffer);
        return BitConverter.ToInt16(buffer, 0);
    }
    
    public static void WriteShort(int processHandle, long addr, short value)
    {
        byte[] buffer = BitConverter.GetBytes(value);
        WriteMemory(processHandle, addr, buffer);
    }
    public static Process GetProcess(string name)
    {
        return Process.GetProcessesByName(name)[0];
    }
    public static long GetProcessBaseAddress(Process proc)
    {
        return proc.MainModule.BaseAddress.ToInt64();
    }
    public static int GetProcessHandle(Process process)
    {
        IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);
        return (int)processHandle;
    }
    /*public static void Main()
    {

        Process process = Process.GetProcessesByName("game.exe")[0];
        IntPtr processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, process.Id);

        int bytesWritten = 0;
        byte[] buffer = Encoding.Unicode.GetBytes("It works!\0"); // '\0' marks the end of string

        // replace 0x0046A3B8 with your address
        WriteProcessMemory((int)processHandle, 0x0046A3B8, buffer, buffer.Length, ref bytesWritten);
        Console.ReadLine();
    }*/
}