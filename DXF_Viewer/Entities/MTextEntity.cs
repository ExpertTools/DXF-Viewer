using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Globalization;
using System.Text.RegularExpressions;
using DXF.Extensions;
using DXF.GeneralInformation;
using DXF.Entities;
using DXF.Util;

namespace DXF_Viewer
{
    class MTextEntity : Entity
    {
        string styleName = "STANDARD";
        string text = "";
        Point anchor = new Point(0, 0);
        Point end = new Point(0, 0);
        double size = 1;
        double rectangleWidth = 10;
        double rectangleHeight = 10;
        double charWidth = 1;
        double angle = 0;
        DrawingStyle style;

        public MTextEntity()
        { }
        
        public MTextEntity(Schematic drawing, Viewer topLevelViewer)
            : base (drawing, topLevelViewer)
        { }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            styleName = getCode("  7", styleName);
            style = new DrawingStyle(parent.styles[styleName]);

            text = getCode("  1", text);

            anchor.X = getCode(" 10", anchor.X);
            anchor.Y = getCode(" 20", anchor.Y);
            end.X = getCode(" 11", end.X);
            end.Y = getCode(" 21", end.Y);
            size = getCode(" 40", size);

            charWidth = getCode(" 42", charWidth);
            rectangleWidth = getCode(" 41", rectangleWidth);
            rectangleHeight = getCode(" 43", rectangleHeight);
            angle = getCode(" 50", angle);

            return this;
        }

        public override Path draw()
        {
            Path path = new Path();

            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.Fill = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.StrokeThickness = Math.Abs(this.parent.header.yMin) > 400 ? 0.2 : 0.01;

            FormattedText text = new FormattedText(ViewerHelper.swtichDXFSymbols(this.text), 
                CultureInfo.CurrentCulture, 
                FlowDirection.LeftToRight, 
                new Typeface(style.getFontFamily()), 
                size, 
                new SolidColorBrush(ViewerHelper.getColor(layer.lineColor)));

            text.MaxTextWidth = rectangleWidth;
            Geometry geometry = text.BuildGeometry(ViewerHelper.mapToWPF(anchor, parent));
            path.Data = geometry;

            return path;
        }

        public override Path draw(InsertEntity insert)
        {
            end.X += insert.anchor.X;
            end.Y += insert.anchor.Y;
            anchor.X += insert.anchor.X;
            anchor.Y += insert.anchor.Y;

            Path path = draw();

            end.X -= insert.anchor.X;
            end.Y -= insert.anchor.Y;
            anchor.X -= insert.anchor.X;
            anchor.Y -= insert.anchor.Y;

            path.RenderTransform = insert.getTransforms(path.RenderTransform);

            return path;

        }
    }

}
