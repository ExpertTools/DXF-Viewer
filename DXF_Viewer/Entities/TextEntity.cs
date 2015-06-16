using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Globalization;
using DXF.Extensions;
using DXF.Entities;
using DXF.GeneralInformation;
using DXF.Util;


namespace DXF_Viewer
{
    class TextEntity : Entity
    {
        public enum Alignment { Left, Center, Right, Aligned, Middle, Fit, BLeft, BCenter, BRight, MLeft, MCenter, MRight, TLeft, TCenter, TRight, Invalid };

        public static Alignment[,] DXF_ALIGNMENT_TABLE = { 
            { Alignment.Left,  Alignment.Center,  Alignment.Right,  Alignment.Aligned, Alignment.Middle, Alignment.Fit },      // 73 = 0
            { Alignment.BLeft, Alignment.BCenter, Alignment.BRight, Alignment.Invalid, Alignment.Invalid, Alignment.Invalid }, // 73 = 1
            { Alignment.MLeft, Alignment.MCenter, Alignment.MRight, Alignment.Invalid, Alignment.Invalid, Alignment.Invalid},  // 73 = 2
            { Alignment.TLeft, Alignment.TCenter, Alignment.TRight, Alignment.Invalid, Alignment.Invalid, Alignment.Invalid }};// 73 = 3
        Point start = new Point(0,0);
        Point purePosition = new Point(0,0);
        string styleName = "STANDARD";
        string text = "";
        double size = 1;
        double angle = 0;
        int alignmentRowIndex = 0;
        int alignmentColIndex = 0;
        DrawingStyle style;

        public TextEntity(Schematic drawing, Viewer topLevelViewer)
            : base(drawing, topLevelViewer)
        { }

        public override Path draw()
        {
            Path path = new Path();
            TransformGroup transforms = new TransformGroup();
            Point shiftedStart = ViewerHelper.mapToWPF(this.start, this.parent);

            this.text = ViewerHelper.swtichDXFSymbols(this.text);
            path.Stroke = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.Fill = new SolidColorBrush(ViewerHelper.getColor(layer.lineColor));
            path.StrokeThickness = Math.Abs(this.parent.header.yMin) > 400 ? 0.2 : 0.01;

            FormattedText text = new FormattedText(this.text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(style.getFontFamily()), .05 + this.size * (96/72), Brushes.White);

            //set font according to style
            //text.SetFontFamily(style.getFontFamily());
            
            //Adjust for difference in WPF text origin (top left) and DXF origin (bottom left)
            start.Y += text.Baseline;
            Geometry geometry = text.BuildGeometry(ViewerHelper.mapToWPF(this.start, this.parent));

            transforms.Children.Add(style.getMirrorTransform(shiftedStart));

            if(!this.text.Equals("")) transforms.Children.Add( new RotateTransform(-angle, shiftedStart.X, shiftedStart.Y));
            path.Data = geometry;
            path.RenderTransform = transforms;
            return path;
        }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            //Grab inheiritable values first
            styleName = getCode("  7", styleName);
            style = new DrawingStyle(parent.styles[styleName]);

            //Get possible local overrides
            style.obliqueAngle = getCode(" 51", style.obliqueAngle);

            int mirrorValue = getCode(" 71", 0);
            style.mirrorX = mirrorValue == 2 || mirrorValue == 6;
            style.mirrorY = mirrorValue == 4 || mirrorValue == 6;
            
            //Set entity values
            start.X = getCode(" 10", start.X);
            start.Y = getCode(" 20", start.Y);
            purePosition.X = getCode(" 11", purePosition.X);
            purePosition.Y = getCode(" 21", purePosition.Y);
            text = getCode("  1", text);
            size = getCode(" 40", size);
            angle = getCode(" 50", angle);
            alignmentColIndex = getCode(" 72", alignmentColIndex);
            alignmentRowIndex = getCode(" 73", alignmentRowIndex);

            return this;
        }
    }
}
