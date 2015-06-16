using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace DXF_Viewer
{
    class Blocks : Entity_Old
    {
        public static List<String> blockList;

        public void getBlockInfo(string lineIn, string blockName)
        {
            blockList = new List<string>();
            List<String> blocksList = new List<String>();
            StreamReader dxfFileIn = new StreamReader(new FileStream(lineIn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            String temp = dxfFileIn.ReadLine();
            List<String> blockInfo = new List<String>();
            InsertEntity insert = new InsertEntity();
            String tempName = null;

            while (temp != "BLOCKS")
            {
                temp = dxfFileIn.ReadLine();
            }
            while (temp != "ENDSEC")
            {
                temp = dxfFileIn.ReadLine();
                blockList.Add(temp);
                while (temp != "  0")
                {
                    if (temp == "ENDBLK" || temp == "ENDSEC")
                        break;
                    if (temp == "  3")
                    {
                        temp = dxfFileIn.ReadLine();
                        tempName = temp;
                        if (tempName == blockName)
                        {
                            MessageBox.Show(blockName + " " + tempName);
                            blockList = new List<String>();
                        }
                    }
                    blockList.Add(temp);
                    if (temp == "ENDSEC") break;
                    if (tempName == blockName)
                    {
                        blockInfo = blockList;
                        blockList = new List<String>();
                    }
                    temp = dxfFileIn.ReadLine();
                }
            }
            dxfFileIn.Close();
        }
    }
}