using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using DXF.Extensions;
using DXF.Entities;
using DXF.GeneralInformation;

namespace DXF_Viewer
{
    class PolyLineEntity : Entity
    {

        public PolyLineEntity()
        { }

        public PolyLineEntity(Schematic drawing, Viewer topLevelViewer)
            : base(drawing, topLevelViewer)
        { }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();


            return this;
        }
        public override Path draw()
        {
            Path path = new Path();
            return path;
        }

        /// <summary>
        /// Gets the Path for the polyline Entity 
        /// </summary>
        /// <param name="listIn">The list of strings which includes all the information for the polyline entity</param>
        /// <param name="layerList">The list of layer information to include line color and line type</param>
        /// <param name="xOffsetIn">if part of a block this is the x coordinate offset</param>
        /// <param name="yOffsetIn">if part of a block this is the y coordinate offset</param>
        /// <returns>The path of the polyline to be drawn</returns>
        public Path getInfo(List<String> listIn, List<LayerInfo_dep> layerList, double height, double minX, double minY, double xOffsetIn, double yOffsetIn)
        {
            String lineType = "CONTINUOUS";
            double lineThickness = 0.0;
            int lineColor = 0;
            List<Point> polyLinePoints = new List<Point>();
            Point temp = new Point();
            Point polyPoint = new Point();
            Point startPoint = new Point();
            Point endPoint = new Point();
            String layerName = "0";
            double radius, bulge, chord, saggita, startAngle;
            temp = new Point();
            startPoint = new Point();
            endPoint = new Point();
            Boolean isClosed = false;
            Boolean xChecked = false;
            double rotation = 0;

            int j = 0;
            while (j <= listIn.Count - 1)
            {

                //The name of the layer
                if (listIn[j] == "  8")
                {
                    layerName = listIn[++j];
                }
                //Checks if it is closed or not and sets to the isClosed bool value
                if (listIn[j] == " 70")
                {
                    if (listIn[++j] == "     1")
                        isClosed = true;
                    else
                        isClosed = false;
                }
                if (listIn[j] == "ROTATION")
                {
                    rotation = listIn[++j].ConvertToDoubleWithCulture();
                }
                if (listIn[j] == "OFFSET")
                {
                    xOffsetIn = listIn[++j].ConvertToDoubleWithCulture();
                    yOffsetIn = listIn[++j].ConvertToDoubleWithCulture();
                }
                //Checks for the x coordinate of the point
                if (listIn[j] == " 10")
                {
                    //Checks to see if the x coordinate has already been passed once, if not passes by it
                    if (xChecked == true)
                    {
                        polyPoint.X = listIn[++j].ConvertToDoubleWithCulture();
                    }
                    else
                        xChecked = true; 
                    j++;
                }
                //Checks for the y coordinate of the point for the polyline
                if (listIn[j] == " 20")
                {
                    polyPoint.Y = listIn[++j].ConvertToDoubleWithCulture();
                    //If both the x and y coordinates have been set then they are added to the polyline points list
                    if (polyPoint.X != 0 && polyPoint.Y != 0)
                    {
                        polyLinePoints.Add(polyPoint);
                    }
                  
                }
                //gets the line thickness for the polyline
                if (listIn[j] == " 40")
                {
                    lineThickness = listIn[++j].ConvertToDoubleWithCulture();
                }
                //gets the line color for the polyline
                if (listIn[j] == " 62")
                {
                    lineColor = Convert.ToInt32(listIn[++j]);
                }
                //Checks to see if there is a bulge in the polyline and if so it will calculate that bulge
                if (listIn[j] == " 42")
                {
                    //Calculates the angle of the bulge and it is already in radians
                    //The bulge is 1/4 the Atan of the included angle
                    //Does the math for the startAngle
                    bulge = listIn[++j].ConvertToDoubleWithCulture();
                    startAngle = 4 * Math.Atan(bulge);
                    temp = polyPoint;
                    while (j <= listIn.Count - 1)
                    {

                        //takes the previous point and assigns it to the start point
                        startPoint = temp;

                        //find the end point and add it to the polylinepoints
                        //For the x point in the polyline
                        if (listIn[j] == " 10")
                        {
                            endPoint.X = listIn[++j].ConvertToDoubleWithCulture();
                            j++;
                        }
                        //For the y point in the polyline
                        if (listIn[j] == " 20")
                        {
                            endPoint.Y = listIn[++j].ConvertToDoubleWithCulture();
                            j++;
                        }
                        //gets the end angle for the bulge
                        if (listIn[j] == " 42")
                        {
                            //Does the math for the chord, saggita, and radius
                            bulge = listIn[++j].ConvertToDoubleWithCulture();
                            chord = Math.Pow((endPoint.X - startPoint.X), 2) + Math.Pow((endPoint.Y - startPoint.Y), 2);
                            saggita = (chord / 2) * bulge;
                            radius = (Math.Pow(chord / 2, 2) + Math.Pow(saggita, 2)) / (2 * saggita);

                            //If there is a bulge goes to the buildbulge method to draw the bulge
                            return buildBulge(startPoint, endPoint, radius, lineType, lineColor, layerName, layerList,lineThickness, height, minX, minY, rotation, xOffsetIn, yOffsetIn);
                        }
                        j++;

                    }

                }



                j++;
            }

            //If there is no bulge it goes just to the build polyline method to draw the polyline
            return buildPolyline(polyLinePoints, lineType, lineThickness, lineColor, layerName, layerList, isClosed, height, minX, minY, rotation, xOffsetIn, yOffsetIn);
        }

        /// <summary>
        /// Builds the path for the polyline without a bulge
        /// </summary>
        /// <param name="inPoints">The list of points for the polyline both x and y coordinates</param>
        /// <param name="lineType">The type of line to be drawn for the polyline</param>
        /// <param name="lineThickness">The thickness of the line to be drawn</param>
        /// <param name="lineColor">The color of the polyline</param>
        /// <param name="layerName">The name of the layer</param>
        /// <param name="layerList">List of layer information to include line type and line color</param>
        /// <param name="isClosed">Boolean value that says whether the polyline is closed or not</param>
        /// <param name="xOffset">if part of a block this is the x coordinate offset</param>
        /// <param name="yOffset">if part of a block this is the y coordinate offset</param>
        /// <returns>Build a path for the polyline to be drawn</returns>
        public Path buildPolyline(List<Point> inPoints, string lineType, double lineThickness, int lineColor, String layerName, List<LayerInfo_dep> layerList, Boolean isClosed, double height, double minX, double minY, double rotation, double xOffset = 0, double yOffset = 0)
        {
            //Create the polyline path
            Path polyPath = new Path();

            //Create the polyline
            LineSegment polyLine = new LineSegment();

            //Create the polyline path figure
            PathFigure polyFigure = new PathFigure();

            //Create the polyline path geometry
            PathGeometry polyGeometry = new PathGeometry();

            //Create teh polyline Geometry group
            GeometryGroup polyGroup = new GeometryGroup();

            //Create the collection of path figures for the polyline
            PathFigureCollection polyCollection = new PathFigureCollection();

            TransformPoint trans = new TransformPoint();
            Point temp = new Point();

            //Sets the start point of the figure to the first point in the list of points, then transforms it to proper coordinates
            polyFigure.StartPoint = trans.transformPoint(inPoints[0], height, minX, minY);

            //Sets each point after the first point and makes a line between that point and the previous one
            for (int k = 0; k <= inPoints.Count - 1; k++)
            {
                temp = trans.transformPoint(inPoints[k], height, minX, minY);    
                polyLine = new LineSegment(temp, true);
                polyFigure.Segments.Add(polyLine);
            }

            //Makes the polyline closed or not depending on the isClosed valuef
            polyFigure.IsClosed = isClosed;
            //Adds the polyfigure to the collection of polyline figures
            polyCollection.Add(polyFigure);
            //Sets the polyline geometry to the collection of polyline figures
            polyGeometry.Figures = polyCollection;

            //Sets the line thickness to default of 0.2 if it is 0
            if (lineThickness == 0.0)
                lineThickness = 0.2;

            //If there is no line color set it will check the layer for line color and line type
            if (lineColor == 0)
            {
                foreach (LayerInfo_dep tempLayer in layerList)
                {
                    if (tempLayer.layerName == layerName)
                    {
                        lineColor = tempLayer.lineColor;
                        lineType = tempLayer.lineType;
                    }
                }
            }   

            //Sets the color of the tempbrush dependent on what the line color has been set to
            ColorSorter cs = new ColorSorter();
            Color color = new Color();
            color = cs.getColor(lineColor);
            Brush tempBrush = new SolidColorBrush(color);
            polyPath.Stroke = tempBrush;

            //Sets the stroke thickness of the polyline
            polyPath.StrokeThickness = lineThickness;
            
            //sets the path data to the geometry of the polyline
            polyPath.Data = polyGeometry;

            //gets the rotation of the line if it is part of a rotated block
            if (rotation != 0)
            {
                Point centerPointForRotate = new Point();
                centerPointForRotate = trans.transformPoint(new Point(xOffset, yOffset), height, minX, minY);
                polyPath.RenderTransform = new RotateTransform(-rotation, centerPointForRotate.X, centerPointForRotate.Y);
            }

            return polyPath;
        }

        /// <summary>
        /// This will build the path of the polyline if there is a bulge
        /// </summary>
        /// <param name="startPoint">The start point of the bulge</param>
        /// <param name="endPoint">The end point of the bulge</param>
        /// <param name="radius">The radius of the bulge</param>
        /// <param name="lineType">The type of line that the bulge should be drawn as</param>
        /// <param name="lineColor">The color of the line to be drawn for the bulge</param>
        /// <param name="layerName">The name of the layer for the bulge</param>
        /// <param name="layerList">The list of layer information that includes the line color and line type</param>
        /// <param name="lineThickness">The line thickness of the polyline bulge</param>
        /// <returns>The path of the polyline bulge</returns>
        public Path buildBulge(Point startPoint, Point endPoint, double radius, string lineType, int lineColor, String layerName, List<LayerInfo_dep> layerList, double lineThickness, double height, double minX, double minY, double rotation, double xOffset, double yOffset)
        {
            //Transforms the start and end point to the proper coordinates, different method than normal polyline
            TransformPoint trans = new TransformPoint();
            startPoint = trans.transformPoint2(startPoint, height, minX, minY);
            endPoint = trans.transformPoint2(endPoint, height, minX, minY);

            //Creates a new path
            Path path = new Path();

            //Creates a new path Geometry
            PathGeometry arcGeometry = new PathGeometry();

            //Creates a new path figure
            PathFigure pathFigure = new PathFigure();

            //Creates a new arc segment
            ArcSegment arc = new ArcSegment();

            //Set the radius of the arc (circular arc only for now)
            Size size = new Size();
            size.Height = Math.Abs(radius);
            size.Width = Math.Abs(radius);

            //If the line thickness is not set it gives it a default value of 0.35
            if (lineThickness == 0.0)
                lineThickness = 0.35;

            //set the line thickness
            path.StrokeThickness = lineThickness;

            //If there is no line color set it checks the layerlist and determines line type and line color
            if (lineColor == 0)
            {
                foreach (LayerInfo_dep tempLayer in layerList)
                {
                    if (tempLayer.layerName == layerName)
                    {
                        lineColor = tempLayer.lineColor;
                        lineType = tempLayer.lineType;
                    }
                }
            }

            //This will get the color from the list of colors dependent on the line color it is given and set the stroke color to that 
            ColorSorter cs = new ColorSorter();
            Color color = new Color();
            color = cs.getColor(lineColor);
            Brush tempBrush = new SolidColorBrush(color);
            path.Stroke = tempBrush;

            //Set the line Type
            SetLineType setLine = new SetLineType();
            path.StrokeDashArray = setLine.setLineType(lineType);


            //Sets the arc end point
            arc.Point = endPoint;

            //Sets the arc start point
            pathFigure.StartPoint = startPoint;

            //Sets the arc radius
            arc.Size = size;

            //Sets the path figure to be closed
            pathFigure.IsClosed = true;

            //Adds the arc to the path figure
            pathFigure.Segments.Add(arc);

            //Adds the path figure to the path geometry
            arcGeometry.Figures.Add(pathFigure);

            //Sets the path equal to the path Geometry
            path.Data = arcGeometry;

            //gets the rotation of the line if it is part of a rotated block
            if (rotation != 0)
            {
                Point centerPointForRotate = new Point();
                centerPointForRotate = trans.transformPoint(new Point(xOffset, yOffset), height, minX, minY);
                path.RenderTransform = new RotateTransform(-rotation, centerPointForRotate.X, centerPointForRotate.Y);
            }

            return path;
        }

        public override Path draw(InsertEntity insertion)
        {
            throw new NotImplementedException();
        }
    }
}