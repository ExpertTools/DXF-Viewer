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
using DXF.Util;

namespace DXF_Viewer
{
    class LwPolylineEntity : Entity
    {
        bool closed  = true;
        PolylineVertex[] vertices;
        double thickness = .2;
        double widthConstant = 0;

        struct PolylineVertex
        {
            public Point lastVertex;
            public Point vertex;
            public double bWidth;
            public double eWidth;
            public double bulge;
        }

        enum readStatus { x, y, bulge, beginWidth, endWidth, seek };

        public LwPolylineEntity ()
        { }

        public LwPolylineEntity (Schematic drawing, Viewer viewer)
            : base(drawing, viewer)
        { }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            closed = getCode(" 70", 0) > 0;
            thickness = getCode(" 39", thickness);
            widthConstant = getCode(" 43", widthConstant);
            vertices = new PolylineVertex[closed ? getCode(" 90", 2) + 1 : getCode(" 90", 2)];
            //bulge = new double[getCode(" 90", 2)];

            int i = 0;
            while (!(section[i + 1].Equals(" 10") || section[i + 1].Equals(" 42"))) { i++; }

            readStatus status = readStatus.seek;
            int vertexCount = 0;
            while(i < section.Count - 1)
            {
                switch(status)
                {
                    case readStatus.x:
                        vertices[vertexCount].vertex.X = section[i].ConvertToDoubleWithCulture();
                        if(vertexCount < vertices.Length - 1)
                        {
                            vertices[vertexCount + 1].lastVertex.X = vertices[vertexCount].vertex.X;
                        }
                        status = readStatus.seek;
                        break;
                    case readStatus.y:
                        vertices[vertexCount].vertex.Y = section[i].ConvertToDoubleWithCulture();
                        if (vertexCount < vertices.Length - 1)
                        {
                            vertices[vertexCount + 1].lastVertex.Y = vertices[vertexCount].vertex.Y;
                        }
                        vertexCount++;
                        status = readStatus.seek;
                        break;
                    case readStatus.bulge:
                        if(vertexCount == vertices.Length)
                        {
                            vertices[vertexCount - 1].bulge = section[i].ConvertToDoubleWithCulture();
                        }
                        else
                        {
                            vertices[vertexCount].bulge = section[i].ConvertToDoubleWithCulture();
                        }
                        status = readStatus.seek;
                        break;
                    case readStatus.beginWidth:
                        vertices[vertexCount - 1].bWidth = section[i].ConvertToDoubleWithCulture();
                        status = readStatus.seek;
                        break;
                    case readStatus.endWidth:
                        vertices[vertexCount - 1].eWidth = section[i].ConvertToDoubleWithCulture();
                        status = readStatus.seek;
                        break;
                    case readStatus.seek:
                        switch(section[i])
                        {
                            case " 10":
                                status = readStatus.x;
                                break;
                            case " 20":
                                status = readStatus.y;
                                break;
                            case " 40":
                                status = readStatus.beginWidth;
                                break;
                            case " 41":
                                status = readStatus.endWidth;
                                break;
                            case " 42":
                                status = readStatus.bulge;
                                break;
                        }
                        break;
                }
                i++;
            }

            if(closed)
            {
                vertices[vertices.Length - 1].vertex = vertices[0].vertex;
            }


            return this;
        }

        public override Path draw()
        {
            Path path = new Path();
            PathFigure figure = new PathFigure();
            PathGeometry geometry = new PathGeometry();
            PathFigureCollection collection = new PathFigureCollection();

            figure.StartPoint = ViewerHelper.mapToWPF(vertices[0].vertex, parent);

            foreach(PolylineVertex vertex in vertices)
            {
                if (vertex.bulge == 0)
                {
                    figure.Segments.Add(new LineSegment(ViewerHelper.mapToWPF(vertex.vertex, parent), true));
                }
                else
                {
                    double radius = calculateRadius(vertex.bulge, vertex.lastVertex, vertex.vertex);
                    Size size = new Size(radius, radius);
                    figure.Segments.Add(new ArcSegment(ViewerHelper.mapToWPF(vertex.vertex, parent), size, 0,
                       Math.Abs(vertex.bulge) >= 1, vertex.bulge > 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise, true ));
                }
            }


            figure.IsClosed = closed;
            collection.Add(figure);
            geometry.Figures = collection;
            path.Data = geometry;

            path.StrokeThickness = thickness;
            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            return path;
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
       
        public override Path draw(InsertEntity insert)
        {

            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].lastVertex.X += insert.anchor.X;
                vertices[i].lastVertex.Y += insert.anchor.Y;
                vertices[i].vertex.X += insert.anchor.X;
                vertices[i].vertex.Y += insert.anchor.Y;
            }

            Path path = draw();

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].lastVertex.X -= insert.anchor.X;
                vertices[i].lastVertex.Y -= insert.anchor.Y;
                vertices[i].vertex.X -= insert.anchor.X;
                vertices[i].vertex.Y -= insert.anchor.Y;
            }

            path.RenderTransform = insert.getTransforms(path.RenderTransform);

            return path;
        }
    }
}
