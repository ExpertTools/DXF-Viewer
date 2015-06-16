using DXF_Viewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXF.GeneralInformation
{
    class DrawingInfo_dep
    {
        public double xOffset { get; set; }
        public double yOffset { get; set; }
        public double xMin { get; set; }
        public double yMin { get; set; }
        public double height { get; set; }
        public List<LayerInfo_dep> layers {get;set;}
        public Dictionary<string, LayerInfo_dep> layerMap { get; set; }


        public DrawingInfo_dep()
        {
            xOffset = 0;
            yOffset = 0;
            xMin = 0;
            yMin = 0;
            height = 0;
            layers = new List<LayerInfo_dep>();
        }

        public DrawingInfo_dep(double xOff, double yOff, double yMin, double xMin, double height, List<LayerInfo_dep> layers)
        {
            this.xOffset = xOff;
            this.yOffset = yOff;
            this.yMin = yMin;
            this.xMin = xMin;
            this.height = height;
            this.layers = layers;
            this.layerMap = layers.GroupBy(layer => layer.layerName).Select(group => group.First()).ToDictionary(layer => layer.layerName);
        }

        public DrawingInfo_dep(DrawingInfo_dep drawing)
        {
            this.xOffset = drawing.xOffset;
            this.yOffset = drawing.yOffset;
            this.yMin = drawing.yMin;
            this.xMin = drawing.xMin;
            this.height = drawing.height;
            this.layers = drawing.layers;
            this.layerMap = this.layers.GroupBy(layer => layer.layerName).Select(group => group.First()).ToDictionary(layer => layer.layerName);
        }

        public DrawingInfo_dep(Schematic drawing)
        {
            this.yMin = drawing.header.yMin;
            this.xMin = drawing.header.xMin;
            this.height = drawing.header.yMax - drawing.header.yMin;

        }



    }
}
