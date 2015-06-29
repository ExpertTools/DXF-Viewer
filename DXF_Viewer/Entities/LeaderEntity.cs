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
    class LeaderEntity : Entity
    {
        Point[] vertices;
        bool arrow = false;
        bool hook = false;
        string annotationReference;
        int leaderType = 3;

        enum readStatus { x, y, seek };


        public LeaderEntity (Schematic parent, Viewer viewer)
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

            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));

            return path;
        }

        public override Path draw(InsertEntity insert)
        {
            return draw();
        }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            vertices = new Point[getCode(" 76", 2)];
            arrow = getCode(" 71", 0) > 0;
            hook = getCode(" 75", 0) > 0;
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
    }
}
