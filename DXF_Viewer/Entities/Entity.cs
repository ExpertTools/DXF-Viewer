using DXF.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows;
using DXF.Viewer.Model;

namespace DXF.Viewer.Entities
{
    /// <summary>
    /// This factory class provides a singular way to turn a list of strings describing a DXF entity into the c# representation. 
    /// </summary>
    class EntityFactory
    {
        /// <summary>
        /// The static factory method to construct the correct Entity sub class based on the type provided.
        /// </summary>
        /// <param name="type">The DXF entity type to create.</param>
        /// <param name="section">The list of strings that hold the DXF group codes for the entity.</param>
        /// <param name="parent">The Schematic that this entity belongs to.</param>
        /// <param name="viewer">The Viewer object that this Entity will be a member of.</param>
        /// <returns></returns>
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
                case "LEADER":
                    return new LeaderEntity(parent, viewer).parse(section);
                default:
                    throw new EntityNotSupportedException();
            }
        }
    }

    abstract class Entity
    {
        public string handle;
        public string layerName = "0";
        public Schematic parent;
        public Viewer viewer;
        public Dictionary<string, string> groupCode = new Dictionary<string, string>();
        public Layer layer = new Layer();

        //Experimental
        public Dictionary<string, List<string>> groupCodeMultiples = new Dictionary<string, List<string>>();

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

        /// <summary>
        ///  Gather all group code -> Value pairings in the provided section of DXF and add them to the Entity's groupCode dictionary.
        /// </summary>
        /// <param name="section">The section of DXF spec to parse for group codes.</param>
        /// <returns>The Entity gatherCodes was called on. </returns>
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

        /// <summary>
        ///  Gathers all group code -> value pairings in the provided section of DXF and add them to the Entitys groupCodeMultiples dictionary.
        ///  This populates the dictionary of type string, List(String) to accomodate the possibility of a group code having multiple values.
        /// </summary>
        /// <param name="section">The section of DXF spec to parse for group codes.</param>
        /// <returns>The Entity that this method was called on. </returns>
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

        /// <summary>
        ///  Gets the value associated with the given code in the groupCodes dictionary if it exists. Returns the passed default value if the key is not found.
        ///  If the code corresponding to the layer name or style name it will return an upper case version of the string to ensure that it will be a compatible key.
        /// </summary>
        /// <param name="code">The DXF group code.</param>
        /// <param name="defaultValue">The default value to be returned if no key matches group code. </param>
        /// <returns>The value corresponding to the group code.</returns>
        public string getCode(string code, string defaultValue)
        {
            if (groupCode.ContainsKey(code))
            {
                //If the code is a case sensitive key value (layer name or style name) return in upper case
                return code.Equals("  7")  || code.Equals("  8") ?  groupCode[code].ToUpper() : groupCode[code];
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the value associated with the given code in the groupCodes dictionary if it exists. Returns the passed default value if the key is not found.
        /// If the default value is an int, the value matching the key will be returned as an int.
        /// </summary>
        /// <param name="code">The DXF group code.</param>
        /// <param name="defaultValue">The default value to be returned if no key matches group code.</param>
        /// <returns>The int value corresponding to the group code.</returns>
        public int getCode(string code, int defaultValue)
        {
            if (groupCode.ContainsKey(code))
                return Convert.ToInt32(groupCode[code]);
            else
                return defaultValue;
        }

        /// <summary>
        /// Gets the value associated with the given code in the groupCodes dictionary if it exists. Returns the passed default value if the key is not found.]
        /// If the default value is a double, the value matching the key will be returned as a double.
        /// </summary>
        /// <param name="code">The DXF group code.</param>
        /// <param name="defaultValue">The default value to be returned if no key maches group code. </param>
        /// <returns>The double value corresponding to the group code. </returns>
        public double getCode(string code, double defaultValue)
        {
            if (groupCode.ContainsKey(code))
                return groupCode[code].ConvertToDoubleWithCulture();
            else
                return defaultValue;
        }
        /// <summary>
        /// Helper method to fetch the common group codes for an entity. Will create the layer specified by the DXF entity's layer group code.
        /// If the entity overrides the layer's line color or line type, this method will set those values to be used over the layer's specified values.
        /// </summary>
        public void getCommonCodes()
        {
            handle = getCode("  5", handle);
            layerName = getCode("  8", layerName);
            layer = new Layer(parent.layers[layerName]);
            layer.lineColor = getCode(" 62", layer.lineColor);
            layer.lineType = getCode("  6", layer.lineType);
        }
        /// <summary>
        /// The abstract version of the method used to convert the list of strings into the c# representation of the DXF entity.
        /// </summary>
        /// <param name="section">The DXF spec of an Entity to be parsed for group codes.</param>
        /// <returns>The Entity that this method was called on.</returns>
        public abstract Entity parse(List<string> section);
        /// <summary>
        /// Uses the c# representation of the DXF entity to create a WPF Path. 
        /// </summary>
        /// <returns>The WPF Path containing the geometry representing the DXF Entity.</returns>
        public abstract Path draw();
        /// <summary>
        /// Method used by Block to create WPF Paths that are offset and scaled according to the InsertEntity that invoked the Block. 
        /// Calls the default Draw after adjusting the relevant coordinates by the offset provided by the InsertEntity.
        /// After default Draw, the relevant coordinates are adjusted back to their previous values.
        /// The Path has transforms specified by the InsertEntity added (xScale, yScale, Rotation) before it is returned.
        /// </summary>
        /// <param name="insert">The InsertEntity that invoked the draw method of the Block object.</param>
        /// <returns>The Path for and Entity that has been adjusted by the translations and transformations specified by the passed InsertEntity. </returns>
        public abstract Path draw(InsertEntity insert);

    }
}
