using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DXF.Extensions;


namespace DXF_Viewer
{
    class HeaderVar
    {
        public string name;
        public Dictionary<string, string> groupCode = new Dictionary<string, string>();

        public HeaderVar(string name)
        {
            this.name = name;
        }

    }

    class Header
    {
        public double xMax;
        public double xMin;
        public double yMax;
        public double yMin;
        public double height;
        public double area;
        public double width;
        public Dictionary<string, HeaderVar> vars = new Dictionary<string, HeaderVar>();

        public Header ()
        {

        }

        public Header (List<string> section)
        {
            int i = 0;
            while(i <= section.Count - 1)
            {
                if(section[i].Contains('$'))
                {
                    HeaderVar current = new HeaderVar(section[i]);
                    while(true)
                    {
                        string code = section[++i];
                        string value = section[++i];
                        current.groupCode.Add(code, value);
                        if(section[i+1].Equals("  9") || section[i+1].Equals("  0")) break;
                    }
                    this.vars.Add(current.name, current);
                }
                else
                {
                    i++;
                }
            }
            this.xMin = this.vars["$EXTMIN"].groupCode[" 10"].ConvertToDoubleWithCulture();
            this.yMin = this.vars["$EXTMIN"].groupCode[" 20"].ConvertToDoubleWithCulture();
            this.xMax = this.vars["$EXTMAX"].groupCode[" 10"].ConvertToDoubleWithCulture();
            this.yMax = this.vars["$EXTMAX"].groupCode[" 20"].ConvertToDoubleWithCulture();
            this.height = this.yMax - this.yMin;
            this.width = this.xMax - this.xMin;
            this.area = (xMax - xMin) * (yMax - yMin);
            
        }





        /// <summary>
        /// Gets the information from the header section of the dxf file
        /// </summary>
        /// <param name="lineIn">Entire dxf file is taken in as a list of strings</param>
        /// <returns>List of doubles which contain the min and max for size of canvas</returns>
        public List<double> getHeaderInfo(List<String> lineIn) 
        {
          
            double xMax;
            double xMin;
            double yMax;
            double yMin;
            List<double> sizeInfo = new List<double>();

            int j = 0;
            //Until the header section is hit this will just pass through each line
            while (lineIn[j] != "HEADER")
            {
                j++;
            }
            //Once header is hit it will look for the endsec portion and look for min and max
            while (lineIn[j] != "ENDSEC")
            {
                j++;
                // if the extmax section is hit it will get the x and y max for the canvas size
                // and will then add them to the list of size info
                if (lineIn[j] == "$EXTMAX")
                {
                    j++;
                    //the x coordinate max for the canvas
                    //xMax = Convert.ToDouble(lineIn[++j]);
                    xMax = lineIn[++j].ConvertToDoubleWithCulture();
                    sizeInfo.Add(xMax);
                    j++;
                    //the y coordinate max for the canvas
                    yMax = lineIn[++j].ConvertToDoubleWithCulture();
                    sizeInfo.Add(yMax);
                }
                //if the extmin section is hit it will get the x and y min for the canvas size
                // and will then add them to the list of size info
                else if (lineIn[j] == "$EXTMIN")
                {
                    j++;
                    //the x coordinate min for the canvas
                    xMin = lineIn[++j].ConvertToDoubleWithCulture();
                    sizeInfo.Add(xMin);
                    j++;
                    //the y coordinate min for the canvas
                    yMin = lineIn[++j].ConvertToDoubleWithCulture();
                    sizeInfo.Add(yMin);
                }
            }

            return sizeInfo;
        }
    }
}
