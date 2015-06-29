using DXF.Viewer.Entities;
using DXF.Viewer.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace DXF.Viewer.Model
{
    class Schematic : EntityComposite
    {
        public Dictionary<string, DrawingStyle> styles = new Dictionary<string, DrawingStyle>();
        public Dictionary<string, Layer> layers = new Dictionary<string, Layer>();
        public Header header;
        public List<Entity> entities = new List<Entity>();
        public Dictionary<string, Block> blocks = new Dictionary<string, Block>();

        public Schematic(DXFFile file, Viewer viewer, Canvas canvas)
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
            if (!layers.ContainsKey("0"))
            {
                this.layers.Add("0", new Layer());
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
            foreach(List<string> blockSection in file.blocks)
            {
                Block currentBlock = new Block(canvas).parse(blockSection, this, viewer);
                blocks.Add(currentBlock.name, currentBlock);
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
