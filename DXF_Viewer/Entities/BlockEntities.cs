using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DXF.Extensions;


namespace DXF_Viewer
{
    class BlockEntities : Entity_Old
    {
        List<List<String>> blockList = new List<List<string>>();


        /// <summary>
        /// gets the information for the blocks and  makes a list of each entity 
        /// and a list of lists containing all of the entity lists
        /// </summary>
        /// <param name="lineIn">The list of entities inside the block</param>
        /// <param name="blockFile">The list of strings which include the entire block chunk of the dxf file</param>
        /// <param name="xOffset">The x coordinate offset</param>
        /// <param name="yOffset">The y coordinate offset</param>
        /// <param name="handleName">The name of the "handle" or identifying label of the block</param>
        /// <returns>A list of lists where each list is an entity from the block to be used</returns>
        public List<List<string>> getBlockInfo(List<String> lineIn, List<string>blockFile,double xScale, double yScale, double xOffset, double yOffset, string handleName, double rotation)
        {
            //Creates the Lists to be used by each entity
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
            List<string> handleList = new List<string>();
            string temp = null;

            //adds "  5" to the handlelist and then adds the handle name so that it can 
            //be referenced later easily by block handle and not by each individual entity
            //handle
            handleList.Add("  5");
            handleList.Add(handleName);

            while (j < lineIn.Count)
            {
                if (lineIn[j] == "ARC")
                {
                    while (lineIn[j] != "  0")
                    {
                        arcList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            arcList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            arcList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            arcList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            arcList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            arcList.Add(temp);
                        }
                        if (lineIn[j] == " 40")
                        {
                            temp = blockYOffset(lineIn[++j], yScale, 0);
                            arcList.Add(temp);
                        }
                        j++;
              
                    }
                    arcList.Add("ROTATION");
                    arcList.Add(Convert.ToString(rotation));
                    arcList.Add("OFFSET");
                    arcList.Add(Convert.ToString(xOffset));
                    arcList.Add(Convert.ToString(yOffset));
                    //adds the arc list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(arcList);
                    arcList = new List<String>();
                }
                else if (lineIn[j] == "CIRCLE")
                {
                    while (lineIn[j] != "  0")
                    {
                        circleList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            circleList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            circleList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            circleList.Add(temp);
                        }
                        if (lineIn[j] == " 40")
                        {
                            temp = blockYOffset(lineIn[++j], yScale, 0);
                            circleList.Add(temp);
                        }
                        j++;
                    }
                    circleList.Add("ROTATION");
                    circleList.Add(Convert.ToString(rotation)); 
                    circleList.Add("OFFSET");
                    circleList.Add(Convert.ToString(xOffset));
                    circleList.Add(Convert.ToString(yOffset));
                    //adds the circle list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(circleList);
                    circleList = new List<String>();
                }
                else if (lineIn[j] == "ELLIPSE")
                {
                    while (lineIn[j] != "  0")
                    {
                        ellipseList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            ellipseList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            ellipseList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            ellipseList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            ellipseList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            ellipseList.Add(temp);
                        }
                        j++;
                    }
                    ellipseList.Add("ROTATION");
                    ellipseList.Add(Convert.ToString(rotation));
                    ellipseList.Add("OFFSET");
                    ellipseList.Add(Convert.ToString(xOffset));
                    ellipseList.Add(Convert.ToString(yOffset));
                    //adds the ellipse list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(ellipseList);
                    ellipseList = new List<String>();
                }
                else if (lineIn[j] == "HATCH")
                {
                    while (lineIn[j] != "  0")
                    {
                        hatchList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            hatchList.Add(handleName);
                            hatchList.Add("ROTATION");
                            hatchList.Add(Convert.ToString(rotation));
                            hatchList.Add("OFFSET");
                            hatchList.Add(Convert.ToString(xOffset));
                            hatchList.Add(Convert.ToString(yOffset));
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            hatchList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            hatchList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            hatchList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            hatchList.Add(temp);
                        }
                        j++;
                    }

                    //adds the hatch list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(hatchList);
                    hatchList = new List<String>();
                }
                else if (lineIn[j] == "INSERT")
                {
                    while (lineIn[j] != "  0")
                    {
                        insertList.Add(lineIn[j]);
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            insertList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            insertList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            insertList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            insertList.Add(temp);
                        }
                        j++;
                    }
                    InsertEntity insert = new InsertEntity();
                    insertListFull = insert.getInfo(insertList, blockFile);
                    blockList.AddRange(insertListFull);
                }
                else if (lineIn[j] == "LINE")
                {
                    while (lineIn[j] != "  0")
                    {
                        lineList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            lineList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            lineList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            lineList.Add(temp);
                        } 
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            lineList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            lineList.Add(temp);
                        }
                        j++;
                    }
                    lineList.Add("ROTATION");
                    lineList.Add(Convert.ToString(rotation));
                    lineList.Add("OFFSET");
                    lineList.Add(Convert.ToString(xOffset));
                    lineList.Add(Convert.ToString(yOffset));
                    //LineEntity line = new LineEntity();
                    //adds the line list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(lineList);
                    lineList = new List<String>();

                }
                else if (lineIn[j] == "MTEXT")
                {
                    while (lineIn[j] != "  0")
                    {
                        mTextList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                           mTextList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            mTextList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            mTextList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            mTextList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            mTextList.Add(temp);
                        }
                        j++;
                    }
                    mTextList.Add("ROTATION");
                    mTextList.Add(Convert.ToString(rotation));
                    mTextList.Add("OFFSET");
                    mTextList.Add(Convert.ToString(xOffset));
                    mTextList.Add(Convert.ToString(yOffset));
                    //adds the mtext list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(mTextList);
                    mTextList = new List<String>();
                }

                else if (lineIn[j] == "POINT")
                {
                    while (lineIn[j] != "  0")
                    {
                        pointList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            pointList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            pointList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            pointList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            pointList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            pointList.Add(temp);
                        }
                        j++;
                    }
                    pointList.Add("ROTATION");
                    pointList.Add(Convert.ToString(rotation));
                    pointList.Add("OFFSET");
                    pointList.Add(Convert.ToString(xOffset));
                    pointList.Add(Convert.ToString(yOffset));
                    //adds the point list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(pointList);
                    pointList = new List<String>();
                }
                else if (lineIn[j] == "LWPOLYLINE")
                {

                    while (lineIn[j] != "  0")
                    {
                        lwPolylineList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            lwPolylineList.Add(handleName);
                            lwPolylineList.Add("ROTATION");
                            lwPolylineList.Add(Convert.ToString(rotation));
                            lwPolylineList.Add("OFFSET");
                            lwPolylineList.Add(Convert.ToString(xOffset));
                            lwPolylineList.Add(Convert.ToString(yOffset));
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            lwPolylineList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            lwPolylineList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            lwPolylineList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            lwPolylineList.Add(temp);
                        }
                        if (lineIn[j] == " 42")
                        {
                            temp = blockYOffset(lineIn[++j], yScale, 0);
                            lwPolylineList.Add(temp);
                        }
                        j++;
                    }

                    //adds the lwpolyline list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(lwPolylineList);
                    lwPolylineList = new List<String>();
                }
                else if (lineIn[j] == "VERTEX" || lineIn[j] == "POLYLINE")
                {
                    while (lineIn[j] != "SEQEND")
                    {
                        polylineList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            polylineList.Add(handleName);
                            polylineList.Add("ROTATION");
                            polylineList.Add(Convert.ToString(rotation));
                            polylineList.Add("OFFSET");
                            polylineList.Add(Convert.ToString(xOffset));
                            polylineList.Add(Convert.ToString(yOffset));
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            polylineList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            polylineList.Add(temp);
                        }
                        
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            polylineList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            polylineList.Add(temp);
                        }
        
                        j++;
                    }

                    //adds the polyline list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(polylineList);
                    polylineList = new List<String>();
                }
                else if (lineIn[j] == "SOLID")
                {
                    while (lineIn[j] != "  0")
                    {
                        solidList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            solidList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            solidList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            solidList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            solidList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            solidList.Add(temp);
                        }
                        if (lineIn[j] == " 12")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            solidList.Add(temp);
                        }
                        if (lineIn[j] == " 22")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            solidList.Add(temp);
                        }
                        if (lineIn[j] == " 13")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            solidList.Add(temp);
                        }
                        if (lineIn[j] == " 23")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            solidList.Add(temp);
                        }
                        j++;
                    }
                    solidList.Add("ROTATION");
                    solidList.Add(Convert.ToString(rotation));
                    solidList.Add("OFFSET");
                    solidList.Add(Convert.ToString(xOffset));
                    solidList.Add(Convert.ToString(yOffset));
                    //adds the solid list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(solidList);
                    solidList = new List<String>();
                }
                else if (lineIn[j] == "TEXT")
                {
                    while (lineIn[j] != "  0")
                    {
                        textList.Add(lineIn[j]);
                        if (lineIn[j] == "  5")
                        {
                            j++;
                            //Gets the handle name of this specific entity
                            textList.Add(handleName);
                        }
                        if (lineIn[j] == " 10")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            textList.Add(temp);
                        }
                        if (lineIn[j] == " 20")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            textList.Add(temp);
                        }
                        if (lineIn[j] == " 11")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockXOffset(lineIn[++j], xOffset, xScale);
                            textList.Add(temp);
                        }
                        if (lineIn[j] == " 21")
                        {
                            //adds the offset then adds new coordinate to the list
                            temp = blockYOffset(lineIn[++j], yScale, yOffset);
                            textList.Add(temp);
                        }
                        j++;
                    }
                    textList.Add("ROTATION");
                    textList.Add(Convert.ToString(rotation));
                    textList.Add("OFFSET");
                    textList.Add(Convert.ToString(xOffset));
                    textList.Add(Convert.ToString(yOffset));
                    //adds the text list to the list of lists called blockList 
                    //which has one handle for all entities inside of it
                    blockList.Add(textList);
                    textList = new List<String>();
                }


                j++;
            }
            return blockList;
        }


        /// <summary>
        /// Takes the x coordinate and offset and adds them together and
        /// returns the correct value
        /// </summary>
        /// <param name="lineIn">the x coordinate as a strin</param>
        /// <param name="xOffset">the x offset as a double</param>
        /// <returns>the corrected coordinate as a string</returns>
        public string blockXOffset(string lineIn, double xOffset, double xScale)
        {
            BlockEntities block = new BlockEntities();
            double temp = 0;

            temp = (lineIn.ConvertToDoubleWithCulture() * xScale) + xOffset;
            
            lineIn = Convert.ToString(temp);
            
            return lineIn;      
        }

        /// <summary>
        /// Takes the y coordinate and offset and adds them together 
        /// and returns the coorrect value
        /// </summary>
        /// <param name="lineIn">y coordinate as a strin</param>
        /// <param name="yOffset">y offset as a double</param>
        /// <returns>corrected coordinate as a string</returns>
        public string blockYOffset(string lineIn, double yScale, double yOffset = 0)
        {
            BlockEntities block = new BlockEntities();
            double temp = 0;

            temp = (lineIn.ConvertToDoubleWithCulture() * yScale) + yOffset;

            lineIn = Convert.ToString(temp);

            return lineIn;
        }

    }


}
