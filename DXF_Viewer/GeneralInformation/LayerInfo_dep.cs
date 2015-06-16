using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DXF_Viewer
{
    /// <summary>
    /// Gets the Layer information from the dxf file which contains the 
    /// color, line type, and name of the layer to be used to make paths
    /// </summary>
    class LayerInfo_dep
    {        
        public List<LayerInfo_dep> layerList;
        public string layerName = "0";
        public string lineType = "CONTINUOUS";
        public int lineColor = 0;
        private int j = 0;
        /// <summary>
        /// Takes the Layer information from the dxf file and takes the
        /// layer information and puts it into a List so that it can be used
        /// at a later time when making the paths for each entity
        /// </summary>
        /// <param name="lineIn">Entire file as a list of strings</param>
        /// <returns>Layer information in a list to include line color, type, and layerName</returns>
        public List<LayerInfo_dep> getLayerInfo(List<String> lineIn)
        {
            LayerInfo_dep dxfLayer = new LayerInfo_dep();
            layerList = new List<LayerInfo_dep>();

            //Until layer section is hit, this will just cycle through each line
            while (lineIn[j] != "LAYER")
            {
                j++;
            }
            //Once layer section is hit it will look for the specific information until endtab is hit
            while (lineIn[j] != "ENDTAB")
            {
                dxfLayer = new LayerInfo_dep();
                while (lineIn[j] != "  0")
                {
                    //This is the layer name 
                    if (lineIn[j] == "  2")
                    {
                        dxfLayer.layerName = lineIn[++j];
                    }
                    //This is the line color for the specific layer
                    else if (lineIn[j] == " 62")
                    {
                        dxfLayer.lineColor = Convert.ToInt32(lineIn[++j]);
                    }
                    //This is the line type for the specific layer
                    else if (lineIn[j] == "  6")
                    {
                        dxfLayer.lineType = lineIn[++j];
                    }
                    j++;
                }
                //It then adds each layer to a list of layers
                layerList.Add(dxfLayer);
                j++;
            }
            return layerList;
        }
               
    }
}
