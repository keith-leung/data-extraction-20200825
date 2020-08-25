using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataExtraction
{
    class Program
    {
        /// <summary>
        /// Caution: 
        /// Debug this program need to specify two parameters, 
        /// the source file path and the destination file path
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Please give input and output path. Press any key to exit. ");
                Console.ReadLine();
                System.Environment.Exit(-1);
            }

            try
            {
                string sourcePath = args[0];
                string destinationPath = args[1];
                int columns = -1;
                if (args.Length > 2)
                {
                    int.TryParse(args[2], out columns);
                }
                Console.WriteLine($"Input: {sourcePath}");
                Console.WriteLine($"Output: {destinationPath}");
                Console.WriteLine($"Columns to keep: {columns}");

                if (columns < 1)
                {//Automatically check columns count
                    //read the CSV file for counting the columns
                    using (StreamReader reader = new StreamReader(sourcePath, true))
                    {
                        while (columns < 0)
                        {
                            var line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line))
                                continue;

                            //check title column count
                            var splited = line.Split(',', StringSplitOptions.None);
                            columns = Math.Max(splited.Length + 1, columns);
                            break;
                        }
                    }
                    Console.WriteLine($"Columns to keep after automatically counted: {columns}");
                }

                bool firstLine = true;
                int lineNum = 1;

                //open or replace a file to save the cleaned data lines
                using (StreamWriter writer = new StreamWriter(destinationPath, false, Encoding.UTF8))
                {
                    //read the CSV file for extracting the contents
                    using (StreamReader reader = new StreamReader(sourcePath, true))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (string.IsNullOrEmpty(line))
                            {//ignore blank lines
                                continue;
                            }
                            if (firstLine)
                            {//ignore the first title line
                                //and echo to destination
                                writer.WriteLine(line);
                                firstLine = false;
                                continue;
                            }

                            var splited = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            if (splited.Length >= columns)
                            {//when columns count is equal or greater than the required number                                
                                writer.Write(lineNum);

                                for (int i = 1; i < splited.Length; i++)
                                {
                                    writer.Write(',');
                                    writer.Write(splited[i]);
                                }

                                writer.WriteLine();
                                firstLine = false;
                                lineNum++;
                            }
                        }
                    }

                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Program error: " + ex.Message + "\r\n" + ex.StackTrace);
            }

            Console.WriteLine("Program ended......");
            Console.ReadLine();
        }
    }
}
