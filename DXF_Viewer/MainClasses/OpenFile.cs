using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Data;



namespace DXF_Viewer
{
    /// <summary>
    /// Opens the file so that it can create a List to be used later to get 
    /// the information from the dxf file
    /// </summary>
    class OpenFile
    {
        public static String source = null;

        /// <summary>
        /// Opens the file and creates a list of strings to be used by each 
        /// class that finds information from the dxf file
        /// </summary>
        /// <param name="source">The name of the file being used</param>
        /// <param name="errFlg">True indicates that an error has occured.</param>
        /// <returns>Entire file as a list of strings</returns>
        public List<string> OpenSource(string source, ref bool errFlg)
        {

           try
           {

            StreamReader dxfFileIn = new StreamReader(new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            String temp = dxfFileIn.ReadLine();
            List<String> wholeFile = new List<String>();
            
            //Reads in the entire file and writes each line of the dxf to a temp list of strings
            //called wholeFile so that it can be used later by the rest of the program
            while (temp != "EOF")
            {
                temp = dxfFileIn.ReadLine();
                wholeFile.Add(temp);
            }
            dxfFileIn.Close();
            return wholeFile;
           }
           catch (Exception ex)
           {
              errFlg = true;
              List<string> lst = new List<string>();
              lst.Add("General exception in OpenFile.OpenSource: " + ex.Message);
              return lst;
           }

        }
    }
}
