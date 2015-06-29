using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DXF.Extensions;
using DXF.Entities;
using System.Windows.Shapes;
using DXF.GeneralInformation;
using System.Windows.Media;
using DXF.Util;

namespace DXF_Viewer
{

    class InsertEntity : Entity
    {
        public string block = "";
        public Point anchor = new Point(0, 0);
        public double xScale = 1;
        public double yScale = 1;
        public double angle = 0;

        public InsertEntity()
        { }

        public InsertEntity(Schematic parent, Viewer viewer)
            : base(parent, viewer)
        { }

        public override Path draw()
        {
            parent.blocks[block].draw(this);
            return new Path();
        }

        public override Path draw(InsertEntity insert)
        {

            anchor.X += insert.anchor.X;
            anchor.Y += insert.anchor.Y;
            xScale *= insert.xScale;
            yScale *= insert.yScale;
            angle -= insert.angle;

            Path path = draw();

            anchor.X -= insert.anchor.X;
            anchor.Y -= insert.anchor.Y;
            xScale /= insert.xScale;
            yScale /= insert.yScale;
            angle += insert.angle;
            path.RenderTransform = insert.getTransforms(path.RenderTransform);

            return path;
        }

        public override Entity parse(List<string> section)
        {
            gatherCodes(section);
            getCommonCodes();

            block = getCode("  2", block);
            anchor.X = getCode(" 10", anchor.X);
            anchor.Y = getCode(" 20", anchor.Y);
            xScale = getCode(" 41", xScale);
            yScale = getCode(" 42", yScale);
            angle = getCode(" 50", angle);

            return this;
        }


        public TransformGroup getTransforms(Transform existingTransforms)
        {
            TransformGroup transforms = new TransformGroup();
            transforms.Children.Add(existingTransforms);
            transforms.Children.Add(new ScaleTransform(xScale, yScale, ViewerHelper.mapToWPF(anchor, parent).X, ViewerHelper.mapToWPF(anchor, parent).Y));
            transforms.Children.Add(new RotateTransform(-angle, ViewerHelper.mapToWPF(anchor, parent).X, ViewerHelper.mapToWPF(anchor, parent).Y));
            return transforms;
        }
    }
}
