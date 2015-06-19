using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using DXF.Extensions;


namespace DXF_Viewer
{
    
    class InsertEntity : Entity_Old
    {
        String blockName = null;
        string tempBlockName = null;
        string layerName = "0";
        string insertHandle;
        int i = 0;
        BlockInfo_dep block = new BlockInfo_dep();
        List<List<string>> insertList = new List<List<string>>();
        List<List<string>> insertListFull = new List<List<string>>();
        List<string> tempList = new List<string>();

        /// <summary>
        /// This will take the insert entity and get the information for the proper block so that the block can be drawn
        /// </summary>
        /// <param name="listIn">The list of strings which includes all the information for this entity</param>
        /// <param name="blockFile">A list of all the blocks in the block section so that the proper block can be found</param>
        /// <returns>List of entities from the proper block that need to be drawn</returns>
        public List<List<string>> getInfo(List<String> listIn, List<String> blockFile)
        {
            blockName = null;
            double offsetX = 0;
            double offsetY = 0;
            double xScale = 1;
            double yScale = 1;
            double rotation = 0;

            j++;
            while (j < listIn.Count)
            {
                //Name of the block this entity is saying to insert
                if (listIn[j] == "  2")
                {
                    blockName = listIn[++j];
                }
                //the name of the layer that this entity uses
                if (listIn[j] == "  8")
                {
                    layerName = listIn[++j];
                }
                //The x coordinate offset value
                if (listIn[j] == " 10")
                {
                    offsetX = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The y coordinate offset value
                if (listIn[j] == " 20")
                {
                    offsetY = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The handle of the insert entity
                if (listIn[j] == "  5")
                {
                    insertHandle = listIn[++j];
                }
                //The scale of the block to be drawn in x coordinates.
                //ie if it is 50% original size etc
                if (listIn[j] == " 41")
                {
                    xScale = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The scale of the block to be drawn in y coordinates.
                //ie if it is 50% original size etc
                if (listIn[j] == " 42")
                {
                    yScale = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The rotation of the block to be drawn
                if (listIn[j] == " 50")
                {
                    rotation = listIn[++j].ConvertToDoubleWithCulture();
                }
                j++;
            }
            //makes a new list of all of the block information so that the proper block can be pulled out
            insertList = block.getBlockInfo(blockFile);
            BlockEntities blockEnt = new BlockEntities();
            
            //checks for the proper block name dependent on the block name the insert was looking for
            //and starts to pull that information so that it can build a list of strings for that block
            while (i < insertList.Count)
            {
                List<string> temp = insertList[i];
                int k = 0;
                while (k < temp.Count - 1)
                {
                    if (temp[k] == "  3")
                        tempBlockName = temp[++k];
                    if (blockName == tempBlockName)
                    {
                        tempList.AddRange(temp);
                        break;
                    }
                    k++;
                }
                i++;
                if (blockName == tempBlockName)
                    break;
            }
            //Takes the list of strings for the block and finds the individual entities in it so that they can be returned to 
            //a new list that can take all of its entities and be drawn later
            insertListFull = blockEnt.getBlockInfo(tempList, blockFile, xScale, yScale, offsetX, offsetY, insertHandle, rotation);
            return insertListFull;
       }

    }
}
