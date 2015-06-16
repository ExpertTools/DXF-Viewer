using DXF.Entities;
using DXF_Viewer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DXF.GeneralInformation
{
    class Schematic : EntityComposite
    {
        public Dictionary<string, DrawingStyle> styles = new Dictionary<string, DrawingStyle>();
        public Dictionary<string, Layer> layers = new Dictionary<string, Layer>();
        public Header header;
        public List<Entity> entities = new List<Entity>();

        public Schematic(DXFFile file, Viewer viewer)
        {
            foreach(List<string> style in file.styles)
            {
                DrawingStyle currentStyle = new DrawingStyle().parse(style);
                this.styles.Add(currentStyle.name.ToUpper(), currentStyle);
            }
            foreach(List<string> layer in file.layers)
            {
                Layer currentLayer = new Layer().parse(layer);
                this.layers.Add(currentLayer.name.ToUpper(), currentLayer);
            }
            header = new Header(file.header);
            foreach(string type in file.entities.Keys)
            {
                foreach(List<string> section in file.entities[type])
                {
                    try
                    {
                        entities.Add(EntityFactory.makeEntity(type, section, this, viewer));
                    }
                    catch(EntityNotSupportedException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public override EntityComposite draw(Canvas canvas)
        {
            foreach(Entity current in this.entities)
            {
                canvas.Children.Add(current.draw());
            }
            return this;
        }
    }
}
