using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DXF.Extensions;
using System.Windows.Media;
using System.Windows;
using DXF.Util;

namespace DXF.GeneralInformation
{
    class DrawingStyle
    {
        public string name;
        public string fontFileName;
        public double textHeight;
        public double widthFactor;
        public double obliqueAngle;
        public bool mirrorX;
        public bool mirrorY;


        public DrawingStyle()
        {
        }

        public DrawingStyle (DrawingStyle style)
        {
            name = style.name;
            fontFileName = style.fontFileName;
            textHeight = style.textHeight;
            widthFactor = style.widthFactor;
            obliqueAngle = style.obliqueAngle;
            mirrorX = style.mirrorX;
            mirrorY = style.mirrorY;
        }


        public DrawingStyle parse(List<string> section)
        {
            int i = 0;
            while(i <= section.Count - 1)
            {
                switch(section[i])
                {
                    /* Style name */
                    case "  2":
                        name = section[++i];
                        break;
                    /* Primary font file name */
                    case "  3":
                        fontFileName = section[++i];
                        break;
                    /* Bigfont filename */
                    case "  4":
                        i++;
                        break;
                    /* Fixed Text height; 0 if not fixed */
                    case " 40":
                        textHeight = section[++i].ConvertToDoubleWithCulture();
                        break;
                    /* Width factor */
                    case " 41":
                        widthFactor = section[++i].ConvertToDoubleWithCulture();
                        break;
                    /* Last height used */
                    case " 42":
                        i++;
                        break;
                    /* Oblique Angle */
                    case " 50":
                        obliqueAngle = section[++i].ConvertToDoubleWithCulture();
                        break;
                    /* Standard Flag values (bit coded) */
                    case " 70":
                        i++;
                        break;
                    /* Mirror setting: 2 = backwards, 4 = upside down, 6 = both */
                    case " 71":
                        int flag = Convert.ToInt32(section[++i]);
                        mirrorX = flag == 2 || flag == 6;
                        mirrorY = flag == 4 || flag == 6;
                        break;
                    default:
                        i++;
                        break;
                }
            }
            return this;
        }

        public string getFontFamily()
        {
            if (fontFileName.Contains('.'))
            {
                return fontFileName.Substring(0, fontFileName.IndexOf('.'));
            }
            else
            {
                return fontFileName;
            }
        }

        public ScaleTransform getMirrorTransform(Point origin)
        {
            return ViewerHelper.getMirrorTransform(mirrorX, mirrorY, origin);
        }
    }
}
