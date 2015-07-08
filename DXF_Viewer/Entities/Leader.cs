using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using DXF.Extensions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using DXF.Viewer.Model;
using DXF.Viewer.Util;

namespace DXF.Viewer.Entities
{
    class Leader : Entity
    {
        Point[] vertices;
        bool arrow = false;
        bool hook = false;
        string annotationReference;
        int leaderType = 3;

        enum readStatus { x, y, seek };


        public Leader (Schematic parent, Viewer viewer)
            : base (parent, viewer)
        {
        }

        public override Path draw()
        {
            Path path = new Path();
            PathFigure figure = new PathFigure();
            PathGeometry geometry = new PathGeometry();
            PathFigureCollection collection = new PathFigureCollection();

            figure.StartPoint = ViewerHelper.mapToWPF(vertices[0], parent);
            foreach(Point vertex in vertices)
            {
                figure.Segments.Add(new LineSegment(ViewerHelper.mapToWPF(vertex, parent), true));
            }

            figure.IsClosed = false;
            collection.Add(figure);
            geometry.Figures = collection;
            path.Data = geometry;

            Binding bind = new Binding();
            bind.Source = viewer;
            bind.Path = new PropertyPath("LineThickness");
            path.SetBinding(Path.StrokeThicknessProperty, bind);
            Brush brush = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.Stroke = brush;

            if(arrow) viewer.mainCanvas.Children.Add(getArrowPath(brush, bind));

            return path;
        }

        public override Path draw(Insert insert)
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                vertices[i].X += insert.anchor.X;
                vertices[i].Y += insert.anchor.X;
            }

            Path path = draw();

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].X -= insert.anchor.X;
                vertices[i].Y -= insert.anchor.X;
            }

            return path;
        }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            vertices = new Point[getCode(" 76", 2)];
            arrow = getCode(" 71", true);
            hook = getCode(" 75", false);
            annotationReference = getCode("340", "");

            readStatus status = readStatus.seek;
            int vertexCount = 0;
            for(int i = 0; i < section.Count; i++)
            {
                switch(status)
                {
                    case readStatus.x:
                        vertices[vertexCount].X = section[i].ConvertToDoubleWithCulture();
                        status = readStatus.seek;
                        break;
                    case readStatus.y:
                        vertices[vertexCount++].Y = section[i].ConvertToDoubleWithCulture();
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
                        }
                        break;
                }
            }

            return this;
        }

        public Path getArrowPath(Brush brush, Binding bind)
        {
            Path path = new Path();
            PathFigure figure = new PathFigure();
            PathGeometry headGeo = new PathGeometry();
            PathFigureCollection headCollection = new PathFigureCollection();

            figure.StartPoint = ViewerHelper.mapToWPF(vertices[0], parent);

            double arrowHeight = parent.header.vars["$DIMASZ"].groupCode[" 40"].ConvertToDoubleWithCulture();

            Point b1 = new Point();
            Point b2 = new Point();
            b1.Y = vertices[0].Y + (arrowHeight / 6);
            b2.Y = vertices[0].Y - (arrowHeight / 6);
            b1.X = vertices[0].X + arrowHeight;
            b2.X = vertices[0].X + arrowHeight;

            figure.Segments.Add(new LineSegment(ViewerHelper.mapToWPF(b1, parent), true));
            figure.Segments.Add(new LineSegment(ViewerHelper.mapToWPF(b2, parent), true));

            figure.IsClosed = true;
            figure.IsFilled = true;

            headCollection.Add(figure);
            headGeo.Figures = headCollection;
            double rotation = getArrowRotation() * (180 / Math.PI);

            headGeo.Transform = (new RotateTransform(-rotation,
                ViewerHelper.mapToWPF(vertices[0], parent).X,
                ViewerHelper.mapToWPF(vertices[0], parent).Y));

            path.Data = headGeo;
            path.Stroke = brush;
            path.Fill = brush;
            path.SetBinding(Path.StrokeThicknessProperty, bind);

            return path;
        }

        public double getArrowRotation()
        {
            Point v1 = vertices[0];
            Point v2 = vertices[1];

            double top = v2.Y - v1.Y;
            double bottom = v2.X - v1.X;
      

            double atan = Math.Atan(top / bottom);

            if(bottom >= 0)
            {
                if(top >= 0)
                {
                    return atan;
                }
                else
                {
                    return -((2 * Math.PI) - atan);
                }
            }
            else
            {
                if(top >= 0)
                {
                    return Math.PI - atan;
                }
                else
                {
                    return Math.PI + atan;
                }
            }
        }

    }
}
