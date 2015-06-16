using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DXF.GeneralInformation
{
    class Layer
    {
        public string name = "0";
        public string lineType = "CONTINUOUS";
        public int lineColor = 0;

        public Layer()
        {

        }

        public Layer(Layer layer)
        {
            name = layer.name;
            lineType = layer.lineType;
            lineColor = layer.lineColor;
        }

        public Layer parse(List<string> section)
        {
            int i = 0;
            while(i <= section.Count - 1)
            {
                switch(section[i])
                {
                    /* Layer Name */
                    case "  2":
                        name = section[++i];
                        break;
                    /* Linetype name */
                    case "  6":
                        lineType = section[++i];
                        break;
                    /* Color number */
                    case " 62":
                        lineColor = Convert.ToInt32(section[++i]);
                        break;
                    default:
                        i++;
                        break;
                }
            }
            return this;
        }
    }
}
