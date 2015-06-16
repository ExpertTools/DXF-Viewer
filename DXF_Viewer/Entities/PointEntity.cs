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


namespace DXF_Viewer
{
    class PointEntity : Entity
    {
        public Point location;
        public const double POINT_RADIUS = .005;
        public PointEntity(Schematic drawing, Viewer viewer)
            : base(drawing, viewer)
        {
        }

        public override Path draw()
        {
            Path path = new CircleEntity(this).draw();
            path.Stroke = Brushes.White;
            path.Fill = Brushes.White;
            return path;
        }

        public override Entity parse(List<string> listIn)
        {
            int j = 0;
            while(j <= listIn.Count - 1)
            {
                switch(listIn[j])
                {
                    case "  8":
                        this.layerName = listIn[++j].ToUpper();
                        break;
                    case " 10":
                        this.location.X = listIn[++j].ConvertToDoubleWithCulture();
                        break;
                    case " 20":
                        this.location.Y = listIn[++j].ConvertToDoubleWithCulture();
                        break;
                    case "OFFSET":
                        j++;
                        //this.drawing.xOffset = listIn[++j].ConvertToDoubleWithCulture();
                        //this.drawing.yOffset = listIn[++j].ConvertToDoubleWithCulture();
                        break;
                    default:
                        j++;
                        break;
                }
            }
            return this;
        }
    }
}
