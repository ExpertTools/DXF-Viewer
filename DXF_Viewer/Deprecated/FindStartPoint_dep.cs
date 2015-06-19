using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXF_Viewer
{
    /// <summary>
    /// finds the starting point of the arc from the given information
    /// </summary>
    class FindStartPoint
    {
        /// <summary>
        /// Gets a start point from the center point, radius, and start angle
        /// </summary>
        /// <param name="centerPoint">The point between the start point and end point</param>
        /// <param name="radius">The radius of the entity</param>
        /// <param name="startAngle">The start angle</param>
        /// <returns>the start point for the arc</returns>
        public Point findStartPoint(Point centerPoint, double radius, double startAngle)
        {
            //Finds the start point of the arc
            double xVal = (Math.Cos(startAngle * (Math.PI / 180)) * radius) + centerPoint.X;
            double yVal = centerPoint.Y - (Math.Sin(startAngle * (Math.PI / 180)) * radius);

            //Creates the point and sets its values to the start point
            Point newPoint = new Point();
            newPoint.X = xVal;
            newPoint.Y = yVal;

            //Returns the point
            return newPoint;
        }

    }
}
