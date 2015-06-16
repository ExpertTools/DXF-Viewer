using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;
using DXF.Extensions;
using DXF.Entities;
using DXF.GeneralInformation;

namespace DXF_Viewer
{
    class LwPolylineEntity : Entity
    {

        public LwPolylineEntity ()
        { }

        public LwPolylineEntity (Schematic drawing, Viewer viewer)
            : base(drawing, viewer)
        { }

        public override Entity parse(List<string> section)
        {
            gatherCodesAllowMultiples(section);

            return this;
        }

        public override Path draw()
        {
            Path path = new Path();
            return path;
        }

        public struct WindowInfo
        {
            public double minX;
            public double minY;
            public double height;
            public double offsetX;
            public double offsetY;
        }
        /// <summary>
        /// Gets and builds the path of the lwPolyline so that it can be drawn 
        /// </summary>
        /// <param name="listIn">The list of strings which contain the entire piece of lwPolyline data for this particular entity</param>
        /// <param name="layerList"> the list of layer information which includes line type and line color</param>
        /// <param name="xOffsetIn">The x coordinate offset if this entity is part of a block</param>
        /// <param name="yOffsetIn">The y coordinate offset if this entity is part of a block</param>
        /// <returns></returns>
        public Path getInfo(List<String> listIn, List<LayerInfo_dep> layerList, double height, double minX, double minY, double xOffsetIn, double yOffsetIn)
        {
            //geometry set up
            Path path = new Path();
            PathGeometry pathGeo = new PathGeometry();
            PathFigure figure = new PathFigure();
            WindowInfo windowInfo;
            ColorSorter cs = new ColorSorter();
            Color color = new Color();
            Brush brush =  new SolidColorBrush(Color.FromRgb(255, 100, 100));
            TransformPoint trans = new TransformPoint();

            bool colorSpecified = false;
            bool isClosed = false;
            int i = 0;
            int numberOfVertices;
            int vertexCount = 0;
            int lineColor = 0;
            double rotation = 0;
            double lineThickness = 0;
            string layerName;
            string lineType = "CONTINUOUS";

            //used passed values to give values to WindowInfo Struct
            //holds standard info about the window being drawn on
            windowInfo.height = height;
            windowInfo.minX = minX;
            windowInfo.minY = minY;
            windowInfo.offsetX = xOffsetIn;
            windowInfo.offsetY = yOffsetIn;
            //add an indentifier to the end of the list
            listIn.Add("  0");
            //iterates until the start of the polyline information
            int j = 0;
            while (listIn[i] != " 90")
            {
                if (listIn[i] == "ROTATION")
                {
                    rotation = listIn[++i].ConvertToDoubleWithCulture();
                }
                if (listIn[i] == "OFFSET")
                {
                    windowInfo.offsetX = listIn[++j].ConvertToDoubleWithCulture();
                    windowInfo.offsetY = listIn[++j].ConvertToDoubleWithCulture();
                }
                if (listIn[i] == "  8")
                {
                    i++;
                    layerName = listIn[i];
                    if (!colorSpecified)
                    {
                        foreach (LayerInfo_dep tempLayer in layerList)
                        {
                            if (tempLayer.layerName == layerName)
                            {
                                color = cs.getColor(tempLayer.lineColor);
                                lineType = tempLayer.lineType;
                                brush = new SolidColorBrush(color);
                            }
                        }
                    }
                }

                i++;
            }
            //The line following the tag ' 90' contains the number of vertices in this polyline.
            numberOfVertices = Convert.ToInt32(listIn[++i]);
            //holds the values for each of the ' 10' ' 20' pairs.
            List<Point> vertices = new List<Point>(numberOfVertices);
            List<double> bulgeInfo = new List<double>(numberOfVertices);

            while (vertexCount < numberOfVertices)
            {
                //in two places because of uncertainty of where these tags will be in the file.
                if (listIn[i] == "ROTATION")
                {
                    rotation = listIn[++i].ConvertToDoubleWithCulture();
                }
                if (listIn[i] == "OFFSET")
                {
                    windowInfo.offsetX = listIn[++j].ConvertToDoubleWithCulture();
                    windowInfo.offsetY = listIn[++j].ConvertToDoubleWithCulture();
                }
                //check if polyine is closed;
                if(listIn[i] == " 70")
                {
                    i++;
                    if (listIn[i] == "     1")
                    {
                        isClosed = true;
                    }
                    else
                    {
                        isClosed = false;
                    }
                }
                //check for new layer name
                
                
                //check for a vertex description
                if (listIn[i] == " 10")
                {
                    //redo this so that when 10 is hit it will loop until the end of the file
                    //may need to be compeletly deleted and rewritten
                    //the file is laid out by:
                    // 10
                    // 20 - end of first vertex
                    // from there it can either be a straight line or a curve
                    // if it is a curve then it will be 42
                    // if it is a line then it will be 10
                    // it is important to differentiate between the loop reading the
                    // list of strings and the pseudo loop of creating segments.
                    // the sequence of  
                    Point p1 = new Point();
                    Point p2 = new Point();
                    //read until the end of the file
                    while (listIn[i] != "  0" && vertexCount < numberOfVertices)
                    {
                        //if this is the first vertex handle it differently

                        if (vertexCount == 0)
                        {
                            i++;
                            p1.X = listIn[i].ConvertToDoubleWithCulture();
                            i += 2;
                            p1.Y = listIn[i].ConvertToDoubleWithCulture();
                            figure.StartPoint = trans.transformPoint(p1, windowInfo.height, windowInfo.minX, windowInfo.minY);
                            //first point read in
                            vertexCount++;
                        }
                        else
                        {
                            //next segment is an arc
                            if (listIn[i] == " 42")
                            {
                                i++;
                                double angle = listIn[i].ConvertToDoubleWithCulture();
                                i += 2;
                                p2.X = listIn[i].ConvertToDoubleWithCulture();
                                i += 2;
                                p2.Y = listIn[i].ConvertToDoubleWithCulture();
                                vertexCount++;
                                //build the arc
                                figure.Segments.Add(makeBulge(p1, p2, angle, windowInfo));
                                //put the values of p2 into p1 so that it can be
                                //used to detemine the starting point for the
                                //next segment.

                                if (vertexCount == numberOfVertices && isClosed == true && listIn[i+1] == " 42")
                                {
                                    figure.Segments.Add(makeBulge(p2, p1, angle, windowInfo));
                                }
                                p1.X = p2.X;
                                p1.Y = p2.Y;

                            }
                            else if (listIn[i] == " 10")
                            {
                                //next segment is a line
                                i++;
                                p2.X = listIn[i].ConvertToDoubleWithCulture();
                                i += 2;
                                p2.Y = listIn[i].ConvertToDoubleWithCulture();
                                vertexCount++;
                                //build the line
                                figure.Segments.Add(makeSegment(p1, p2, windowInfo));
                                //p2->p1
                                p1.X = p2.X;
                                p1.Y = p2.Y;
                            }

                        }


                        i++;
                    }
                }

                if (listIn[i] == " 43")
                {
                    i++;
                    lineThickness = listIn[i].ConvertToDoubleWithCulture();
                }
                //read in color and set it for future drawing
                if (listIn[i] == " 62")
                {
                    i++;
                    lineColor = Convert.ToInt32(listIn[i]);
                    color = cs.getColor(lineColor);
                    brush = new SolidColorBrush(color);
                }
                

                
                i++;
            }

            SetLineType setLine = new SetLineType();
            figure.IsClosed = isClosed;
            pathGeo.Figures.Add(figure);
            
            path.Data = pathGeo;
            if (rotation != 0)
            {
                Point centerPointForRotate = new Point();
                centerPointForRotate = trans.transformPoint(new Point(windowInfo.offsetX, windowInfo.offsetY), windowInfo.height, windowInfo.minX, windowInfo.minY);
                path.RenderTransform = new RotateTransform(-rotation, centerPointForRotate.X, centerPointForRotate.Y);
            }
            if (lineThickness == 0.0)
            {
                lineThickness = .2;
            }
            path.StrokeThickness = lineThickness;
            path.Stroke = brush;
            
            path.StrokeDashArray = setLine.setLineType(lineType);

            return path;
        }


        public ArcSegment makeBulge(Point start, Point end, double angle, WindowInfo windowInfo)
        {
            ArcSegment arc = null;
            TransformPoint trans = new TransformPoint();
            SweepDirection sweep;
            bool largeArc;
            //if the bulge is positive sweep = CCW
            if (angle > 0)
            {
                sweep = SweepDirection.Counterclockwise;
            }
            else
            {
                sweep = SweepDirection.Clockwise;
            }
            if (Math.Abs(angle) >= 1)
            {
                largeArc = true;
            }
            else
            {
                largeArc = false;
            }
            double radius = calculateRadius(angle, start, end);
            Size size = new Size(radius, radius);
            start = trans.transformPoint(start, windowInfo.height, windowInfo.minX, windowInfo.minY);
            end = trans.transformPoint(end, windowInfo.height, windowInfo.minX, windowInfo.minY);
            arc = new ArcSegment(end, size, angle, largeArc, sweep, true);
            
            return arc;
        }

        /// <summary>
        /// Helper method to calculate radius of bulges using the distance between points.
        /// </summary>
        /// <param name="angle">The included angle read in from the DXF file</param>
        /// <param name="start">The starting point of an arc.</param>
        /// <param name="end">The ending point of an arc.</param>
        /// <returns></returns>
        public double calculateRadius(double angle, Point start, Point end)
        {
            double chord = Math.Pow(Math.Pow((end.X - start.X), 2) + Math.Pow((end.Y - start.Y), 2), 0.5);
            double sag = Math.Abs((chord / 2) * angle);
            return (Math.Pow(chord / 2, 2) + Math.Pow(sag, 2)) / (2 * sag);
        }
        public LineSegment makeSegment(Point start, Point end, WindowInfo windowInfo)
        {
            TransformPoint trans = new TransformPoint();
            end = trans.transformPoint(end, windowInfo.height, windowInfo.minX, windowInfo.minY);

            LineSegment line = new LineSegment(end, true);

            return line;
        }

        /// <summary>
        /// If there is a bulge it will take the information from above and use it to calculate the path of the bulge here
        /// </summary>
        /// <param name="startPoint">the first point of the bulge x and y coordinate</param>
        /// <param name="endPoint">the final point of the bulge x and y coordinate</param>
        /// <param name="radius">the radius of the bulge</param>
        /// <param name="lineType">the type of line that the polyline is during the bulge</param>
        /// <param name="lineColor">the color of the bulge</param>
        /// <param name="layerName">the name of the layer that is to be used to find color and line type later</param>
        /// <param name="lineThickness">the thickness of the polyline</param>
        /// <param name="layerList">The list of layer information such as the line color and line type</param>
        /// <param name="isClosed">Boolean value stating whether or not the polyline is closed</param>
        /// <returns>The path of the bulge to be drawn later</returns>
        public Path buildBulge(Point startPoint, Point endPoint, double radius, string lineType, int lineColor, String layerName, SweepDirection sweepDirectionBool, double lineThickness, List<LayerInfo_dep> layerList, Boolean isClosed, double height, double minX, double minY, double rotation, double xOffset, double yOffset)
        {

            //transform the points to the proper canvas coordinates, using a slightly different method than all the other points
            TransformPoint trans = new TransformPoint();

            startPoint = trans.transformPoint(startPoint, height, minX, minY);
            endPoint = trans.transformPoint(endPoint, height, minX, minY);
                   
            //}
            //else if(startPoint.Y <= endPoint.Y)
            //{
            //    startPoint = trans.transformPoint3(startPoint, height, minX, minY);
            //    endPoint = trans.transformPoint3(endPoint, height, minX, minY);
            //}

            //Creates a new path
            Path path = new Path();

            //Creates a new path Geometry
            PathGeometry arcGeometry = new PathGeometry();

            //Creates a new path figure
            PathFigure pathFigure = new PathFigure();

            //Creates a new arc segment
            ArcSegment arc;// = new ArcSegment();
            
            //Set the radius of the arc (circular arc only for now)
            Size size = new Size();
            size.Height = radius;
            size.Width = radius;

            //if line thickness is 0 sets it to the default size of 0.35
            if (lineThickness == 0.0)
                lineThickness = .2;

            
            //if the bulge is positive it will set the sweep direction to counterclockwis
            //if (endPoint.Y < startPoint.Y)
            //    sweepDirection = SweepDirection.Counterclockwise;
            //if the bulge is less than 0 it will set the sweep direction to clockwise
            //else
            //    sweepDirection = SweepDirection.Clockwise;

            //set the line thickness
            path.StrokeThickness = lineThickness;

            //if the line color has not been set checks the layer information to find line color and line t ype
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
            //Sets the color of the polyline dependent on the color it was given either by layer or from the line color above
            ColorSorter cs = new ColorSorter();
            Color color = new Color();
            color = cs.getColor(lineColor);
            Brush tempBrush = new SolidColorBrush(color);
            path.Stroke = tempBrush;



            //Set the line Type
            SetLineType setLine = new SetLineType();
            path.StrokeDashArray = setLine.setLineType(lineType);
          

            //Sets the arc end point
            //arc.Point = endPoint;

            //Sets the arc start point
            pathFigure.StartPoint = startPoint;
            
            //Makes the path closed if it should be and not if it shouldn't
            pathFigure.IsClosed = isClosed;

            //Sets the arc radius
            //arc.Size = size;
            //arc.SweepDirection = sweepDirectionBool;
            arc = new ArcSegment(endPoint, size, 0, false, sweepDirectionBool, true);
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

        /// <summary>
        /// Builds the Path of the lwPolyline if there are no bulges
        /// </summary>
        /// <param name="lwPolyPoints">A list of points in x and y coordinates that the polyline will use</param>
        /// <param name="lineColor">The color of the lwPolyline</param>
        /// <param name="layerName">The name of the layer that the lwPolyline is using</param>
        /// <param name="lineType">The type of line that the polyline is drawn as</param>
        /// <param name="numVertices">The number of vertices in this particular lwpolyline</param>
        /// <param name="lineThickness">The thickness of the lwpolyline to be drawn</param>
        /// <param name="layerList">The list of layer information such as line color and line type</param>
        /// <param name="isClosed">Boolean value stating whether the lwpolyline is closed or not</param>
        /// <returns>The path of the lwPolyline if there is no bulge</returns>
        public Path buildLWpolyline(List<Point>lwPolyPoints, int lineColor, string layerName, string lineType, int numVertices, double lineThickness, List<LayerInfo_dep> layerList, Boolean isClosed, double height, double minX, double minY, double rotation, double xOffset, double yOffset)
        {
            Path LwPolyPath = new Path();
            LineSegment line = new LineSegment();
            PathFigure polyFigure = new PathFigure();
            PathGeometry polyPathGeometry = new PathGeometry();
            GeometryGroup lwGroup = new GeometryGroup();
            TransformPoint trans = new TransformPoint();
            Point temp = new Point();
            PathFigureCollection polyCollection = new PathFigureCollection();

            //Determines the start point of the polyline by the first point in the list of points
            polyFigure.StartPoint = trans.transformPoint(lwPolyPoints[0], height, minX, minY);
            
            //Transforms each Point in the lwpolypoints list, and uses those points to add a line segment to the polyline figure
            for (int k= 0; k <= lwPolyPoints.Count - 1; k++)
            {
                temp = trans.transformPoint(lwPolyPoints[k], height, minX, minY);
                line = new LineSegment(temp, true);
                polyFigure.Segments.Add(line);
                
            }

            //Determines if the polyline is closed from the isClosed bool
            polyFigure.IsClosed = isClosed;

            //adds the figure to the collection of polyline figures to be drawn later
            polyCollection.Add(polyFigure);

            //Sets the geometry as the polylineCollection
            polyPathGeometry.Figures = polyCollection;
            
            //if the line thickness is 0 sets it to the default 0.2
            if (lineThickness == 0.0)
                lineThickness = 0.2;

            //If no line color is set it checks the layer information and sets the color and line type dependent on that
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

            //Takes the linecolor and sets the paths color to that value
            ColorSorter cs = new ColorSorter();
            Color color = new Color();
            color = cs.getColor(lineColor);
            Brush tempBrush = new SolidColorBrush(color);
            LwPolyPath.Stroke = tempBrush;

            //Sets the stroke thickness
            LwPolyPath.StrokeThickness = lineThickness;
            
            //Sets the data of the path to the geometry
            LwPolyPath.Data = polyPathGeometry;

            //gets the rotation of the line if it is part of a rotated block
            if (rotation != 0)
            {
                Point centerPointForRotate = new Point();
                centerPointForRotate = trans.transformPoint(new Point(xOffset, yOffset), height, minX, minY);
                LwPolyPath.RenderTransform = new RotateTransform(-rotation, centerPointForRotate.X, centerPointForRotate.Y);
            }

            return LwPolyPath;
        }
    }
}
