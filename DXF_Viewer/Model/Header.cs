using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DXF.Extensions;


namespace DXF.Viewer.Model
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
                    vars.Add(current.name, current);
                }
                else
                {
                    i++;
                }
            }
            xMin = vars["$EXTMIN"].groupCode[" 10"].ConvertToDoubleWithCulture();
            yMin = vars["$EXTMIN"].groupCode[" 20"].ConvertToDoubleWithCulture();
            xMax = vars["$EXTMAX"].groupCode[" 10"].ConvertToDoubleWithCulture();
            yMax = vars["$EXTMAX"].groupCode[" 20"].ConvertToDoubleWithCulture();
            height = yMax - yMin;
            width = xMax - xMin;
            area = (xMax - xMin) * (yMax - yMin);
        }
    }
}
