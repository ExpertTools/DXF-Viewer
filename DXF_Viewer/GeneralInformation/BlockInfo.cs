using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXF_Viewer
{
    class BlockInfo
    {

        List<List<string>> fullBlockList = new List<List<string>>();
        /// <summary>
        /// Get the information from the Blocks section of the dxf file
        /// </summary>
        /// <param name="lineIn">The full dxf file as a list of strings</param>
        /// <returns></returns>
        public List<List<string>> getBlockInfo(List<String> lineIn)
        {
            int j = 0;
            List<string> blockList = new List<string>();

            //Looks through the list until it hits the blocks section
            while (lineIn[j] != "BLOCKS")
            {
                j++;
            }
            //Once the block section is hit it will then look for each individual
            //block until endsec it hit
            while (lineIn[j] != "ENDSEC")
            {
                if (lineIn[j] == "BLOCK")
                {
                    //Once a block is hit until the end of the block each line is 
                    //added to blocklist until the endblk is hit
                    while (lineIn[j] != "ENDBLK")
                    {
                        blockList.Add(lineIn[j]);
                        j++;
                    }
                    //Each indivdidual blocklist is added to fullblocklist which is a list 
                    //of lists of block information
                    fullBlockList.Add(blockList);
                    blockList = new List<string>();
                }
                j++;
                if (lineIn[j] == "ENDSEC")
                    break;
            }
            return fullBlockList;
        }
    }
}
