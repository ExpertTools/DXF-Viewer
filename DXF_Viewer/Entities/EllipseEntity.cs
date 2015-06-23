using DXF.Entities;
using DXF.Extensions;
using DXF.GeneralInformation;
using DXF.Util;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;


namespace DXF_Viewer
{
    class EllipseEntity : Entity
    {
        Point center = new Point(0,0);
        Point end = new Point(0,0);
        double axisRatio = 1;

        public EllipseEntity(Schematic drawing, Viewer topLevelViewer)
            : base(drawing, topLevelViewer)
        {
        }

        public override Path draw()
        {
            //init wpf object stack
            Path path = new Path();
            EllipseGeometry geometry = new EllipseGeometry();
            GeometryGroup group = new GeometryGroup();
            //translate dxf coords to wpf
            geometry.Center = ViewerHelper.mapToWPF(center, parent);
            geometry.RadiusX = ViewerHelper.calculateAxisLength(center, end);
            geometry.RadiusY = geometry.RadiusX * axisRatio;
            //set up brush
            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.StrokeDashArray = new SetLineType().setLineType(layer.lineType);
            path.RenderTransformOrigin = geometry.Center;
            path.RenderTransform = new RotateTransform(ViewerHelper.calculateRotationAngle(end, center));
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
            end.X = getCode(" 11", end.X);
            end.Y = getCode(" 21", end.Y);
            axisRatio = getCode(" 40", axisRatio);
            return this;
        }

        public override Path draw(InsertEntity insert)
        {
            center.X += insert.anchor.X;
            center.Y += insert.anchor.Y;
            end.X += insert.anchor.X;
            end.Y += insert.anchor.Y;

            Path path = draw();

            center.X -= insert.anchor.X;
            center.Y -= insert.anchor.Y;
            end.X -= insert.anchor.X;
            end.Y -= insert.anchor.Y;

            path.RenderTransform = insert.getTransforms(path.RenderTransform);

            return path;
        }
    }
}
