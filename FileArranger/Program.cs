using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Text.RegularExpressions;

namespace FileArranger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please type the folder path for rearraging filenames according to time \n");
            try
            {
                string folderPath = Console.ReadLine();
                if (Directory.Exists(folderPath))
                {
                    Dictionary<string, DateTime> fileDict = new Dictionary<string, DateTime>();
                    foreach (var fileName in Directory.GetFiles(folderPath))
                    {
                        DateTime creationDate = GetDateTakenFromImage(Path.Combine(folderPath, fileName));
                        var attr = File.GetAttributes(fileName);
                        fileDict.Add(fileName, creationDate);
                    }
                    Console.WriteLine("File scanning done. Found {0} file. Please enter name to be appended: ", fileDict.Count);
                    string appender = Console.ReadLine();
                    Console.WriteLine("Starting rename operation. Please wait");
                    int fileNumber = 1;
                    foreach (var orderedDict in fileDict.OrderBy(x => x.Value))
                    {
                        string fileName = orderedDict.Key;
                        string fullPath = Path.Combine(folderPath, fileName);
                        string newPath = Path.Combine(folderPath, string.Format("{0:d4} - {1}{2}", fileNumber++, appender, Path.GetExtension(fullPath)));
                        File.Move(fullPath, newPath);
                    }
                    Console.WriteLine(" Operation completed successfully \n");
                }
                else
                {
                    Console.WriteLine("Please input valid Directory.");
                }
            } catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION:  " + ex.ToString());
            }
            Console.WriteLine("Exiting App. ");
            Console.ReadKey();
        }

        private static Regex r = new Regex(":");
        public static DateTime GetDateTakenFromImage(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                    return DateTime.Parse(dateTaken);
                }
            } catch (Exception ex)
            {
                return File.GetLastWriteTime(path);
            }
        }
    }
}

