﻿
using System.Diagnostics;
using System.Runtime.InteropServices;
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

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("kernel32.dll")]
    public static extern bool AttachConsole(int hProcess);

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

    public static byte ReadByte(int processHandle, long addr)
    {
        byte[] buffer = new byte[1];
        ReadMemory(processHandle, addr, ref buffer);
        return buffer[0];
    }
    public static void WriteByte(int processHandle, long addr, byte value)
    {

        WriteMemory(processHandle, addr, new byte[1] {value});
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
    public static long ReadLong(int processHandle, long addr)
    {
        byte[] buffer = new byte[8];
        ReadMemory(processHandle, addr, ref buffer);
        return BitConverter.ToInt64(buffer, 0);
    }
    public static void WriteLong(int processHandle, long addr, long value)
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
    public static bool IsWindowFocused(int procId)
    {
        return GetForegroundWindow() == procId;
    }
}