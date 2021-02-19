using System;
using System.IO;

namespace Touch
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintHeader();
            try
            {
                TheMain(args);
            }
            catch(Exception err)
            {
                PrintError(err);
            }
        }

        static void TheMain(string[] args)
        {
            var target = null != args && args.Length > 0 ? args[0] : null;
            if (null == target)
            {
                Console.WriteLine("Specify target file or folder as first argument.");
                return;
            }

            var isFile = File.Exists(target);
            var isDir = Directory.Exists(target);

            if (!isFile && !isDir)
            {
                Console.WriteLine($"File or Directory not found: {target}");
                return;
            }

            DateTime timeStamp = DateTime.Now;

            if (args.Length > 1)
            {
                var good = DateTime.TryParse(args[1], out timeStamp);
                if (!good)
                {
                    Console.WriteLine($"Suggested timestamp '{args[1]}' is invalid.");
                    return;
                }
            }

            var flags = (args.Length > 2 ? args[2] : "CM").ToUpper();
            var touchCreationTime = flags.IndexOf('C') >= 0;
            var touchLastWriteTime = flags.IndexOf('M') >= 0;
            var touchLastAccessTime = flags.IndexOf('A') >= 0;

            flags = string.Empty;
            flags += touchCreationTime ? "[C]reationTime " : string.Empty;
            flags += touchLastWriteTime ? "Last[M]odifiedTime " : string.Empty;
            flags += touchLastAccessTime ? "Last[A]ccessTime " : string.Empty;
            if (string.IsNullOrEmpty(flags))
            {
                Console.WriteLine($"Invalid flags. Expecting [C]reationTime, [M]odifiedTime, [A]ccessTime. Examples: 'M', 'CM', 'CMA");
                return;
            }

            Console.WriteLine($"Target     : {target}");
            Console.WriteLine($"Timestamp  : {timeStamp}");
            Console.WriteLine($"Will touch : {flags}");

            Console.WriteLine();
            Console.WriteLine("Press ENTER to touch or Ctrl-C to stop.");
            Console.ReadLine();

            var dirCount = 0;
            var fileCount = 0;

            if (isFile)
            {
                var file = new FileInfo(target);

                if (touchCreationTime) file.CreationTime = timeStamp;
                if (touchLastWriteTime) file.LastWriteTime = timeStamp;
                if (touchLastAccessTime) file.LastAccessTime = timeStamp;

                fileCount += 1;
            }
            else
            {
                var baseDir = new DirectoryInfo(target);

                foreach (var dir in baseDir.EnumerateDirectories("*", SearchOption.AllDirectories))
                {
                    if (touchCreationTime) dir.CreationTime = timeStamp;
                    if (touchLastWriteTime) dir.LastWriteTime = timeStamp;
                    if (touchLastAccessTime) dir.LastAccessTime = timeStamp;

                    dirCount += 1;
                }

                foreach (var file in baseDir.EnumerateFiles("*", SearchOption.AllDirectories))
                {
                    if (touchCreationTime) file.CreationTime = timeStamp;
                    if (touchLastWriteTime) file.LastWriteTime = timeStamp;
                    if (touchLastAccessTime) file.LastAccessTime = timeStamp;

                    fileCount += 1;
                }
            }

            Console.WriteLine($"Touched {dirCount:#,0} folders and {fileCount:#,0} files.");
            Console.WriteLine($"Done.");
        }

        static readonly string DOTS = new string('.', 70);

        static void PrintHeader()
        {
            Console.WriteLine();
            Console.WriteLine(DOTS);
            Console.WriteLine("Usage: Touch.exe <File|Directory> [timeStamp] [CMA]");
            Console.WriteLine("Flags: [C]reateTime, [M]odifiedTime, [A]ccessTime");

            Console.WriteLine(DOTS);
            Console.WriteLine();
        }

        static void PrintError(Exception err)
        {
            Console.WriteLine();
            Console.WriteLine("An error occured:");
            while(null != err)
            {
                Console.WriteLine(err.Message);
                err = err.InnerException;
            }
            Console.WriteLine();
        }
    }
}
