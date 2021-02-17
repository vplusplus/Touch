﻿using System;
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
            var baseFolder = null != args && args.Length > 0 ? args[0] : null;
            if (null == baseFolder)
            {
                Console.WriteLine("Specify the baseFolder as first argument.");
                return;
            }

            var isFile = File.Exists(baseFolder);
            if (isFile)
            {
                Console.WriteLine("First argument should specify a directoty, not a file. Try again.");
                return;
            }

            var exists = Directory.Exists(baseFolder);
            if (!exists)
            {
                Console.WriteLine($"Directory not found: {baseFolder}");
                return;
            }

            DateTime timeStamp = DateTime.Now;

            if (args.Length > 1)
            {
                var good = DateTime.TryParse(args[1], out timeStamp);
                if (!good)
                {
                    Console.WriteLine($"Suggested time stamp '{args[1]}' is invalid. Check secone argumnt.");
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
                Console.WriteLine($"Invalid specs. Expecting [C]reationTime, [M]odifiedTime, [A]ccessTime. Examples: 'M', 'CM', 'CMA");
                return;
            }

            Console.WriteLine($"Target folder: {baseFolder}");
            Console.WriteLine($"Timestamp    : {timeStamp}");
            Console.WriteLine($"Will touch   : {flags}");

            Console.WriteLine();
            Console.WriteLine("Press ENTER to touch or Ctrl-C to stop.");
            Console.ReadLine();

            var baseDir = new DirectoryInfo(baseFolder);
            var dirCount = 0;
            var fileCount = 0;

            foreach(var dir in baseDir.GetDirectories("*", SearchOption.AllDirectories))
            {
                if (touchCreationTime) dir.CreationTime = timeStamp;
                if (touchLastWriteTime) dir.LastWriteTime = timeStamp;
                if (touchLastAccessTime) dir.LastAccessTime = timeStamp;
                
                dirCount += 1;
            }

            foreach(var file in baseDir.GetFiles("*", SearchOption.AllDirectories))
            {
                if (touchCreationTime) file.CreationTime = timeStamp;
                if (touchLastWriteTime) file.LastWriteTime = timeStamp;
                if (touchLastAccessTime) file.LastAccessTime = timeStamp;

                fileCount += 1;
            }

            Console.WriteLine($"Touched {dirCount:#,0} folders and {fileCount:#,0} files.");
            Console.WriteLine($"Done.");
        }

        static readonly string DOTS = new string('.', 70);

        static void PrintHeader()
        {
            Console.WriteLine();
            Console.WriteLine(DOTS);
            Console.WriteLine("Usage: Touch.exe <directory> [timeStamp] [CMA]");
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