using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXF_Viewer
{
    /// <summary>
    /// Finds the end point from the given values
    /// </summary>
    class FindEndPoint
    {
        /// <summary>
        /// Gets the endpoint from the centerPoint the radius and the end angle
        /// </summary>
        /// <param name="centerPoint">The point between start and end point</param>
        /// <param name="radius">the radius of the entity</param>
        /// <param name="endAngle">The end angle</param>
        /// <returns>The end point for the entity</returns>
        public Point findEndPoint(Point centerPoint, double radius, double endAngle)
        {
            //Finds the end point of the arc
            double xVal = (Math.Cos(endAngle * (Math.PI / 180)) * radius) + centerPoint.X;
            double yVal = centerPoint.Y - (Math.Sin(endAngle * (Math.PI / 180)) * radius);

            //Creates the point and sets its values to the start point
            Point newPoint = new Point();
            newPoint.X = xVal;
            newPoint.Y = yVal;

            //Returns the point
            return newPoint;
        }
    }
}
