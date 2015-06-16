using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls;
using DXF.Extensions;
using System.ComponentModel;


namespace DXF_Viewer
{
   class Entity : INotifyPropertyChanged
   {
      protected int j = 0;
      public string EntityHandle { get; set; }
      List<List<String>> entityList = new List<List<string>>();
      public Point offsets = new Point();


      /// <summary>
      /// Gets the entity lists so that it can take the individual lists and make one big
      /// list from the smaller lists
      /// </summary>
      /// <param name="lineIn">The list of entities to be added to the full list</param>
      /// <param name="blockFile">The list of block information to be added to the full list</param>

      public void getEntityInfo(List<String> lineIn, List<String> blockFile)
      {
         //Creates the Lists to be used by each entity

         //used to match dimension ID to block records.
         BlockInfo bi = new BlockInfo();
         List<List<String>> blockList = bi.getBlockInfo(blockFile);

         double tempX;
         double tempY;
         List<String> arcList = new List<String>();
         List<String> circleList = new List<String>();
         List<String> ellipseList = new List<String>();
         List<String> hatchList = new List<String>();
         List<String> insertList = new List<String>();
         List<List<String>> insertListFull = new List<List<String>>();
         List<String> lineList = new List<String>();
         List<String> lwPolylineList = new List<String>();
         List<String> mTextList = new List<String>();
         List<String> polylineList = new List<String>();
         List<String> pointList = new List<String>();
         List<String> solidList = new List<String>();
         List<String> textList = new List<String>();

         //makes a list of lists which contains each entity 
         //seperately with each entity making its own list
         while (j < lineIn.Count)
         {

             //I do not believe that dimList is still needed in the
             //current implentation. Only the ID needs to be saved in
             //order to match it to a block/block record.
            if (lineIn[j] == "DIMENSION")
            {
                String id = "";
                while (lineIn[j] != "  0")
                {  
                    //the ID for the blocks start with * in dxf file.
                    if(lineIn[j].StartsWith("*"))
                    {
                        id = lineIn[j];
                    }
                    j++;
                }       

                //idea is to call entity.getEntityInfo on the subset of block records
                //this should add into the same global entity list.
                //Instead of a recursive-ish approach, each case is handled directly
                //using the same method and global variables that would
                //be used if a non-dimension-associated entity were being 
                //considered.

                foreach (List<String> list in blockList)
                {
                    int i = 0;
                    Boolean startFlag = false;
                    while (i < list.Count())
                    {
                        //in block/block record there are some code/values pairs 
                        //that include the block ID before the entity definitions
                        //this if ensures that it starts at the final mention
                        //of the ID by looking at the line after.
                        //additionally the startFlag is used to keep track of 
                        //whether the program is still reading in entities or
                        //if it has reached the end of the block.
                        //it is needed because the first condition will only occur 
                        //once per block and it needs to be true for each line 
                        //after the matched ID is found until the end of the 
                        //identified block.

                        if ((list[i] == id && list[i+1] == "  0") || startFlag)
                        {
                            //this is the start of the entities that make up the dimension
                            //the next line will be the type of entity
                            //ex: LINE or CIRCLE or MTEXT
                            //from there 
                            startFlag = true;
                            if (list[i] == "ENDBLK")
                            {
                                startFlag = false;
                            }
                            else if (list[i] == "LINE")
                            {
                                while (list[i] != "  0")
                                {
                                    lineList.Add(list[i]);
                                    i++;
                                }
                                entityList.Add(lineList);
                                lineList = new List<String>();
                            }
                            else if (list[i] == "SOLID")
                            {
                                while (list[i] != "  0")
                                {
                                    solidList.Add(list[i]);
                                    i++;
                                }
                                entityList.Add(solidList);
                                solidList = new List<String>();
                            }
                            else if (list[i] == "MTEXT")
                            {
                                while (list[i] != "  0")
                                {
                                    mTextList.Add(list[i]);
                                    i++;
                                }
                                entityList.Add(mTextList);
                                mTextList = new List<String>();
                            }
                            else if (list[i] == "POINT")
                            {
                                while (list[i] != "  0")
                                {
                                    pointList.Add(list[i]);
                                    i++;
                                }
                                entityList.Add(pointList);
                                pointList = new List<String>();
                            }
                            else if (list[i] == "ARC")
                            {
                                while (list[i] != "  0")
                                {
                                    arcList.Add(list[i]);
                                    i++;
                                }
                                entityList.Add(arcList);
                                arcList = new List<String>();
                            }
                        }
                        i++;
                    }
                }
                
            }
            else if (lineIn[j] == "ARC")
            {
                while (lineIn[j] != "  0")
                {
                    arcList.Add(lineIn[j]);
                    j++;
                }
                entityList.Add(arcList);
                arcList = new List<String>();
            }
            else if (lineIn[j] == "CIRCLE")
            {
               while (lineIn[j] != "  0")
               {
                  circleList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(circleList);
               circleList = new List<String>();
            }
            else if (lineIn[j] == "ELLIPSE")
            {
               while (lineIn[j] != "  0")
               {
                  ellipseList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(ellipseList);
               ellipseList = new List<String>();
            }
            else if (lineIn[j] == "HATCH")
            {
               while (lineIn[j] != "  0")
               {
                  hatchList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(hatchList);
               hatchList = new List<String>();
            }
            else if (lineIn[j] == "INSERT")
            {
               while (lineIn[j] != "  0")
               {
                  insertList.Add(lineIn[j]);
                  if (lineIn[j] == " 10")
                  {
                     insertList.Add(lineIn[++j]);
                     tempX = lineIn[j].ConvertToDoubleWithCulture();
                  }
                  if (lineIn[j] == " 20")
                  {
                     insertList.Add(lineIn[++j]);
                     tempY = lineIn[j].ConvertToDoubleWithCulture();
                  }
                  j++;
               }
               InsertEntity insert = new InsertEntity();
               insertListFull = insert.getInfo(insertList, blockFile);
               entityList.AddRange(insertListFull);
            }
            else if (lineIn[j] == "LINE")
            {
               while (lineIn[j] != "  0")
               {
                  lineList.Add(lineIn[j]);
                  j++;
               }
               LineEntity line = new LineEntity();
               entityList.Add(lineList);
               lineList = new List<String>();

            }
            else if (lineIn[j] == "MTEXT")
            {
               while (lineIn[j] != "  0")
               {
                  mTextList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(mTextList);
               mTextList = new List<String>();
            }

            else if (lineIn[j] == "POINT")
            {
               while (lineIn[j] != "  0")
               {
                  pointList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(pointList);
               pointList = new List<String>();
            }
            else if (lineIn[j] == "LWPOLYLINE")
            {
               while (lineIn[j] != "  0")
               {
                  lwPolylineList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(lwPolylineList);
               lwPolylineList = new List<String>();
            }
            else if (lineIn[j] == "POLYLINE")
            {
               while (lineIn[j] != "SEQEND")
               {
                  polylineList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(polylineList);
               polylineList = new List<String>();
            }
            else if (lineIn[j] == "SOLID")
            {
               while (lineIn[j] != "  0")
               {
                  solidList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(solidList);
               solidList = new List<String>();
            }
            else if (lineIn[j] == "TEXT")
            {
               while (lineIn[j] != "  0")
               {
                  textList.Add(lineIn[j]);
                  j++;
               }
               entityList.Add(textList);
               textList = new List<String>();
            }


            j++;
         }
      }
      //gets the list of lists which is called entity list
      //and returns it so that the information can be passed
      //to make the paths for each entity
      public List<List<String>> EntityList()
      {
         return entityList;
      }

      private void NotifyPropertyChanged(string info)
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(info));

         }
      }

      public event PropertyChangedEventHandler PropertyChanged;

   }

}
