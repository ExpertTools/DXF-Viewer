using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXF_Viewer
{
    class TransformPoint : Entity_Old
    {
        /// <summary>
        /// takes the original point and makes it into a new point so that 
        /// it will be moved to the correct point on the canvas
        /// </summary>
        /// <param name="originalPoint">The original point that was passed in</param>
        /// <returns>new point on the canvas where it should be</returns>
        public Point transformPoint(Point originalPoint, double height, double minX, double minY)
        {
            Point newPoint = new Point();
            
            //adds to the original point so that it is shown on the canvas in the correct spot
            newPoint.X = (originalPoint.X) - minX + 0.5 ;
            newPoint.Y = height - (originalPoint.Y - minY) + 0.5 ;
            return newPoint;

        }
        public Point transformPoint2(Point originalPoint, double height, double minX, double minY)
        {
            Point newPoint = new Point();

            //adds to the original point so that it is shown on the canvas in the correct spot
            newPoint.X = originalPoint.X - minX + 0.6;
            newPoint.Y = height - (originalPoint.Y - minY) + 0.5;
            return newPoint;
        }
        public Point transformPoint3(Point originalPoint, double height, double minX, double minY)
        {
            Point newPoint = new Point();

            //adds to the original point so that it is shown on the canvas in the correct spot
            newPoint.X = originalPoint.X - minX + 0.4;
            newPoint.Y = height - (originalPoint.Y - minY) + 0.5;
            return newPoint;
        }
 
    }
}
