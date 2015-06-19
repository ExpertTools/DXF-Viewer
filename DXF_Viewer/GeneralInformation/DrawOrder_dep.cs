using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace DXF_Viewer
{
    class DrawOrder_dep
    {
        /// <summary>
        /// Determines the draw order of the entities to be drawn
        /// </summary>
        /// <param name="lineIn">List of handles in the order to be drawn</param>
        /// <returns> Returns the correct order of entities according to the dxf file</returns>
          public List<string> getDrawOrder(List<string> lineIn)
        {
            int j = 0;
            string order = null;
            string tempOrder = null;
            List<string> tempOrderList = new List<string>();
            List<string> orderList = new List<string>();
            SortedList<string, string> sortedOrder = new SortedList<string, string>();

            while (lineIn[j] != "AcDbSortentsTable")
            {
                j++;
            }
            j++;
            while (lineIn[j] != "AcDbSortentsTable")
            {
                j++;
            }
            while (lineIn[j] != "  0")
            {
                //looks for the handle that is used once sorted
                if (lineIn[j] == "331")
                    order = (lineIn[++j]);
                //looks for the handle to be sorted by
                if (lineIn[j] == "  5")
                    tempOrder = (lineIn[++j]);
                j++;
                //makes sure that the lists are not empty before sorting so that no errors are thrown
                if (order != null && tempOrder != null)
                {
                    int numInTempOrder = tempOrder.Count();
                    if (numInTempOrder < 8)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(tempOrder);
                        if (numInTempOrder == 7)
                        {
                            sb.Insert(0,'0');
                            tempOrder = sb.ToString();
                        }
                        if (numInTempOrder == 6)
                        {
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            tempOrder = sb.ToString(); 
                        }
                        if (numInTempOrder == 5)
                        {
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            tempOrder = sb.ToString();
                        }
                        if (numInTempOrder == 4)
                        {
                            sb.Insert(0, '0'); 
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            tempOrder = sb.ToString();
                        }
                        if (numInTempOrder == 3)
                        {
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            sb.Insert(0, '0');
                            tempOrder = sb.ToString();
                        }
                    }
                    sortedOrder.Add(tempOrder, order);
                    order = null;
                    tempOrder = null;
                }
                if (lineIn[j] == "  0")
                    break;
            }
            //make a new list to be passed out so that the order is correct
            tempOrderList = new List<string>(sortedOrder.Values);
            //adds the string to the orderList which is the one that is returned
            foreach(string s in SortByLength(tempOrderList))
            {
                orderList.Add(s);
            }

            return orderList;
        }
          //Sorts the list again to make sure that it is sorted the way humans would read it and
          //not the way a computer thinks that it should be sorted by
          //like aa1 comes before b2
          static IEnumerable<string> SortByLength(IEnumerable<string> e)
          {
              // Use LINQ to sort the array received and return a copy.
              var sorted = from s in e
                           orderby s.Length ascending
                           select s;
              return sorted;
          }
    }
}
