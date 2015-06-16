using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

namespace DXF_Builder
{
    public class Builder
    {
        public bool ErrFlag { get; set; }
        public string ErrString { get; set; }
        public string decimalSeparator;


        public void Build(String baseFile, List<string> fileList, String fileOut)
        {
           try
           {
              //Retrieve the decimal separator for the current culture on the
              //user's computer.  This will be used to change dots to commas in
              //cases where it is required.
              decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

              ErrFlag = false;
              ErrString = string.Empty;

              List<string> listOut = new List<string>();
              List<string> entList = new List<string>();
              List<string> blockList = new List<string>();
              List<string> tempList = new List<string>();
              List<string> layerList = new List<string>();
              List<string> tableList = new List<string>();
              List<string> orderInformation = new List<string>();
              int handleNum = 100;
              int i = 0;
              int j = 0;
              int k = 0;
              double tempMinX = 0;
              double tempMinY = 0;
              double tempMaxX = 0;
              double tempMaxY = 0;
              double minX = 100000;
              double minY = 100000;
              double maxX = -100000;
              double maxY = -100000;

              try
              {
                 StreamReader baseFileIn = new StreamReader(new FileStream(baseFile, FileMode.Open, FileAccess.Read));
                 string baseTemp = baseFileIn.ReadLine();
                 listOut.Add("  0");
                 //Gets the entire base file and puts that into a list so that
                 //it can be accessed and added to later.  All of the files that 
                 //are being read in are going to be added to this file
                 while (baseTemp != "EOF")
                 {

                    baseTemp = baseFileIn.ReadLine();
                    listOut.Add(baseTemp);
                 }


              }
              catch (System.IO.FileNotFoundException ioex)
              {
                 if (ErrFlag == false)
                    ErrFlag = true;
                 ErrString += "Base file not found at: " + baseFile + "Dxf file was not built";
                 ErrString += "\n" + ioex.Message;
                 return;
              }
              //Point 1 
              catch (System.Exception ex)
              {
                 ErrFlag = true;
                 ErrString += "\nGeneric exception in Builder.Build, Point 1:  " + ex.Message;
                 return;
              }

              foreach (string file in fileList)
              {
                 //Beginning of Point 2 try-catch.
                 try
                 {
                    i = 0;
                    tempList = new List<string>();
                    StreamReader fileIn = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    string temp = fileIn.ReadLine();

                    while (temp != "EOF")
                    {
                       temp = fileIn.ReadLine();
                       temp = ConvertDecimalSymbol(temp);
                       tempList.Add(temp);
                    }
                    fileIn.Close();
                    while (tempList[i] != "EOF")
                    {
                       if (tempList[i] == "ENTITIES")
                       {
                          i++;

                          while (tempList[i] != "ENDSEC")
                          {
                             i++;
                             if (tempList[i] == "ENDSEC")
                                break;
                             else
                                entList.Add(tempList[i]);
                          }
                       }
                       if (tempList[i] == "HEADER")
                       {
                          while (tempList[i] != "ENDSEC")
                          {
                             i++;
                             if (tempList[i] == "ENDSEC")
                                break;
                             else
                             {
                                if (tempList[i] == "$EXTMIN")
                                {
                                   while (tempList[i] != "  9")
                                   {

                                      if (tempList[i] == " 10")
                                      {
                                         tempMinX = Convert.ToDouble(tempList[++i]);
                                         if (tempMinX < minX)
                                            minX = tempMinX;
                                      }
                                      if (tempList[i] == " 20")
                                      {
                                         tempMinY = Convert.ToDouble(tempList[++i]);
                                         if (tempMinY < minY)
                                            minY = tempMinY;
                                      }
                                      i++;
                                   }
                                }
                                if (tempList[i] == "$EXTMAX")
                                {
                                   while (tempList[i] != "  9")
                                   {
                                      if (tempList[i] == " 10")
                                      {
                                         tempMaxX = Convert.ToDouble(tempList[++i]);
                                         if (tempMaxX > maxX)
                                            maxX = tempMaxX;
                                      }
                                      if (tempList[i] == " 20")
                                      {
                                         tempMaxY = Convert.ToDouble(tempList[++i]);
                                         if (tempMaxY > maxY)
                                            maxY = tempMaxY;
                                      }
                                      i++;
                                   }
                                }

                             }
                          }
                       }
                       if (tempList[i] == "BLOCKS")
                       {
                          i++;
                          i++;
                          while (tempList[i] != "ENDSEC")
                          {
                             blockList.Add(tempList[i]);
                             i++;
                          }
                       }
                       if (tempList[i] == "TABLES")
                       {
                          while (tempList[i] != "ENDSEC")
                          {
                             i++;
                             if (tempList[i] == "ENDSEC")
                                break;
                             else
                                tableList.Add(tempList[i]);
                          }
                       }
                       if (tempList[i] == "LAYER")
                       {
                          while (tempList[i] != "ENDTAB")
                          {
                             i++;
                             if (tempList[i] == "ENDTAB")
                                break;
                             else
                                layerList.Add(tempList[i]);
                          }
                       }
                       i++;


                    }



                 }
                 catch (System.IO.FileNotFoundException ioex)
                 {
                    if (ErrFlag == false)
                       ErrFlag = true;
                    ErrString += " File not found at: " + file;
                    ErrString += "\n" + ioex.Message;
                 }
                 //Point 2 
                 catch (System.Exception ex)
                 {
                    ErrFlag = true;
                    ErrString += "\nGeneric exception in Builder.Build, Point 2:  " + ex.Message;
                    return;
                 }


              }

              while (listOut[j] != "EOF")
              {
                 j++;
                 for (k = 0; k < entList.Count - 1; k++)
                 {
                    int tempNum;
                    tempNum = handleNum;
                    if (entList[k] == "  5")
                    {
                       k++;
                       if (orderInformation != null)
                       {
                          for (int m = 0; m < orderInformation.Count - 1; m++)
                          {
                             if (orderInformation[m] == "331")
                             {
                                m++;
                                if (orderInformation[m] == entList[k])
                                {
                                   orderInformation[m] = Convert.ToString(handleNum);
                                   entList[k] = Convert.ToString(handleNum);
                                   handleNum++;
                                   break;
                                }
                             }
                          }
                       }
                       if (tempNum == handleNum)
                       {
                          entList[k] = Convert.ToString(handleNum);
                          k++;
                          handleNum++;
                       }
                    }
                 }
                 if (listOut[j] == "HEADER")
                 {
                    j++;
                    while (listOut[j] != "ENDSEC")
                    {
                       j++;
                       if (listOut[j] == "$EXTMIN")
                       {
                          while (listOut[j] != "  9")
                          {
                             j++;
                             if (listOut[j] == " 10")
                             {
                                j++;
                                listOut.Insert(j, (Convert.ToString(minX)));
                                j++;
                                listOut.RemoveAt(j);
                             }
                             if (listOut[j] == " 20")
                             {
                                j++;
                                listOut.Insert(j, (Convert.ToString(minY)));
                                j++;
                                listOut.RemoveAt(j);
                             }
                             if (listOut[j] == "ENDSEC")
                                break;
                          }
                       }

                       if (listOut[j] == "$EXTMAX")
                       {
                          while (listOut[j] != "  9")
                          {
                             j++;
                             if (listOut[j] == " 10")
                             {
                                j++;
                                listOut.Insert(j, (Convert.ToString(maxX)));
                                j++;
                                listOut.RemoveAt(j);
                             }
                             if (listOut[j] == " 20")
                             {
                                j++;
                                listOut.Insert(j, (Convert.ToString(maxY)));
                                j++;
                                listOut.RemoveAt(j);
                             }
                             if (listOut[j] == "ENDSEC")
                                break;
                          }
                       }
                       if (listOut[j] == "ENDSEC")
                          break;
                    }
                 }
                 if (listOut[j] == "ENTITIES")
                 {
                    while (listOut[j] != "ENDSEC")
                    {
                       j++;
                    }

                    listOut.InsertRange(j, entList);
                 }


              }

              try
              {
                 StreamWriter newFile = new StreamWriter(new FileStream(fileOut, FileMode.Create, FileAccess.Write));
                 string tmpStr;

                 //writes the listout to the new file which can now be read by the 
                 //dxf viewer
                 for (int count = 0; count <= listOut.Count - 1; count++)
                 {
                    tmpStr = ReConvertDecimalSymbol(listOut[count]);
                    //tmpStr = listOut[count];
                    newFile.WriteLine(tmpStr);
                 }
                 newFile.Close();
              }
              catch (System.IO.DirectoryNotFoundException dnfex)
              {
                 ErrFlag = true;
                 ErrString += " Could not create output file: " + fileOut;
                 ErrString += "\n" + dnfex.Message;
              }
              //Point 3 
              catch (System.Exception ex)
              {
                 ErrFlag = true;
                 ErrString += "\nGeneric exception in Builder.Build, Point 3:  " + ex.Message;
                 return;
              }

              if (ErrString != null)
              {
                 ErrString += " may not be complete or accurate.";
              }

           }
           //Point 4
           catch (System.Exception ex)
           {
              ErrFlag = true;
              ErrString += "An unexpected error has occured, Point 4: " + ex.Message;
              return;
           }
        }


       private string ConvertDecimalSymbol(string input)
       {
          if (decimalSeparator != ".")
          {
             input = input.Replace(".", decimalSeparator);
          }
          return input;
       }

       private string ReConvertDecimalSymbol(string input)
       {
          if (decimalSeparator != ".")
          {
             input = input.Replace(decimalSeparator, ".");
          }
          return input;
       }


    }
}
