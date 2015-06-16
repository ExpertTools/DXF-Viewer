using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Media;
using DXF.Extensions;
using DXF.Entities;
using DXF.GeneralInformation;
using DXF.Util;


namespace DXF_Viewer
{
    class LineEntity : Entity
    {
        Point start = new Point(0,0);
        Point end = new Point(0,0);

        public LineEntity(Schematic drawing, Viewer topLevelViewer)
            :base(drawing, topLevelViewer)
        {
        }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            start.X = getCode(" 10", start.X);
            start.Y = getCode(" 20", start.Y);
            end.X = getCode(" 11", end.X);
            end.Y = getCode(" 21", end.Y);

            return this;
        }

        public override Path draw()
        {
            //init wpf object stack
            Path path = new Path();
            LineGeometry geometry = new LineGeometry();
            GeometryGroup group = new GeometryGroup();

            //translate dxf coords to wpf 
            geometry.StartPoint = ViewerHelper.mapToWPF(start, parent);
            geometry.EndPoint = ViewerHelper.mapToWPF(end, parent);

            //set up brush
            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.StrokeDashArray = new SetLineType().setLineType(layer.lineType);

            //package wpf elements
            group.Children.Add(geometry);
            path.Data = group;
            path.StrokeLineJoin = PenLineJoin.Bevel;

            //set up line thickness binding
            Binding bind = new Binding();
            bind.Source = viewer;
            bind.Path = new PropertyPath("LineThickness");
            path.SetBinding(Path.StrokeThicknessProperty, bind);

            return path;
        }
    }
}
