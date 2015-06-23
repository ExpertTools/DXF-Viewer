using DXF.GeneralInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace DXF.Util
{
    class ViewerHelper
    {
        /// <summary>
        ///     Helper method to assist in mapping DXF style coordinates to WPF
        ///     coordinates.
        /// </summary>
        /// <param name="target">The DXF point to transform</param>
        /// <param name="height"></param>
        /// <param name="xMin">The minimum x axis value from DXF extents</param>
        /// <param name="yMin">The minimum y axis value from DXF extents</param>
        /// <returns></returns>
        public static Point mapToWPF(Point target, double height, double xMin, double yMin)
        {
            Point wpfPoint = new Point();
            wpfPoint.X = target.X - xMin + 0.5;
            wpfPoint.Y = height - (target.Y - yMin) + 0.5;
            return wpfPoint;
        }
        public static Point mapToWPF(Point target, DrawingInfo_dep drawing)
        {
            Point wpfPoint = new Point();
            wpfPoint.X = target.X - drawing.xMin + 0.5;
            wpfPoint.Y = drawing.height - (target.Y - drawing.yMin) + 0.5;
            return wpfPoint;
        }

        public static Point mapToWPF(Point target, Schematic drawing)
        {
            Point wpfPoint = new Point();
            wpfPoint.X = target.X - drawing.header.xMin + 0.5;
            wpfPoint.Y = (drawing.header.yMax - drawing.header.yMin) - (target.Y - drawing.header.yMin) + 0.5;
            return wpfPoint;
        }

        public static double calculateAxisLength(Point start, Point end)
        {
            double x = Math.Pow((end.X - start.X), 2);
            double y = Math.Pow((end.Y - start.Y), 2);
            return Math.Sqrt(x + y);
        }

        public static double calculateRotationAngle(Point start, Point end)
        {
            double x = Math.Abs(end.X - start.X);
            double y = Math.Abs(end.Y - start.Y);
            double theta = Math.Atan(y / x);
            return theta * (180 / Math.PI);
        }

        public static string swtichDXFSymbols(string text)
        {
            text = text.Replace("%%C", "Ø");
            text = text.Replace("%%c", "Ø");
            text = text.Replace(@"\U+2205", "Ø");
            text = text.Replace("%%D", "°");
            text = text.Replace("%%d", "°");
            text = text.Replace(@"\U+00B0", "°");
            text = text.Replace("%%P", "±");
            text = text.Replace("%%p", "±");
            text = text.Replace(@"\U+00B1", "±");

            text = text.Replace(@"\P", Environment.NewLine);

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
            string afterText = "";
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

            return text;
        }

        public static ScaleTransform getMirrorTransform(bool mirrorX, bool mirrorY, Point origin)
        {
            ScaleTransform x;
            if (mirrorX && mirrorY)
            {
                x = new ScaleTransform(-1.0, -1.0, origin.X, origin.Y);
            }
            else if (mirrorX)
            {
                x = new ScaleTransform(-1.0, 1.0, origin.X, origin.Y);
            }
            else if (mirrorY)
            {
                x = new ScaleTransform(1.0, -1.0, origin.X, origin.Y);
            }
            else
            {
                x = new ScaleTransform(1.0, 1.0, origin.X, origin.Y);
            }
            return x;
        }

        /// <summary>
        /// The color is taken from the integer that was given and then used 
        /// to sort from the list of hexadecimal values so that the color 
        /// can be found from the list
        /// </summary>
        /// <param name="lineColor">Line color as an integer to be found from the hexadecimal value</param>
        /// <returns>the color of the particular entity</returns>
        public static Color getColor(int lineColor)
        {
            SortedList<int, Color> colorList = new SortedList<int, Color>();

            colorList.Add(0, (Color)ColorConverter.ConvertFromString("#FFFFFF"));
            colorList.Add(1, (Color)ColorConverter.ConvertFromString("#FF0000"));
            colorList.Add(2, (Color)ColorConverter.ConvertFromString("#FFFF00"));
            colorList.Add(3, (Color)ColorConverter.ConvertFromString("#00FF00"));
            colorList.Add(4, (Color)ColorConverter.ConvertFromString("#00FFFF"));
            // Changed #5 from #0000FF to #00CCFF to make it easier to read on black 
            colorList.Add(5, (Color)ColorConverter.ConvertFromString("#00CCFF"));
            colorList.Add(6, (Color)ColorConverter.ConvertFromString("#FF00FF"));
            colorList.Add(7, (Color)ColorConverter.ConvertFromString("#FFFFFF"));
            colorList.Add(8, (Color)ColorConverter.ConvertFromString("#414141"));
            colorList.Add(9, (Color)ColorConverter.ConvertFromString("#808080"));
            colorList.Add(10, (Color)ColorConverter.ConvertFromString("#FF0000"));
            colorList.Add(11, (Color)ColorConverter.ConvertFromString("#FFAAAA"));
            colorList.Add(12, (Color)ColorConverter.ConvertFromString("#BD0000"));
            colorList.Add(13, (Color)ColorConverter.ConvertFromString("#BD7E7E"));
            colorList.Add(14, (Color)ColorConverter.ConvertFromString("#810000"));
            colorList.Add(15, (Color)ColorConverter.ConvertFromString("#815656"));
            colorList.Add(16, (Color)ColorConverter.ConvertFromString("#680000"));
            colorList.Add(17, (Color)ColorConverter.ConvertFromString("#684545"));
            colorList.Add(18, (Color)ColorConverter.ConvertFromString("#4F0000"));
            colorList.Add(19, (Color)ColorConverter.ConvertFromString("#4F3535"));
            colorList.Add(20, (Color)ColorConverter.ConvertFromString("#FF3F00"));
            colorList.Add(21, (Color)ColorConverter.ConvertFromString("#FFBFAA"));
            colorList.Add(22, (Color)ColorConverter.ConvertFromString("#BD2E00"));
            colorList.Add(23, (Color)ColorConverter.ConvertFromString("#BD8D7E"));
            colorList.Add(24, (Color)ColorConverter.ConvertFromString("#811F00"));
            colorList.Add(25, (Color)ColorConverter.ConvertFromString("#816056"));
            colorList.Add(26, (Color)ColorConverter.ConvertFromString("#681900"));
            colorList.Add(27, (Color)ColorConverter.ConvertFromString("#684E45"));
            colorList.Add(28, (Color)ColorConverter.ConvertFromString("#4F1300"));
            colorList.Add(29, (Color)ColorConverter.ConvertFromString("#4F3B35"));
            colorList.Add(30, (Color)ColorConverter.ConvertFromString("#FF7F00"));
            colorList.Add(31, (Color)ColorConverter.ConvertFromString("#FFD4AA"));
            colorList.Add(32, (Color)ColorConverter.ConvertFromString("#BD5E00"));
            colorList.Add(33, (Color)ColorConverter.ConvertFromString("#BD9D7E"));
            colorList.Add(34, (Color)ColorConverter.ConvertFromString("#814000"));
            colorList.Add(35, (Color)ColorConverter.ConvertFromString("#816B56"));
            colorList.Add(36, (Color)ColorConverter.ConvertFromString("#683400"));
            colorList.Add(37, (Color)ColorConverter.ConvertFromString("#685645"));
            colorList.Add(38, (Color)ColorConverter.ConvertFromString("#4F2700"));
            colorList.Add(39, (Color)ColorConverter.ConvertFromString("#4F4235"));
            colorList.Add(40, (Color)ColorConverter.ConvertFromString("#FFBF00"));
            colorList.Add(41, (Color)ColorConverter.ConvertFromString("#FFEAAA"));
            colorList.Add(42, (Color)ColorConverter.ConvertFromString("#BD8D00"));
            colorList.Add(43, (Color)ColorConverter.ConvertFromString("#BDAD7E"));
            colorList.Add(44, (Color)ColorConverter.ConvertFromString("#816000"));
            colorList.Add(45, (Color)ColorConverter.ConvertFromString("#817656"));
            colorList.Add(46, (Color)ColorConverter.ConvertFromString("#684E00"));
            colorList.Add(47, (Color)ColorConverter.ConvertFromString("#685F45"));
            colorList.Add(48, (Color)ColorConverter.ConvertFromString("#4F3B00"));
            colorList.Add(49, (Color)ColorConverter.ConvertFromString("#4F4935"));
            colorList.Add(50, (Color)ColorConverter.ConvertFromString("#FFFF00"));
            colorList.Add(51, (Color)ColorConverter.ConvertFromString("#FFFFAA"));
            colorList.Add(52, (Color)ColorConverter.ConvertFromString("#BDBD00"));
            colorList.Add(53, (Color)ColorConverter.ConvertFromString("#BDBD7E"));
            colorList.Add(54, (Color)ColorConverter.ConvertFromString("#818100"));
            colorList.Add(55, (Color)ColorConverter.ConvertFromString("#818156"));
            colorList.Add(56, (Color)ColorConverter.ConvertFromString("#686800"));
            colorList.Add(57, (Color)ColorConverter.ConvertFromString("#686845"));
            colorList.Add(58, (Color)ColorConverter.ConvertFromString("#4F4F00"));
            colorList.Add(59, (Color)ColorConverter.ConvertFromString("#4F4F35"));
            colorList.Add(60, (Color)ColorConverter.ConvertFromString("#BFFF00"));
            colorList.Add(61, (Color)ColorConverter.ConvertFromString("#EAFFAA"));
            colorList.Add(62, (Color)ColorConverter.ConvertFromString("#8DBD00"));
            colorList.Add(63, (Color)ColorConverter.ConvertFromString("#ADBD7E"));
            colorList.Add(64, (Color)ColorConverter.ConvertFromString("#608100"));
            colorList.Add(65, (Color)ColorConverter.ConvertFromString("#768156"));
            colorList.Add(66, (Color)ColorConverter.ConvertFromString("#4E6800"));
            colorList.Add(67, (Color)ColorConverter.ConvertFromString("#5F6845"));
            colorList.Add(68, (Color)ColorConverter.ConvertFromString("#3B4F00"));
            colorList.Add(69, (Color)ColorConverter.ConvertFromString("#494F35"));
            colorList.Add(70, (Color)ColorConverter.ConvertFromString("#7FFF00"));
            colorList.Add(71, (Color)ColorConverter.ConvertFromString("#D4FFAA"));
            colorList.Add(72, (Color)ColorConverter.ConvertFromString("#5EBD00"));
            colorList.Add(73, (Color)ColorConverter.ConvertFromString("#9DBD7E"));
            colorList.Add(74, (Color)ColorConverter.ConvertFromString("#408100"));
            colorList.Add(75, (Color)ColorConverter.ConvertFromString("#6B8156"));
            colorList.Add(76, (Color)ColorConverter.ConvertFromString("#346800"));
            colorList.Add(77, (Color)ColorConverter.ConvertFromString("#566845"));
            colorList.Add(78, (Color)ColorConverter.ConvertFromString("#274F00"));
            colorList.Add(79, (Color)ColorConverter.ConvertFromString("#424F35"));
            colorList.Add(80, (Color)ColorConverter.ConvertFromString("#3FFF00"));
            colorList.Add(81, (Color)ColorConverter.ConvertFromString("#BFFFAA"));
            colorList.Add(82, (Color)ColorConverter.ConvertFromString("#2EBD7E"));
            colorList.Add(83, (Color)ColorConverter.ConvertFromString("#8DBD7E"));
            colorList.Add(84, (Color)ColorConverter.ConvertFromString("#1F8100"));
            colorList.Add(85, (Color)ColorConverter.ConvertFromString("#608156"));
            colorList.Add(86, (Color)ColorConverter.ConvertFromString("#196800"));
            colorList.Add(87, (Color)ColorConverter.ConvertFromString("#4E6845"));
            colorList.Add(88, (Color)ColorConverter.ConvertFromString("#134F00"));
            colorList.Add(89, (Color)ColorConverter.ConvertFromString("#3B4F35"));
            colorList.Add(90, (Color)ColorConverter.ConvertFromString("#00FF00"));
            colorList.Add(91, (Color)ColorConverter.ConvertFromString("#AAFFAA"));
            colorList.Add(92, (Color)ColorConverter.ConvertFromString("#00BD00"));
            colorList.Add(93, (Color)ColorConverter.ConvertFromString("#7EBD7E"));
            colorList.Add(94, (Color)ColorConverter.ConvertFromString("#008100"));
            colorList.Add(95, (Color)ColorConverter.ConvertFromString("#568156"));
            colorList.Add(96, (Color)ColorConverter.ConvertFromString("#006800"));
            colorList.Add(97, (Color)ColorConverter.ConvertFromString("#456845"));
            colorList.Add(98, (Color)ColorConverter.ConvertFromString("#004F00"));
            colorList.Add(99, (Color)ColorConverter.ConvertFromString("#354F35"));
            colorList.Add(100, (Color)ColorConverter.ConvertFromString("#00FF3F"));
            colorList.Add(101, (Color)ColorConverter.ConvertFromString("#AAFFBF"));
            colorList.Add(102, (Color)ColorConverter.ConvertFromString("#00BD2E"));
            colorList.Add(103, (Color)ColorConverter.ConvertFromString("#7EBD8D"));
            colorList.Add(104, (Color)ColorConverter.ConvertFromString("#00811F"));
            colorList.Add(105, (Color)ColorConverter.ConvertFromString("#568160"));
            colorList.Add(106, (Color)ColorConverter.ConvertFromString("#006819"));
            colorList.Add(107, (Color)ColorConverter.ConvertFromString("#45684E"));
            colorList.Add(108, (Color)ColorConverter.ConvertFromString("#004F13"));
            colorList.Add(109, (Color)ColorConverter.ConvertFromString("#354F3B"));
            colorList.Add(110, (Color)ColorConverter.ConvertFromString("#00FF7F"));
            colorList.Add(111, (Color)ColorConverter.ConvertFromString("#AAFFD4"));
            colorList.Add(112, (Color)ColorConverter.ConvertFromString("#00BD5E"));
            colorList.Add(113, (Color)ColorConverter.ConvertFromString("#7EBD9D"));
            colorList.Add(114, (Color)ColorConverter.ConvertFromString("#008140"));
            colorList.Add(115, (Color)ColorConverter.ConvertFromString("#56816B"));
            colorList.Add(116, (Color)ColorConverter.ConvertFromString("#006834"));
            colorList.Add(117, (Color)ColorConverter.ConvertFromString("#456856"));
            colorList.Add(118, (Color)ColorConverter.ConvertFromString("#004F27"));
            colorList.Add(119, (Color)ColorConverter.ConvertFromString("#354F42"));
            colorList.Add(120, (Color)ColorConverter.ConvertFromString("#00FFBF"));
            colorList.Add(121, (Color)ColorConverter.ConvertFromString("#AAFFEA"));
            colorList.Add(122, (Color)ColorConverter.ConvertFromString("#00BD8D"));
            colorList.Add(123, (Color)ColorConverter.ConvertFromString("#7EBDAD"));
            colorList.Add(124, (Color)ColorConverter.ConvertFromString("#008160"));
            colorList.Add(125, (Color)ColorConverter.ConvertFromString("#568176"));
            colorList.Add(126, (Color)ColorConverter.ConvertFromString("#00684E"));
            colorList.Add(127, (Color)ColorConverter.ConvertFromString("#45685F"));
            colorList.Add(128, (Color)ColorConverter.ConvertFromString("#004F3B"));
            colorList.Add(129, (Color)ColorConverter.ConvertFromString("#354F49"));
            colorList.Add(130, (Color)ColorConverter.ConvertFromString("#00FFFF"));
            colorList.Add(131, (Color)ColorConverter.ConvertFromString("#AAFFFF"));
            colorList.Add(132, (Color)ColorConverter.ConvertFromString("#00BDBD"));
            colorList.Add(133, (Color)ColorConverter.ConvertFromString("#7EBDBD"));
            colorList.Add(134, (Color)ColorConverter.ConvertFromString("#008181"));
            colorList.Add(135, (Color)ColorConverter.ConvertFromString("#568181"));
            colorList.Add(136, (Color)ColorConverter.ConvertFromString("#006868"));
            colorList.Add(137, (Color)ColorConverter.ConvertFromString("#456868"));
            colorList.Add(138, (Color)ColorConverter.ConvertFromString("#004F4F"));
            colorList.Add(139, (Color)ColorConverter.ConvertFromString("#354F4F"));
            colorList.Add(140, (Color)ColorConverter.ConvertFromString("#00BFFF"));
            colorList.Add(141, (Color)ColorConverter.ConvertFromString("#AAEAFF"));
            colorList.Add(142, (Color)ColorConverter.ConvertFromString("#008DBD"));
            colorList.Add(143, (Color)ColorConverter.ConvertFromString("#7EADBD"));
            colorList.Add(144, (Color)ColorConverter.ConvertFromString("#006081"));
            colorList.Add(145, (Color)ColorConverter.ConvertFromString("#567681"));
            colorList.Add(146, (Color)ColorConverter.ConvertFromString("#004E68"));
            colorList.Add(147, (Color)ColorConverter.ConvertFromString("#455F68"));
            colorList.Add(148, (Color)ColorConverter.ConvertFromString("#003B4F"));
            colorList.Add(149, (Color)ColorConverter.ConvertFromString("#35494F"));
            colorList.Add(150, (Color)ColorConverter.ConvertFromString("#007FFF"));
            colorList.Add(151, (Color)ColorConverter.ConvertFromString("#AAD4FF"));
            colorList.Add(152, (Color)ColorConverter.ConvertFromString("#005EBD"));
            colorList.Add(153, (Color)ColorConverter.ConvertFromString("#7E9DBD"));
            colorList.Add(154, (Color)ColorConverter.ConvertFromString("#004081"));
            colorList.Add(155, (Color)ColorConverter.ConvertFromString("#566B81"));
            colorList.Add(156, (Color)ColorConverter.ConvertFromString("#003468"));
            colorList.Add(157, (Color)ColorConverter.ConvertFromString("#455668"));
            colorList.Add(158, (Color)ColorConverter.ConvertFromString("#00274F"));
            colorList.Add(159, (Color)ColorConverter.ConvertFromString("#35424F"));
            colorList.Add(160, (Color)ColorConverter.ConvertFromString("#003FFF"));
            colorList.Add(161, (Color)ColorConverter.ConvertFromString("#AABFFF"));
            colorList.Add(162, (Color)ColorConverter.ConvertFromString("#002EBD"));
            colorList.Add(163, (Color)ColorConverter.ConvertFromString("#7E8DBD"));
            colorList.Add(164, (Color)ColorConverter.ConvertFromString("#001F81"));
            colorList.Add(165, (Color)ColorConverter.ConvertFromString("#566081"));
            colorList.Add(166, (Color)ColorConverter.ConvertFromString("#001968"));
            colorList.Add(167, (Color)ColorConverter.ConvertFromString("#454E68"));
            colorList.Add(168, (Color)ColorConverter.ConvertFromString("#00134F"));
            colorList.Add(169, (Color)ColorConverter.ConvertFromString("#353B4F"));
            colorList.Add(170, (Color)ColorConverter.ConvertFromString("#0000FF"));
            colorList.Add(171, (Color)ColorConverter.ConvertFromString("#AAAAFF"));
            colorList.Add(172, (Color)ColorConverter.ConvertFromString("#0000BD"));
            colorList.Add(173, (Color)ColorConverter.ConvertFromString("#7E7EBD"));
            colorList.Add(174, (Color)ColorConverter.ConvertFromString("#000081"));
            colorList.Add(175, (Color)ColorConverter.ConvertFromString("#565691"));
            colorList.Add(176, (Color)ColorConverter.ConvertFromString("#000068"));
            colorList.Add(177, (Color)ColorConverter.ConvertFromString("#454568"));
            colorList.Add(178, (Color)ColorConverter.ConvertFromString("#00004F"));
            colorList.Add(179, (Color)ColorConverter.ConvertFromString("#35354F"));
            colorList.Add(180, (Color)ColorConverter.ConvertFromString("#3F00FF"));
            colorList.Add(181, (Color)ColorConverter.ConvertFromString("#BFAAFF"));
            colorList.Add(182, (Color)ColorConverter.ConvertFromString("#2E00BD"));
            colorList.Add(183, (Color)ColorConverter.ConvertFromString("#8D7EBD"));
            colorList.Add(184, (Color)ColorConverter.ConvertFromString("#1F0081"));
            colorList.Add(185, (Color)ColorConverter.ConvertFromString("#605681"));
            colorList.Add(186, (Color)ColorConverter.ConvertFromString("#190068"));
            colorList.Add(187, (Color)ColorConverter.ConvertFromString("#4E4568"));
            colorList.Add(188, (Color)ColorConverter.ConvertFromString("#13004F"));
            colorList.Add(189, (Color)ColorConverter.ConvertFromString("#3B354F"));
            colorList.Add(190, (Color)ColorConverter.ConvertFromString("#7F00FF"));
            colorList.Add(191, (Color)ColorConverter.ConvertFromString("#D4AAFF"));
            colorList.Add(192, (Color)ColorConverter.ConvertFromString("#5E00BD"));
            colorList.Add(193, (Color)ColorConverter.ConvertFromString("#9D7EBD"));
            colorList.Add(194, (Color)ColorConverter.ConvertFromString("#400081"));
            colorList.Add(195, (Color)ColorConverter.ConvertFromString("#6B5681"));
            colorList.Add(196, (Color)ColorConverter.ConvertFromString("#340068"));
            colorList.Add(197, (Color)ColorConverter.ConvertFromString("#564568"));
            colorList.Add(198, (Color)ColorConverter.ConvertFromString("#27004F"));
            colorList.Add(199, (Color)ColorConverter.ConvertFromString("#42354F"));
            colorList.Add(200, (Color)ColorConverter.ConvertFromString("#BF00FF"));
            colorList.Add(201, (Color)ColorConverter.ConvertFromString("#EAAAFF"));
            colorList.Add(202, (Color)ColorConverter.ConvertFromString("#8D00BD"));
            colorList.Add(203, (Color)ColorConverter.ConvertFromString("#AD7EBD"));
            colorList.Add(204, (Color)ColorConverter.ConvertFromString("#600081"));
            colorList.Add(205, (Color)ColorConverter.ConvertFromString("#765681"));
            colorList.Add(206, (Color)ColorConverter.ConvertFromString("#4E0068"));
            colorList.Add(207, (Color)ColorConverter.ConvertFromString("#5F4568"));
            colorList.Add(208, (Color)ColorConverter.ConvertFromString("#3B004F"));
            colorList.Add(209, (Color)ColorConverter.ConvertFromString("#49354F"));
            colorList.Add(210, (Color)ColorConverter.ConvertFromString("#FF00FF"));
            colorList.Add(211, (Color)ColorConverter.ConvertFromString("#FFAAFF"));
            colorList.Add(212, (Color)ColorConverter.ConvertFromString("#BD00BD"));
            colorList.Add(213, (Color)ColorConverter.ConvertFromString("#BD7EBD"));
            colorList.Add(214, (Color)ColorConverter.ConvertFromString("#810081"));
            colorList.Add(215, (Color)ColorConverter.ConvertFromString("#815681"));
            colorList.Add(216, (Color)ColorConverter.ConvertFromString("#680068"));
            colorList.Add(217, (Color)ColorConverter.ConvertFromString("#684568"));
            colorList.Add(218, (Color)ColorConverter.ConvertFromString("#4F004F"));
            colorList.Add(219, (Color)ColorConverter.ConvertFromString("#4F354F"));
            colorList.Add(220, (Color)ColorConverter.ConvertFromString("#FF00BF"));
            colorList.Add(221, (Color)ColorConverter.ConvertFromString("#FFAAEA"));
            colorList.Add(222, (Color)ColorConverter.ConvertFromString("#BD008D"));
            colorList.Add(223, (Color)ColorConverter.ConvertFromString("#BD7EAD"));
            colorList.Add(224, (Color)ColorConverter.ConvertFromString("#810060"));
            colorList.Add(225, (Color)ColorConverter.ConvertFromString("#815676"));
            colorList.Add(226, (Color)ColorConverter.ConvertFromString("#68004E"));
            colorList.Add(227, (Color)ColorConverter.ConvertFromString("#68455F"));
            colorList.Add(228, (Color)ColorConverter.ConvertFromString("#4F003B"));
            colorList.Add(229, (Color)ColorConverter.ConvertFromString("#4F3549"));
            colorList.Add(230, (Color)ColorConverter.ConvertFromString("#FF007F"));
            colorList.Add(231, (Color)ColorConverter.ConvertFromString("#FFAAD4"));
            colorList.Add(232, (Color)ColorConverter.ConvertFromString("#BD005E"));
            colorList.Add(233, (Color)ColorConverter.ConvertFromString("#BD7E9D"));
            colorList.Add(234, (Color)ColorConverter.ConvertFromString("#810040"));
            colorList.Add(235, (Color)ColorConverter.ConvertFromString("#81566B"));
            colorList.Add(236, (Color)ColorConverter.ConvertFromString("#680034"));
            colorList.Add(237, (Color)ColorConverter.ConvertFromString("#684556"));
            colorList.Add(238, (Color)ColorConverter.ConvertFromString("#4F0027"));
            colorList.Add(239, (Color)ColorConverter.ConvertFromString("#4F3542"));
            colorList.Add(240, (Color)ColorConverter.ConvertFromString("#FF003F"));
            colorList.Add(241, (Color)ColorConverter.ConvertFromString("#FFAABF"));
            colorList.Add(242, (Color)ColorConverter.ConvertFromString("#BD002E"));
            colorList.Add(243, (Color)ColorConverter.ConvertFromString("#BD7E8D"));
            colorList.Add(244, (Color)ColorConverter.ConvertFromString("#81001F"));
            colorList.Add(245, (Color)ColorConverter.ConvertFromString("#815660"));
            colorList.Add(246, (Color)ColorConverter.ConvertFromString("#680019"));
            colorList.Add(247, (Color)ColorConverter.ConvertFromString("#68454E"));
            colorList.Add(248, (Color)ColorConverter.ConvertFromString("#4F0013"));
            colorList.Add(249, (Color)ColorConverter.ConvertFromString("#4F353B"));
            colorList.Add(250, (Color)ColorConverter.ConvertFromString("#333333"));
            colorList.Add(251, (Color)ColorConverter.ConvertFromString("#505050"));
            colorList.Add(252, (Color)ColorConverter.ConvertFromString("#696969"));
            colorList.Add(253, (Color)ColorConverter.ConvertFromString("#828282"));
            colorList.Add(254, (Color)ColorConverter.ConvertFromString("#BEBEBE"));
            colorList.Add(255, (Color)ColorConverter.ConvertFromString("#000000"));

            Color color = new Color();
            color = colorList.Values[lineColor];
            return color;
        }
    }
}
