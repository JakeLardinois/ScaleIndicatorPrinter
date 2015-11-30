using System;
using Microsoft.SPOT;

using System.IO;


namespace ScaleIndicatorPrinter.Models
{
    public static class SharedFunctions
    {
        public static void CreateFile()
        {
            Debug.Print("Creating File...");
            using (var objFileStream = new FileStream(@"\SD\WWW\Test.txt", FileMode.Create))
            using (var objStreamWriter = new StreamWriter(objFileStream))
            {
                objStreamWriter.WriteLine("Some test text...");
                objStreamWriter.WriteLine();
            }
            Debug.Print("File Created...");
        }

        public static void CreateFolder()
        {
            var RootDirectory = "\\SD";
            var RequiredDirectory = RootDirectory + "\\WWW";

            DirectoryInfo objDirectoryInfo = new DirectoryInfo(RootDirectory);
            Debug.Print("Current Directories...");
            foreach (var objDir in objDirectoryInfo.GetDirectories())
                Debug.Print(objDir.FullName);

            Debug.Print("Creating Directory  " + RequiredDirectory + "...");
            Directory.CreateDirectory(RequiredDirectory);

            Debug.Print("Now Current Directories...");
            foreach (var objDir in objDirectoryInfo.GetDirectories())
                Debug.Print(objDir.FullName);
        }
    }
}
