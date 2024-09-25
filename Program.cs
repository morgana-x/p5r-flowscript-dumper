using System.Diagnostics;
using System.Text;

public partial class Program
{
    private static string ReadString(Process proc, long os)
    {
        List<byte> toCompile = new List<byte>();
        int numC = 0;
        while (true)
        { 
            byte c = MemoryRead.ReadByte((int)proc.Handle, os);
            os++;
            if (c != 0x0)
            {
                toCompile.Add(c);
                numC = 0;
                continue;
            }
            numC++;
            if (numC >= 2)
                break;
            }
        return Encoding.Default.GetString(toCompile.ToArray());
    }
    private class flowscriptFuncEntry
    {
        public string name;
        public long entryOffset;
        public long functionOffset;
    }

    public static void Main(string[] args)
    {
        // See https://aka.ms/new-console-template for more information
        Console.WriteLine("Type the search term you wish to use!");
        string searchTerm = Console.ReadLine().ToLower();

        Process proc = Process.GetProcessesByName("P5R")[0];

        long tableStart;
        Dictionary<string, flowscriptFuncEntry> functionDump = new Dictionary<string, flowscriptFuncEntry>();

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
            if (functionDump.ContainsKey(funcName))
                funcName += "(1)";
            functionDump.Add(funcName, new flowscriptFuncEntry() { name = funcName, entryOffset = offset-8, functionOffset = funcPointer});
            Console.WriteLine($"{funcName} : 0x{funcPointer.ToString("X")}");
        }
        string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;
        string exeFolder = Directory.GetParent(exePath).FullName;
        string dumpPath = Path.Combine(exeFolder, "flowscriptdump.txt");
        string text = "";
        foreach (var a in functionDump)
            text += $"{a.Key} : entOffset=0x{a.Value.entryOffset.ToString("X")} : func=0x{a.Value.functionOffset.ToString("X")}\n";
        File.WriteAllText(dumpPath, text);
    }
}