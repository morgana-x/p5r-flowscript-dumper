using System.Diagnostics;
using System.Text;

public partial class Program
{
    private static string ReadString(Process proc, long os)
    {
        List<byte> toCompile = new List<byte>();
        while (true)
        { 
            byte c = MemoryRead.ReadByte((int)proc.Handle, os);
            if (c == 0x0)
                break;
            toCompile.Add(c);
            os++;
        }
        return Encoding.Default.GetString(toCompile.ToArray());
    }
    public static void Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Type the search term you wish to use!");
        string searchTerm = Console.ReadLine().ToLower();

        Process proc = Process.GetProcessesByName("P5R")[0];

        long tableStart;
        Dictionary<string, long> functionDump = new Dictionary<string, long>();

        long offset = 0x14192e890;


        for (int i = 0; i < 20487; i++)
        {
            long stringPointer = MemoryRead.ReadLong((int)proc.Handle, offset);
            offset += 8;
            long funcPointer = MemoryRead.ReadLong((int)proc.Handle, offset);
            offset += 8;
            
            string funcName = stringPointer != 0 ? ReadString(proc,stringPointer) : "UNKNOWN";
            if (!funcName.ToLower().Contains(searchTerm))
                continue;
            functionDump.Add(funcName, funcPointer);
            Console.WriteLine($"{funcName} : 0x{stringPointer.ToString("X")}");
        }
        string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        string exeFolder = Directory.GetParent(exePath).FullName;
        string dumpPath = Path.Combine(exeFolder, "flowscriptdump.txt");
        string text = "";
        foreach (var a in functionDump)
            text += $"{a.Key} : 0x{a.Value.ToString("X")}\n";
        File.WriteAllText(dumpPath, text);
    }
}