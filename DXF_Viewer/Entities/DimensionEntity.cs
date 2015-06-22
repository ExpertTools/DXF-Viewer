using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Globalization;
using System.Text.RegularExpressions;
using DXF.Extensions;

namespace DXF_Viewer
{
    class DimensionEntity : Entity_Old
    {
        public enum DimensionType { ROTATED, ALIGNED, ANGULAR, DIAMETER, RADIUS, THREE_ANGULAR, ORDINATE};
        public enum AttachmentPoint { TOP_LEFT, TOP_CENTER, TOP_RIGHT, MIDDLE_LEFT, MIDDLE_CENTER, MIDDLE_RIGHT, BOTTOM_LEFT, BOTTOM_CENTER, BOTTOM_RIGHT };
        public Path getInfo(List<String> listIn, List<LayerInfo_dep> layerList, double height, double minX, double minY, double xOffsetIn, double yOffsetIn)
        {
            string blockSoftID = ""; //2 Block Record name
            double xPosIn = 0; // 10
            double yPosIn = 0; // 20
            double textMidpointX = 0; //11
            double textMidpointY = 0; //21
            DimensionType type = DimensionType.ROTATED; //70
            AttachmentPoint relativeTo = AttachmentPoint.TOP_LEFT; //71
            string measurementValue = ""; //42
            int j = 0;
            j++;
            while(j <= listIn.Count -1)
            {
                //Block ID
                if (listIn[j] == "  2")
                {
                    blockSoftID = listIn[++j];
                }
                else if (listIn[j] == " 10")
                {
                    xPosIn = listIn[++j].ConvertToDoubleWithCulture();
                }
                else if (listIn[j] == " 20")
                {
                    yPosIn = listIn[++j].ConvertToDoubleWithCulture();
                }
                else if (listIn[j] == " 11")
                {
                    textMidpointX = listIn[++j].ConvertToDoubleWithCulture();
                }
                else if (listIn[j] == " 21")
                {
                    textMidpointY = listIn[++j].ConvertToDoubleWithCulture();
                }
                else if (listIn[j] == " 70")
                {
                    int dimensionTypeRaw = (int)Math.Floor(listIn[++j].ConvertToDoubleWithCulture());
                    if (dimensionTypeRaw - 32 < 0)
                    {
                        //this is not a singular dimension
                        //probably not likely within the current scope of this project
                    }
                    else
                    {
                        //the dxf spec uses by-bit to store extra information  in some 
                        //scenarios (ordinate) because of this we only care about the 
                        //the first 4 bits, or rather the numbers (0-6).
                        dimensionTypeRaw -= 32;
                    }
                    switch (dimensionTypeRaw)
                    {

                        case 0:
                            type = DimensionType.ROTATED;
                            break;
                        case 1:
                            type = DimensionType.ALIGNED;
                            break;
                        case 2:
                            type = DimensionType.ANGULAR;
                            break;
                        case 3:
                            type = DimensionType.DIAMETER;
                            break;
                        case 4:
                            type = DimensionType.RADIUS;
                            break;
                        case 5:
                            type = DimensionType.THREE_ANGULAR;
                            break;
                        case 6:
                            type = DimensionType.ORDINATE;
                            break;
                        default:
                            //this is the 64/128 cases which function as extra
                            //information for user defined positions.
                            //For our purposed it could be assumed that the dimension
                            //type is rotated
                            type = DimensionType.ROTATED;
                            break;
                    }
                }
                else if (listIn[j] == " 71")
                {
                    int attachmentPointRaw = (int)Math.Floor(listIn[++j].ConvertToDoubleWithCulture());
                    switch (attachmentPointRaw)
                    {
                        case 1:
                            relativeTo = AttachmentPoint.TOP_LEFT;
                            break;
                        case 2:
                            relativeTo = AttachmentPoint.TOP_CENTER;
                            break;
                        case 3:
                            relativeTo = AttachmentPoint.TOP_RIGHT;
                            break;
                        case 4:
                            relativeTo = AttachmentPoint.MIDDLE_LEFT;
                            break;
                        case 5:
                            relativeTo = AttachmentPoint.MIDDLE_CENTER;
                            break;
                        case 6:
                            relativeTo = AttachmentPoint.MIDDLE_RIGHT;
                            break;
                        case 7:
                            relativeTo = AttachmentPoint.BOTTOM_LEFT;
                            break;
                        case 8:
                            relativeTo = AttachmentPoint.BOTTOM_CENTER;
                            break;
                        case 9:
                            relativeTo = AttachmentPoint.BOTTOM_RIGHT;
                            break;
                        default:
                            relativeTo = AttachmentPoint.MIDDLE_CENTER;
                            break;
                    }
                }
                else if (listIn[j] == " 42")
                {
                    measurementValue = listIn[++j];
                }
            }
            //check the block record ID to find the geometry associated
            //with the dimension.

            return null;
        }
        public Path buildDimension(double xIn, double yIn, double size, string text, int textLocation, double rotationAngle, int lineColor, String layerName, List<LayerInfo_dep> layerList, double height, double minX, double minY, double rotate, double xOffset = 0, double yOffset = 0)
        {
            return null;
        }
    }
}
