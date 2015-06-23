using DXF.GeneralInformation;
using DXF_Viewer;
using DXF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;

namespace DXF.Entities
{
    class EntityFactory
    {
        public static Entity makeEntity(string type, List<string> section, Schematic parent, Viewer viewer)
        {
            switch(type)
            {
                case "ARC":
                    return new ArcEntity(parent, viewer).parse(section);
                case "CIRCLE":
                    return new CircleEntity(parent, viewer).parse(section);
                case "ELLIPSE":
                    return new EllipseEntity(parent, viewer).parse(section);
                case "LINE":
                    return new LineEntity(parent, viewer).parse(section);
                case "POINT":
                    return new PointEntity(parent, viewer).parse(section);
                case "TEXT":
                    return new TextEntity(parent, viewer).parse(section);
                case "SOLID":
                    return new SolidEntity(parent, viewer).parse(section);
                case "MTEXT":
                    return new MTextEntity(parent, viewer).parse(section);
                case "LWPOLYLINE":
                    return new LwPolylineEntity(parent, viewer).parse(section);
                case "POLYLINE":
                    return new PolyLineEntity(parent, viewer).parse(section);
                case "INSERT":
                    return new InsertEntity(parent, viewer).parse(section);
                default:
                    throw new EntityNotSupportedException();
            }
        }
    }

    abstract class Entity
    {
        public string handle;
        public Schematic parent;
        public Viewer viewer;

        //Experimental
        public Dictionary<string, string> groupCode = new Dictionary<string, string>();
        public Dictionary<string, List<string>> groupCodeMultiples = new Dictionary<string, List<string>>();
        public Layer layer = new Layer();
        public string layerName = "0";

        public Entity()
        {
        }

        public Entity(Schematic drawing, Viewer viewer, Layer layer)
        {
            this.parent = drawing;
            this.viewer = viewer;
            this.layer = new Layer(layer);
        }

        public Entity(Schematic drawing, Viewer viewer)
        {
            this.parent = drawing;
            this.viewer = viewer;
        }

        public Entity gatherCodes(List<string> section)
        {
            int i = 0;
            while(true)
            {
                string code = section[++i];
                string value = section[++i];
                if (!groupCode.ContainsKey(code)) groupCode.Add(code, value);
                else groupCode[code] = value;
                if (section[i + 1].Equals("  0")) break;
            }
            return this;
        }

        public Entity gatherCodesAllowMultiples(List<string> section)
        {
            int i = 0;
            while(true)
            {
                string code = section[++i];
                string value = section[++i];
                if (!groupCodeMultiples.ContainsKey(code))
                {
                    List<string> valueList = new List<string>();
                    valueList.Add(value);
                    groupCodeMultiples.Add(code, valueList);
                }
                else
                {
                    groupCodeMultiples[code].Add(value);
                }
                if (section[i + 1].Equals("  0")) break;
            }
            return this;
        }

        public List<double> getCodeM(string code, double defaultValue)
        {
            List<double> list = new List<double>();
            if(groupCodeMultiples.ContainsKey(code))
            {
                foreach(string item in groupCodeMultiples[code])
                {
                    list.Add(item.ConvertToDoubleWithCulture());
                }
            }
            else
            {
                list.Add(defaultValue);
            }
            
            return list;
        }

        public string getCode(string code, string defaultValue)
        {
            if (groupCode.ContainsKey(code))
            {
                //If the code is a case sensitive key value (layer name or style name) return in upper case
                return code.Equals("  7")  || code.Equals("  8") ?  groupCode[code].ToUpper() : groupCode[code];
            }

            return defaultValue;
        }

        public int getCode(string code, int defaultValue)
        {
            if (groupCode.ContainsKey(code))
                return Convert.ToInt32(groupCode[code]);
            else
                return defaultValue;
        }

        public double getCode(string code, double defaultValue)
        {
            if (groupCode.ContainsKey(code))
                return groupCode[code].ConvertToDoubleWithCulture();
            else
                return defaultValue;
        }

        public void getCommonCodes()
        {
            handle = getCode("  5", handle);
            layerName = getCode("  8", layerName);
            layer = new Layer(parent.layers[layerName]);
            layer.lineColor = getCode(" 62", layer.lineColor);
            layer.lineType = getCode("  6", layer.lineType);
        }

        public abstract Entity parse(List<string> section);
        public abstract Path draw();
        public abstract Path draw(InsertEntity insert);

    }
}
