using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DXF.Viewer.Parsing
{
    class DXFFile
    {

        public List<List<string>> styles;
        public List<List<string>> layers;
        public List<List<string>> blocks;
        public List<List<string>> endBlocks;
        public List<List<string>> blockRecords;
        public List<string> header;
        public Dictionary<string, List<List<string>>> entities;

        public DXFFile(string source)
        {
            //Set up DXF Sections
            styles = new List<List<string>>();
            layers = new List<List<string>>();
            header = new List<string>();
            entities = new Dictionary<string, List<List<string>>>();
            blocks = new List<List<string>>();
            endBlocks = new List<List<string>>();
            blockRecords = new List<List<string>>();

            //Set up File Read
            StreamReader reader = new StreamReader(new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            string line = reader.ReadLine();
            while(!line.Equals("EOF"))
            {
                //Construct Layer Section

                if (line.Equals("AcDbLayerTableRecord"))
                {
                    layers.Add(readSection(reader, line));
                }
                //Construct Entity Section
                if (line.Equals("ENTITIES"))
                {
                    line = reader.ReadLine();
                    while (true)
                    {

                        if (line.Equals("  0"))
                        {
                            line = reader.ReadLine();
                            string type = line;
                            List<string> entity = new List<string>();
                            while (!line.Equals("  0"))
                            {
                                entity.Add(line);
                                line = reader.ReadLine();
                            }
                            entity.Add("  0");
                            if (type.Equals("ENDSEC")) break;
                            if (!entities.ContainsKey(type)) entities.Add(type, new List<List<String>>());
                            entities[type].Add(entity);
                        }
                    }
                }
                //Construct Header Section
                if(line.Equals("HEADER"))
                { 
                    header = readSection(reader, line);
                }

                //Construct Style Section
                if (line.Equals("AcDbTextStyleTableRecord"))
                {
                    styles.Add(readSection(reader, line));
                }
                

                //Block Records
                if(line.Equals("BLOCK_RECORD"))
                {
                    blockRecords.Add(readSection(reader, line));
                }

                //Construct Block Section
                if(line.Equals("BLOCKS"))
                {
                    line = reader.ReadLine();
                    while (true)
                    {
                        if(line.Equals("BLOCK"))
                        {
                            List<string> entry = new List<string>();
                            while(!line.Equals("ENDBLK"))
                            {
                                entry.Add(line);
                                line = reader.ReadLine();
                            }
                            blocks.Add(entry);
                        }
                        if(line.Equals("ENDBLK"))
                        {
                            endBlocks.Add(readSection(reader, line));
                        }
                        line = reader.ReadLine();
                        if (line.Equals("ENDSEC")) break;
                    }
                }

                line = reader.ReadLine();
            }
        }

        private List<string> readSection(StreamReader reader, string line)
        {
            List<string> entry = new List<string>();

            while(!line.Equals("  0"))
            {
                entry.Add(line);
                line = reader.ReadLine();
            }
            entry.Add("  0");
            return entry;
        }
    }
}
