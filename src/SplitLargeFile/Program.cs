using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Text;

namespace SplitLargeFile
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<string>(
                    "--input",
                    description: "The path to the large file to split."),
                
                new Option<string>(
                    "--output",
                    description: "The path to the directory where output will be placed."),

                new Option<int>(
                    "--numberoflines",
                    getDefaultValue: () => 10000,
                    description: "The number of lines for the output files"),
            };

            rootCommand.Description = "Splits large files into smaller files.";

            rootCommand.Handler = CommandHandler.Create<string,string,int>((input,output,numberoflines) =>
            {
                SplitFile(input, output, numberoflines);
            });

            rootCommand.InvokeAsync(args);
            //Console.ReadKey();
        }

        static void SplitFile(string inputFilePath, string outputDirectoryPath, int linesPerFile)
        {
            Console.WriteLine("Splitting File...");
            List<string> lines = new List<string>();

            string baseFileName = Path.GetFileNameWithoutExtension(inputFilePath);
            string ext = Path.GetExtension(inputFilePath);

            int fileCount = 1;
            int lineCount = 0;
            
            var inputFileStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
            var inputStreamReader = new StreamReader(inputFileStream, Encoding.UTF8);
            string inputLine;
            while ((inputLine = inputStreamReader.ReadLine()) != null)
            {
                lines.Add(inputLine);
                lineCount++;
                if (lineCount == linesPerFile)
                {
                    string outputFilePath = Path.Combine(outputDirectoryPath, $"{baseFileName}_{fileCount}{ext}");
                    Console.WriteLine($"Writing to file: {outputFilePath}");
                    WriteToFile(outputFilePath,lines);
                    lineCount = 0;
                    fileCount++;
                    lines.Clear();
                }
            }

            if (lines.Count > 0)    // write any left over lines
            {
                string outputFilePath = Path.Combine(outputDirectoryPath, $"{baseFileName}_{fileCount}.{ext}");
                Console.WriteLine($"Writing to file: {outputFilePath}");
                WriteToFile(outputFilePath, lines);
            }

            Console.WriteLine("done.");
        }

        static void WriteToFile(string filePath, List<string> outputLines)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllLines(filePath,outputLines);
        }

        
        
    }
}
