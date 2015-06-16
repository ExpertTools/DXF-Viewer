using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXF_Viewer
{
    class RotationAngle
    {
        /// <summary>
        /// gets the rotation angle 
        /// </summary>
        /// <param name="startPoint">The beginning point</param>
        /// <param name="endPoint">The end Point</param>
        /// <returns>the angle in degrees so that the entity can be rotated correctly</returns>
        public double rotationAngle(Point startPoint, Point endPoint)
        {
            double rotationAngle = 0;

            //Find the change in x values
            double xVal = Math.Abs(endPoint.X - startPoint.X);

            //Find the change in y values
            double yVal = Math.Abs(endPoint.Y - startPoint.Y);

            //Find the rotation angle in radians
            double radians = Math.Atan(yVal / xVal);

            //Converts the rotation angle to degrees
            rotationAngle = radians * (180 / Math.PI);

            //Returns the rotation angle
            return rotationAngle;
        }
    }
}
