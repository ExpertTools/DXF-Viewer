using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXF_Viewer
{
    /// <summary>
    /// finds the length of the axis for use in bulge and arc
    /// </summary>
    class AxisLength
    {   
        /// <summary>
        /// Does the math to get the correct length of the axis 
        /// </summary>
        /// <param name="startPoint">The beginning point of the axis</param>
        /// <param name="endPoint">The end point of the axis</param>
        /// <returns> the length of the axis</returns>
        public double axisLength(Point startPoint, Point endPoint)
        {
            //Finds the length of the hy-pot-in-use of the triangle
            double xValSqrd = Math.Pow((endPoint.X - startPoint.X), 2);
            double yValSqrd = Math.Pow((endPoint.Y - startPoint.Y), 2);
            double length = Math.Sqrt(xValSqrd + yValSqrd);

            return length;

        }
    }
}
