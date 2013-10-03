using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeneralLib.Utility
{
    public static class Files
    {
        public static string GetFileText(string absolutePath)
        {
            //Read from file
            StreamReader sr;
            sr = new StreamReader(absolutePath);
            string strOut = sr.ReadToEnd();
            sr.Close();
            return strOut;
        }

        public static void CreateToFile(string AbsoluteFilePath, string fileText)
        {
            StreamWriter sw = File.CreateText(AbsoluteFilePath);
            sw.Write(fileText);
            sw.Close();
        }

        /// <summary>
        /// Updates the text in a file with the passed-in values
        /// </summary>
        /// <param name="AbsoluteFilePath"></param>
        /// <param name="LookFor"></param>
        /// <param name="ReplaceWith"></param>
        public static void UpdateFileText(string AbsoluteFilePath, string LookFor, string ReplaceWith)
        {
            string sIn = GetFileText(AbsoluteFilePath);
            string sOut = sIn.Replace(LookFor, ReplaceWith);
            WriteToFile(AbsoluteFilePath, sOut);
        }

        public static void WriteToFile(string AbsoluteFilePath, string fileText)
        {
            StreamWriter sw = new StreamWriter(AbsoluteFilePath, false);
            sw.Write(fileText);
            sw.Close();
        }
    }
}
