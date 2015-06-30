using DXF.Extensions;
using DXF.Viewer.Model;
using DXF.Viewer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;


namespace DXF.Viewer.Entities
{
    class Arc : Entity
    {

        Point center = new Point(0, 0);
        double startAngle = 0;
        double endAngle = 0;
        double radius = 0;
        double rotation = 0;

        public Arc(): base()
        { }

        public Arc(Schematic drawing, Viewer topLevelViewer)
            : base(drawing, topLevelViewer)
        { }

        override public Path draw()
        {

            //map dxf coords to wpf coords
            Point dxfCenter = this.center;
            this.center = ViewerHelper.mapToWPF(center, parent);
            //Set up wpf geometry elements
            Path path = new Path();
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            ArcSegment arc = new ArcSegment();
            Point start = calcStartPoint(this);
            Point end = calcEndPoint(this);
            
            //set up brush and color
            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.StrokeDashArray = ViewerHelper.setLineType(layer.lineType);
            arc.SweepDirection = SweepDirection.Counterclockwise;
            //set arc parameters
            figure.StartPoint = start;
            arc.Point = end;
            arc.RotationAngle = Math.Abs(endAngle - startAngle);
            arc.Size = new Size(radius, radius);
            arc.IsLargeArc = checkArcSize(this);
            //compose the path
            figure.Segments.Add(arc);
            geometry.Figures.Add(figure);
            path.Data = geometry;

            //Set up Line Thickness Binding
            Binding bind = new Binding();
            bind.Source = this.viewer;
            bind.Path = new PropertyPath("LineThickness");
            path.SetBinding(Path.StrokeThicknessProperty, bind);

            //restore dxf coordinates
            center = dxfCenter;
            return path;
        }

        override public Entity parse(List<String> section)
        {
            gatherCodes(section);
            getCommonCodes();

            center.X = getCode(" 10", center.X);
            center.Y = getCode(" 20", center.Y);
            radius = getCode(" 40", radius);
            startAngle = getCode(" 50", startAngle);
            endAngle = getCode(" 51", endAngle);
            
            return this;
        }
        /// <summary>
        ///     Static method to determine WPF Arc IsLargeArc value.
        /// </summary>
        /// <param name="arc">The DXF ArcEntity object to evaluate</param>
        /// <returns>Boolean for IsLargeArc</returns>
        public static bool checkArcSize(Arc arc)
        {
            double angleDiff = arc.endAngle - arc.startAngle;
            return !((angleDiff > 0 && angleDiff <= 180) || angleDiff <= -180);
        }
        /// <summary>
        ///     Static method to detemine the WPF end point of the arc.
        /// </summary>
        /// <param name="arc">The DXF ArcEntity object to evaluate</param>
        /// <returns>Point with values of the arc end point.</returns>
        public static Point calcEndPoint(Arc arc)
        {
            double x = (Math.Cos(arc.endAngle * (Math.PI / 180)) * arc.radius) + arc.center.X;
            double y = arc.center.Y - (Math.Sin(arc.endAngle * (Math.PI / 180)) * arc.radius);
            return new Point(x, y);
        }

        /// <summary>
        ///     Static method to detemine the WPF start point of the arc.
        /// </summary>
        /// <param name="arc">The DXF ArcEntity object to evaluate</param>
        /// <returns>Point with values of the arc start point.</returns>
        public static Point calcStartPoint(Arc arc)
        {
            double x = (Math.Cos(arc.startAngle * (Math.PI / 180)) * arc.radius) + arc.center.X;
            double y = arc.center.Y - (Math.Sin(arc.startAngle * (Math.PI / 180)) * arc.radius);
            return new Point(x, y);
        }

        public override Path draw(Insert insert)
        {
            center.X += insert.anchor.X;
            center.Y += insert.anchor.Y;

            Path path = draw();

            center.X -= insert.anchor.X;
            center.Y -= insert.anchor.Y;

            path.RenderTransform = insert.getTransforms(path.RenderTransform);

            return path;
        }
    }
}
