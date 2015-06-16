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
using DXF.GeneralInformation;

namespace DXF_Viewer
{
    class MTextEntity : Entity_Old
    {

        string text = "";
        Point anchor = new Point(0, 0);
        double size = 1;
        double rectangleWidth = 10;
        double rectangleHeight = 10;
        double charWidth = 1;
        double angle = 0;
        DrawingStyle style;




        /// <summary>
        /// Gets and sets the Path of the entity Mtext which is multiple lines of text
        /// </summary>
        /// <param name="listIn"> a list of strings which includes all the data for the mtext entity</param>
        /// <param name="layerList"> a list of layer information that includes line type and line color</param>
        /// <param name="xOffsetIn">if this entity is part of a block the x coordinate offset value</param>
        /// <param name="yOffsetIn">if this entity is part of a block the y coordinate offset value</param>
        /// <returns></returns>
        public Path getInfo(List<String> listIn, List<LayerInfo_dep> layerList, double height, double minX, double minY, double xOffsetIn, double yOffsetIn)
        {

            double pointX = 0;
            double pointY = 0;
            double size = 0;
            double rotation = 0;
            double vectorX = 0;
            double vectorY = 0;
            int justification = 0;
            int lineColor = 0;
            String text = null;
            String layerName = "0";
            double rotate = 0;

            j++;

            while (j <= listIn.Count - 1)
            {
                //The x coordinate of the start Point
                if (listIn[j] == " 10")
                {
                    pointX = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The y coordinate of the start Point
                else if (listIn[j] == " 20")
                {
                    pointY = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The x coordinate of the End point
                else if (listIn[j] == " 11")
                {
                    vectorX = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The y coordinate of the End point
                else if (listIn[j] == " 21")
                {
                    vectorY = listIn[++j].ConvertToDoubleWithCulture();
                    //the rotation of the mtext entity 
                    rotation = Math.Atan2(vectorY, vectorX);
                }
                //The actual text that is to be used 
                else if (listIn[j] == "  1")
                {
                    text = listIn[++j];                        
                }
                //The size of the text that is to be written
                else if (listIn[j] == " 40")
                {
                    size = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The rotation of the text
                else if (listIn[j] == " 50")
                {
                    rotate = listIn[++j].ConvertToDoubleWithCulture();
                }
                //The color of the mtext entity
                else if (listIn[j] == " 62")
                {
                    lineColor = Convert.ToInt32(listIn[++j]);
                }
                //The name of the layer to be used 
                else if (listIn[j] == "  8")
                {
                    layerName = listIn[++j];
                }
                //This determines the drawing direction
                else if (listIn[j] == " 72")
                {
                    //Top to bottom
                    if (listIn[++j] == "1")
                    {
                        justification = 2;
                    }
                    //By style
                    else if (listIn[j] == "2")
                    {
                        justification = 3;
                    }
                    //Left to right
                    else
                    {
                        justification = 1;
                    }
                }
                else if (listIn[j] == "ROTATION")
                {
                    rotation = listIn[++j].ConvertToDoubleWithCulture();
                }
                else if (listIn[j] == "OFFSET")
                {
                    xOffsetIn = listIn[++j].ConvertToDoubleWithCulture();
                    yOffsetIn = listIn[++j].ConvertToDoubleWithCulture();
                }
                else if (listIn[j] == "  0")
                    break;

                j++;
            }

            return buildMText(pointX, pointY, size, text, justification, rotation, lineColor, layerName, layerList, height, minX, minY, rotate, xOffsetIn, yOffsetIn);

        }

        /// <summary>
        /// This will build the path for the Mtext so that it will be drawn at a later time
        /// </summary>
        /// <param name="xIn"> the x coordinate of the start point of the mtext</param>
        /// <param name="yIn"> the y coordinate of the start point of the mtext</param>
        /// <param name="size">used to calculate the size of the font to be used</param>
        /// <param name="text">The actual text that is to be written</param>
        /// <param name="textLocation">How the text should be drawn, left to right, top to bottom, etc...</param>
        /// <param name="rotationAngle">The angle that the text will be rotated</param>
        /// <param name="lineColor">The color of the mtext entity</param>
        /// <param name="layerName">The name of the layer that mtext will use</param>
        /// <param name="layerList">a list of layer information to include line color</param>
        /// <param name="xOffset">if part of a block this is the x coordinate offset</param>
        /// <param name="yOffset">if part of a block this is the y coordinate offset</param>
        /// <returns>The path of the mtext to be drawn later</returns>
        public Path buildMText(double xIn, double yIn, double size, string text, int textLocation, double rotationAngle, int lineColor, String layerName, List<LayerInfo_dep> layerList, double height, double minX, double minY, double rotate, double xOffset = 0, double yOffset = 0)
        {
            double fontSize = 0;

            //***This section is for converting old dxf codes to the 
            //***proper string representations.
            text = text.Replace("%%C", "Ø");
            text = text.Replace("%%c", "Ø");
            text = text.Replace(@"\U+2205", "Ø");
            text = text.Replace("%%D", "°");
            text = text.Replace("%%d", "°");
            text = text.Replace(@"\U+00B0", "°");
            text = text.Replace("%%P", "±");
            text = text.Replace("%%p", "±");
            text = text.Replace(@"\U+00B1", "±");
            
            //***End of conversion section.


            //Convert the inch size from dxf to the point size used here
            fontSize = .05 + size * (96 / 72);

            
            //Create a point to store the location of the text
            Point textPoint = new Point(xIn, yIn);

            Path mTextPath = new Path();

            //Takes the "\\P" out of the text which was being written before
            text = text.Replace(@"\P", Environment.NewLine);
            
            //Checks to see if the mtext contains characters telling it new lines or new paragraphs and then removes those characters
            //and puts the next line on a new line for itself
            if(text.Contains("{\\C"))
            {
                int i = 0;
                char charToDelete = ';';

                char[] textAsCharArray;
                char[] newLineColor = {' ',' ',' '};
                text = text.TrimStart('{', '\\', 'C');
                textAsCharArray = text.ToCharArray();

                while (textAsCharArray[i] != ';')
                {
                    newLineColor[i] = textAsCharArray[i];
                    i++;
                }

                int charIndex = Array.IndexOf(textAsCharArray, charToDelete);
                textAsCharArray = textAsCharArray.Where((val, idx) => idx != charIndex).ToArray();

                string temp = new string(newLineColor);
                lineColor = Convert.ToInt32(temp);
                text = new string(textAsCharArray);

                char[] checkChar={'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                text = text.TrimStart(checkChar);              
                text = text.TrimEnd('}');
            }
            //removes extra CAD formmatting but does not use it to format.
            if (text.Contains("{\\f"))
            {
                text = text.Substring(text.IndexOf(";")+1);
                text = text.TrimEnd('}');
            }
            if (text.Contains("\\A"))
            {
                text = text.Substring(text.IndexOf(";") + 1);
                //Console.WriteLine(text);
                /*
                if (text.Length > 2)
                {
                    byte symbol = Encoding.Convert(Encoding.UTF7, Encoding.UTF8, Encoding.UTF7.GetBytes(text))[2];
                    Console.WriteLine(Encoding.Unicode.GetBytes(text)[2]);
                    if (symbol == 239)
                    {
                        text = text.Substring(0, text.Length - 1);
                        text += "°";
                    }
                }
                 */
            }
            //This is to replace the bad alt-code insertions of degree symbols in some schematics.
            //The CAD standard is "%%D" but some schematics have the degree symbol inserted with copy/paste.
            //The copy/paste value is saved in the dxf file as a hex value xB0 and when using this viewer
            //will produce a ? symbol instead of the degree symbol.
            //This scans the text entity being made and looks at each char at a byte level after converting
            //to a standard encoding. If the value matches the one for the degree symbol then it is replaced
            //if not it gets appended to the output string.
            
            //I do not believe the encoding swap is nessescary as xB0 = 176 in decimal which is the unicode
            //value for degree.
            //If just looked at as unicode perhaps some trouble could be spared
            String afterText = "";
            foreach (char currentChar in text.ToCharArray())
            {
                //byte symbol = Encoding.Convert(Encoding.UTF8, Encoding.UTF8, Encoding.UTF7.GetBytes(currentChar.ToString()))[0];
                byte symbol = Encoding.UTF8.GetBytes(currentChar.ToString())[0];
                //Console.WriteLine(symbol);
                if (symbol == 239)
                {
                    if (text.IndexOf(currentChar) == text.Length - 1)
                        afterText += "°";
                    else
                        afterText += "⌀";
                }
                else
                {
                    afterText = afterText + currentChar;
                }
            }
            text = afterText;
            //Console.WriteLine(afterText);
            
            FormattedText formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Tahoma"), fontSize, Brushes.White);
            
            //Create a string array to hold text that has been split
            String[] splitText = splitTextApart(text);

            //convert text location from upper left to lower left
            TransformPoint transPoint = new TransformPoint();
            textPoint = transPoint.transformPoint(textPoint, height, minX, minY);

            //Sets the line color for the mtext if it was not already set by looking at the layer information
            if (lineColor == 0)
            {
                foreach (LayerInfo_dep tempLayer in layerList)
                {
                    if (tempLayer.layerName == layerName)
                    {
                        lineColor = tempLayer.lineColor;
                    }
                }
            }
            //gets and sets the color for the entity
            ColorSorter cs = new ColorSorter();
            Color color = new Color();
            color = cs.getColor(lineColor);
            Brush tempBrush = new SolidColorBrush(color);
            mTextPath.Stroke = tempBrush;
            
            //sets the stroke thickness to 0 so that just the fill will show up
            //changed from 0 to 0.15, 2013-Dec-13 S. Damman
            mTextPath.StrokeThickness = 0;

            //sets the fill of the text to the same color it was drawn in
            mTextPath.Fill = tempBrush;

            //sets the data of the path to the geometry of the mtext
            mTextPath.Data = formattedText.BuildGeometry(textPoint);
            //gets the rotation of the line if it is part of a rotated block
            if (rotate != 0)
            {
                Point centerPointForRotate = new Point();
                centerPointForRotate = transPoint.transformPoint(new Point(xOffset, yOffset), height, minX, minY);
                mTextPath.RenderTransform = new RotateTransform(-rotate, centerPointForRotate.X, centerPointForRotate.Y);
            }
            return mTextPath;

        }

        /// <summary>
        /// Splits up the mtext by line so that it makes a new part of the array for each line
        /// </summary>
        /// <param name="text">The text that is to be drawn</param>
        /// <returns>The text split up by line</returns>
        private String[] splitTextApart(String text)
        {

            char[] myChars = { '\\','P' };

            string[] splitText = text.Split(myChars, StringSplitOptions.RemoveEmptyEntries);

            return splitText;
        }

    }

}
