using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace DXF_Viewer
{
    class SetLineType
    {
        /// <summary>
        /// Takes the line type as a string so that the type of line can be 
        /// used to draw the entity with the correct line type
        /// </summary>
        /// <param name="lineType">line type as a string</param>
        /// <param name="scale"></param>
        /// <returns>the double collection of the line type</returns>
        public DoubleCollection setLineType(string lineType, double scale = 1.0)
        {
            DoubleCollection collection = new DoubleCollection();

            //Set the collection to center
            if (lineType == "CENTER")
            {
                collection.Add(9 * scale);
                collection.Add(3 * scale);
                collection.Add(3 * scale);
                collection.Add(3 * scale);
            }

            //Sets the collection to hidden
            else if (lineType == "HIDDEN2" || lineType == "HIDDEN")
            {
                collection.Add(3 * scale);
                collection.Add(3 * scale);
            }
            //Sets the collection to continuous
            else if (lineType == "CONTINUOUS")
            {
                collection.Add(1 * scale);
                collection.Add(0);
            }
            //Returns the collection to set the line type
            return collection;
        }
    }
}
