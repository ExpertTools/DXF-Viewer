using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using DXF.Extensions;
using DXF.Entities;
using DXF.GeneralInformation;
using DXF.Util;


namespace DXF_Viewer
{
    class CircleEntity : Entity
    {
        Point center = new Point(0,0);
        double radius = 0;

        public CircleEntity(Schematic drawing, Viewer topLevelViewer)
            : base(drawing, topLevelViewer)
        {
        }

        public CircleEntity(Schematic drawing, Viewer topLevelViewer, Point center, double radius)
            : base(drawing, topLevelViewer)
        {
            this.center = center;
            this.radius = radius;
        }

        public CircleEntity(PointEntity point)
            :base (point.parent, point.viewer)
        {
            this.center = point.location;
            this.radius = PointEntity.POINT_RADIUS;
            this.layerName = point.layerName;
        }

        public override Path draw()
        {
            //init wpf object stack
            Path path = new Path();
            EllipseGeometry geometry = new EllipseGeometry();
            GeometryGroup group = new GeometryGroup();
            //translate DXF coords to wpf
            geometry.Center = ViewerHelper.mapToWPF(this.center, this.parent);
            geometry.RadiusX = this.radius;
            geometry.RadiusY = this.radius;

            //set up brush
            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.StrokeDashArray = new SetLineType().setLineType(layer.lineType);

            //package wpf elements
            group.Children.Add(geometry);
            path.Data = group;

            //set up line thickness binding
            Binding bind = new Binding();
            bind.Source = this.viewer;
            bind.Path = new PropertyPath("LineThickness");
            path.SetBinding(Path.StrokeThicknessProperty, bind);

            return path;
        }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            center.X = getCode(" 10", center.X);
            center.Y = getCode(" 20", center.Y);
            radius = getCode(" 40", radius);
            
            return this;
        }

        public override Path draw(InsertEntity insert)
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
