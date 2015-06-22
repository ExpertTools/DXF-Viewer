using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using DXF.Extensions;


namespace DXF_Viewer
{
   
    class HatchEntity : Entity_Old
    {
        /// <summary>
        /// This will get the hatch path as a collection of path figures
        /// </summary>
        /// <param name="listIn">The entire list of strings that contains the hatch entity</param>
        /// <param name="layerList">The list of layer information including the color for the hatch</param>
        /// <param name="xOffsetIn">The x coordinate offset for the hatch if in a block</param>
        /// <param name="yOffsetIn">The y coordinate offset for the hatch if in a block</param>
        /// <returns>The path for the hatch</returns>
        public Path getInfo(List<String> listIn, List<LayerInfo_dep> layerList, double height, double minX, double minY, double xOffsetIn, double yOffsetIn)
        {

            List<double> bulge = new List<double>();
            int lineColor = 0;
            string lineType = "CONTINUOUS";
            String layerName = "0";
            Point hatchPoint = new Point();
            List<Point> hatchPoints = new List<Point>();
            List<Point> lineHatchPoints = new List<Point>();
            int numHatchPolylines = 0;
            ArcSegment bulgeSegment = new ArcSegment();
            Path hatch = new Path();
            PathFigureCollection hatchCollection = new PathFigureCollection();
            PathFigure tempFigure = new PathFigure();
            PathGeometry hatchGeometry = new PathGeometry();
            double pathEdges = 0;
            Point lineStart = new Point();
            Point lineEnd = new Point();
            double rotation = 0;
            int isClosed = 1;
            hatchGeometry.FillRule = FillRule.EvenOdd;
            int j = 0;
            while (j < listIn.Count)
            {
                //The name of the layer to be used for the hatch
                if (listIn[j] == "  8")
                {
                    layerName = listIn[++j];
                }
                //THe color of the hatch if it is set
                if (listIn[j] == " 62")
                {
                    lineColor = Convert.ToInt32(listIn[++j]);
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
                //The boundary path flag(bit coded) 
                //0 = default; 1 = external; 2 = polyline;
                // 4 = derived; 8 = textbox; 16 = outermost;
                if (listIn[j] == " 92")
                {
                    j++;
                    //If this is a polyline hatch and it is an "external" polyline 
                    
                    if (listIn[j] == "        3" || listIn[j] == "        7" || listIn[j] == "       11" || listIn[j] == "       15" || listIn[j] == "       19"  ||listIn[j] == "       23" || listIn[j] == "       27" || listIn[j] == "       31")
                    {
                        j++;
                        //The has bulge flag, boolean
                        if (listIn[j] == " 72")
                        {
                            while (listIn[j] != " 97")
                            {
                                j++;
                                //polyline with bulge
                                if (listIn[j] == "     1")
                                {
                                    while (listIn[j] != " 97")
                                    {
                                        //Boolean for if the polyline is closed or not
                                        if (listIn[j] == " 73")
                                            isClosed = Convert.ToInt32(listIn[++j]);
                                        //number of polyline vertices
                                        if (listIn[j] == " 93")
                                            numHatchPolylines = Convert.ToInt32(listIn[++j]);
                                        //this is the x coordinate for the start and end points
                                        if (listIn[j] == " 10" || listIn[j] == " 11")
                                           hatchPoint.X = listIn[++j].ConvertToDoubleWithCulture();
                                        //the y coordinate for the start and end points
                                        if (listIn[j] == " 20" || listIn[j] == " 21")
                                        {
                                           hatchPoint.Y = listIn[++j].ConvertToDoubleWithCulture();
                                            //adds the point to the list of hatch points
                                            hatchPoints.Add(hatchPoint);
                                        }
                                        //The bulge(1/4 the Atan of the included angle)
                                        if (listIn[j] == " 42")
                                           bulge.Add(listIn[++j].ConvertToDoubleWithCulture());
                                        //The end of the polyline chunk of the hatch
                                        if (listIn[j] == " 97")
                                            break;
                                        j++;
                                    }
                                    tempFigure = (buildHatchPoly(hatchPoints, lineColor, layerList, layerName, bulge, lineType, isClosed, height, minX, minY));
                                    hatchPoints = new List<Point>();
                                    //adds the temporary path figure to the collection of path figures
                                    hatchCollection.Add(tempFigure);
                                    bulge = new List<double>();

                                }

                                //if there is no bulge in the polyline
                                else if (listIn[j] == "     0")
                                {
                                    while (listIn[j] != " 97")
                                    {
                                        //number of vertices in the polyline
                                        if (listIn[j] == " 93")
                                            numHatchPolylines = Convert.ToInt32(listIn[++j]);
                                        //x coordinate of the start and end point of this line
                                        if (listIn[j] == " 10" || listIn[j] == " 11")
                                           hatchPoint.X = listIn[++j].ConvertToDoubleWithCulture();
                                        //y coordinate of the start and end point of this line
                                        if (listIn[j] == " 20" || listIn[j] == " 21")
                                        {
                                           hatchPoint.Y = listIn[++j].ConvertToDoubleWithCulture();
                                                hatchPoints.Add(hatchPoint);
                                        }
                                        if (listIn[j] == " 97")
                                            break;
                                        j++;
                                    }
                                    tempFigure = buildHatch(hatchPoints, lineColor, layerList, layerName, isClosed, height, minX, minY);
                                    hatchPoints = new List<Point>();
                                    //adds the path figure to the collection of path figures
                                    hatchCollection.Add(tempFigure);
                                }
                            }
                        }
                    }
                    //This is for some sort of line hatch but for a textbox?
                    else if (listIn[j] == "        8")
                    {
                 
                        j++;
                        //Number of edgees in the boundary path
                        if (listIn[j] == " 93")
                        {
                           pathEdges = listIn[++j].ConvertToDoubleWithCulture();
                            j++;
                        }
                        //Edge type (1=line; 2=circular arc; 3=elliptic arc; 4=spline;)
                        if (listIn[j] == " 72")
                        {
                            while (listIn[j] != " 97")
                            {
                                j++;
                                //a line edge type
                                if (listIn[j] == "     1")
                                {
                                    while (listIn[j] != " 97")
                                    {
                                        //the x coordinate of the start point
                                        if (listIn[j] == " 10")
                                        {
                                           lineStart.X = listIn[++j].ConvertToDoubleWithCulture();
                                        }
                                        //the y coordinate of the start point
                                        if (listIn[j] == " 20")
                                        {
                                           lineStart.Y = listIn[++j].ConvertToDoubleWithCulture();
                                            lineHatchPoints.Add(lineStart);
                                        }
                                        //the x coordinate of the end point
                                        if (listIn[j] == " 11")
                                        {
                                            lineEnd.X = listIn[++j].ConvertToDoubleWithCulture();
                                        }
                                        //the y coordinate of the end point
                                        if (listIn[j] == " 21")
                                        {
                                            lineEnd.Y = listIn[++j].ConvertToDoubleWithCulture();
                                            lineHatchPoints.Add(lineEnd);
                                        }
                                        if (listIn[j] == " 72")
                                        {
                                            break;
                                        }
                                        j++;
                                    }
                                }
                            }
                            tempFigure = buildLineHatch(lineHatchPoints, height, minX, minY);
                            //adds the path figure to the collection of path figures
                            hatchCollection.Add(tempFigure);
                        }
                    }
                    
                }
               
                if (listIn[j] == "ENDSEC")
                    break;
                j++;
            }
            //sets the hatchgeometry to the collection of path figures
            hatchGeometry.Figures = hatchCollection;

            //Sets the color of the hatch by layer if it has not be set already
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
            //Takes the color it was given as an int and finds the proper color from color sorter
            ColorSorter cs = new ColorSorter();
            Color color = new Color();
            color = cs.getColor(lineColor);
            Brush tempBrush = new SolidColorBrush(color);
            
            //fills everywhere the hatch is supposed to be filled, not just the inside or outside
            hatchGeometry.FillRule = FillRule.Nonzero;
            //fills the hatch with the color from above
            hatch.Fill = tempBrush;
            //sets the path data to the geometry from all of the path figures
            hatch.Data = hatchGeometry;

            TransformPoint transPoint = new TransformPoint();
            //gets the rotation of the line if it is part of a rotated block
            if (rotation != 0)
            {
                Point centerPointForRotate = new Point();
                centerPointForRotate = transPoint.transformPoint(new Point(xOffsetIn, yOffsetIn), height, minX, minY);
                hatch.RenderTransform = new RotateTransform(-rotation, centerPointForRotate.X, centerPointForRotate.Y);
            }
            return hatch;

        }

        /// <summary>
        /// Builds the path figure for a line edge type of a hatch
        /// </summary>
        /// <param name="hatchLinePoints">A list of points for the hatch in both x and y coordinates</param>
        /// <returns>Path figure to be added to the collection</returns>
        public PathFigure buildLineHatch(List<Point> hatchLinePoints, double height, double minX, double minY)
        {
            Path lineHatchPath = new Path();
            LineSegment hatchLine = new LineSegment();
            PathFigure hatchFigure = new PathFigure();
            PointCollection pointCollection = new PointCollection();
            Point temp = new Point();
            TransformPoint trans = new TransformPoint();

            //Sets the start point of the path figure to the first point in the collection of points
            hatchFigure.StartPoint = trans.transformPoint(hatchLinePoints[0], height, minX, minY);
            //Go through each point and transform them to proper canvas coordinates
            //then make line segments out of each set of points to be added to the path figure
            for (int i = 0; i <= hatchLinePoints.Count - 1; i++)
            {
              
                temp = trans.transformPoint(hatchLinePoints[i], height, minX, minY);

                hatchLine = new LineSegment(temp, true);
                hatchFigure.Segments.Add(hatchLine);

                temp = new Point();
            }

            //set the hatch figure to be closed
            hatchFigure.IsClosed = true;
            
            //return the path figure to be added to the collection
            return hatchFigure;
        }

        /// <summary>
        /// This will get the path figure for a polyline part of a hatch with no bulge
        /// </summary>
        /// <param name="hatchPoints">The list of points to be used to make the polyline figure</param>
        /// <param name="lineColor">The color of the hatch</param>
        /// <param name="layerList">THe list of layer information to include hatch color</param>
        /// <param name="layerName">The name of the layer to be used for this hatch</param>
        /// <param name="isClosed">a boolean isclosed flag to tell the polyline it is or is not closed</param>
        /// <returns>A path figure to be added to the collection of figures</returns>
        public PathFigure buildHatch(List<Point> hatchPoints, int lineColor, List<LayerInfo_dep> layerList, string layerName, int isClosed, double height, double minX, double minY)
        {
            Path hatchPath = new Path();
            LineSegment hatchLine = new LineSegment();
            PathFigure hatchFigure = new PathFigure();
            PointCollection pointCollection = new PointCollection();
            Point temp = new Point();
            TransformPoint trans = new TransformPoint();

            //Sets the start point of this path figure to the first point in the list of points
            hatchFigure.StartPoint = trans.transformPoint(hatchPoints[0], height, minX, minY);
            
            //Transforms each point then makes a line out of the set of points 
            //then adds the line segments to the path figure 
            for (int i = 0; i <= hatchPoints.Count - 1; i++)
            {
                temp = trans.transformPoint(hatchPoints[i], height, minX, minY);

                hatchLine = new LineSegment(temp, true);
                hatchFigure.Segments.Add(hatchLine);

                temp = new Point();
            }

            //sets the isclosed flag to true
            hatchFigure.IsClosed = true;

            return hatchFigure;
        }

        /// <summary>
        /// This is to get the path figure for a polyline which contains a bulge
        /// </summary>
        /// <param name="hatchPoints">A list of points in x and y coordinates</param>
        /// <param name="lineColor">the color of the hatch</param>
        /// <param name="layerList">List of layer information which includes hatch color</param>
        /// <param name="layerName">The name of the layer that the hatch is assigned</param>
        /// <param name="bulge">A list of doubles which are all the bulge points</param>
        /// <param name="lineType">The type of line that the hatch is</param>
        /// <param name="isClosed">a boolean flag stating whether or not the polyline is closed</param>
        /// <returns>a path figure to be added to the collection of path figures</returns>
        public PathFigure buildHatchPoly(List<Point> hatchPoints, int lineColor, List<LayerInfo_dep> layerList, string layerName, List<double> bulge, string lineType, int isClosed, double height, double minX, double minY)
        {
            Path HatchPolyPath = new Path();
            LineSegment hatchLine = new LineSegment();
            ArcSegment hatchArc = new ArcSegment();
            PathFigure hatchFigure = new PathFigure();
            PathGeometry hatchGeometry = new PathGeometry();
            PointCollection pointCollection = new PointCollection();
            Point temp = new Point();
            TransformPoint trans = new TransformPoint();
            double chord, saggita, radius;

            //Goes through each point in the hatchpoints list
            for (int i = 0; i <= hatchPoints.Count - 1; i++)
            {
                SweepDirection sweepDirection = new SweepDirection();

                //sets the current point to the temporary point
                temp = hatchPoints[i];

                //transforms the temporary point to the proper canvas coordinates
                temp = trans.transformPoint(temp, height, minX, minY);

                //if the bulge is positive it will set the sweep direction to counterclockwis
                if (bulge[i] > 0)
                    sweepDirection = SweepDirection.Counterclockwise;
                //if the bulge is less than 0 it will set the sweep direction to clockwise
                else
                    sweepDirection = SweepDirection.Clockwise;
                
                //If this is the first point in the hatchpoints collection it
                //will set that point as the start point
                if (i == 0)
                {
                    hatchFigure.StartPoint = temp;
                }
                //if the bulge is 0 then it will just add the line segment since there
                //is no bulge in this piece of the path figure
                if (bulge[i] == 0)
                {
                    hatchLine = new LineSegment(temp, true);
                    hatchFigure.Segments.Add(hatchLine);
                }
                //if the bulge is anything but 0 at this point
                if (bulge[i] != 0)
                {

                    Size size = new Size();
                    //If this is the first part of the hatch it has to do things differently
                    if (i == 0)
                    {
                        //Sets the end point to the next point in the list. Also transforms
                        //that point to proper canvas coordinates
                        hatchArc.Point = trans.transformPoint(hatchPoints[i+1], height, minX, minY);
                        //sets the end point to a temporary point
                        temp = hatchArc.Point;

                        //The math for finding the chord, saggita, and radius
                        //so the height and width can be obtained
                        chord = Math.Pow((hatchPoints[i + 1].X - hatchPoints[i].X), 2);
                        chord = chord + Math.Pow((hatchPoints[i + 1].Y - hatchPoints[i].Y), 2);
                        chord = Math.Sqrt(chord);
                        saggita = ((chord) / 2) * bulge[i];
                        radius = (Math.Pow(chord / 2, 2) + Math.Pow(saggita, 2)) / (2 * saggita);

                        //sets the height and width to the radius
                        size.Height = Math.Abs(radius);
                        size.Width = Math.Abs(radius);

                    }
                    //If the hatch doesn't come on the first point of the hatch
                    else
                    { 
                        hatchLine = new LineSegment(temp, true);
                        //add the line segment of the piece before 
                        hatchFigure.Segments.Add(hatchLine);
                        //If there is still more bulge points after this one
                        if (i < hatchPoints.Count - 1)
                        {
                            //sets the temporary point to the next point in the list
                            //and transforms that point to proper canvas coordinates
                            temp = trans.transformPoint(hatchPoints[i + 1], height, minX, minY);
                            chord = Math.Pow((hatchPoints[i + 1].X - hatchPoints[i].X), 2);
                            chord = chord + Math.Pow((hatchPoints[i + 1].Y - hatchPoints[i].Y), 2);
                            chord = Math.Sqrt(chord);  
                        }
                        //If this is the last bulge point in the list
                        else
                        {
                            //sets temporary point as the first point of the list
                            temp = trans.transformPoint(hatchPoints[0], height, minX, minY);
                            //The first point in the list is now being used as the next point since
                            //it will be the last also since this is a closed polyline
                            chord = Math.Pow((hatchPoints[0].X - hatchPoints[i].X), 2);
                            chord = chord + Math.Pow((hatchPoints[0].Y - hatchPoints[i].Y), 2);
                            chord = Math.Sqrt(chord);                             
                        }
                        //do the math for the saggita and radius
                        saggita = ((chord) / 2) * bulge[i];
                        radius = (Math.Pow(chord / 2, 2) + Math.Pow(saggita, 2)) / (2 * saggita);
                        //set the height and width to the radius
                        size.Height = Math.Abs(radius);
                        size.Width = Math.Abs(radius);
                    }
                    //builds a new arc segment 
                    hatchArc = new ArcSegment(temp, size, 0, hatchArc.IsLargeArc, sweepDirection, true);
                    //adds the arc segment to the path figure 
                    hatchFigure.Segments.Add(hatchArc);
                                        
                }

                temp = new Point();
            }
            //sets the path figure to closed
            hatchFigure.IsClosed = true;

            return hatchFigure;
        }
    }
}