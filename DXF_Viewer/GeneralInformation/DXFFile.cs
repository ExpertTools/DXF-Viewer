using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DXF.GeneralInformation
{
    class DXFFile
    {

        public List<List<string>> styles;
        public List<List<string>> layers;
        public List<string> header;
        public Dictionary<string, List<List<string>>> entities;

        public DXFFile(string source)
        {
            //Set up DXF Sections
            styles = new List<List<string>>();
            layers = new List<List<string>>();
            header = new List<string>();
            entities = new Dictionary<string, List<List<string>>>();

            //Set up File Read
            StreamReader reader = new StreamReader(new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
            string line = reader.ReadLine();
            while(!line.Equals("EOF"))
            {
                //Construct Layer Section

                if (line.Equals("AcDbLayerTableRecord"))
                {
                    List<string> entry = new List<string>();
                    while (!line.Equals("  0"))
                    {
                        entry.Add(line);
                        line = reader.ReadLine();
                    }
                    layers.Add(entry);
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
                    while(!line.Equals("  0"))
                    {
                        header.Add(line);
                        line = reader.ReadLine();
                    }
                    header.Add("  0");
                }

                //Construct Style Section
                if (line.Equals("AcDbTextStyleTableRecord"))
                {
                    List<string> entry = new List<string>();
                    while (!line.Equals("  0"))
                    {
                        entry.Add(line);
                        line = reader.ReadLine();
                    }
                    styles.Add(entry);
                }
                line = reader.ReadLine();
            }

        }

        /// <summary>
        /// Seeds the Entity Dictionary with Lists for all DXF Entity types
        /// (DEPRECATED)
        /// </summary>
        private void seedEntities()
        {
            string[] entityTypes = 
            {   "3DFACE","3DSOLID", "ACAD_PROXY_ENTITY", "ARC", "ARCALIGNEDTEXT", 
                "ATTDEF", "ATTRIB", "BODY", "CIRCLE", "DIMENSION", "ELLIPSE", "HATCH",
                "IMAGE", "INSERT", "LEADER", "LINE", "LWPOLYLINE", "MLINE", "MTEXT",
                "OLEFRAME", "OLEFRAME2", "POINT", "POLYLINE", "RAY", "REGION", "RTEXT",
                "SEQEND", "SHAPE", "SPLINE", "TEXT", "TOLERANCE", "TRACE", "VERTEX",
                "VIEWPOINT", "WIPEOUT", "XLINE"};
            foreach(string type in entityTypes)
            {
                this.entities.Add(type, new List<List<string>>());
            }
        }


        // REFACTOR: this constructor should change to take in a source filename where the main loop is looking for EOF
        public DXFFile(List<string> dxfFile)
        {
            styles = new List<List<string>>();

            int i = 0;
            while (i < dxfFile.Count - 1)
            {
                //Construct Layer Section

                //Construct Entity Section

                //Construct Header Section


                //Construct Style Section
                if (dxfFile[i].Equals("AcDbSymbolTable"))
                {
                    while (!dxfFile[i].Equals("ENDTAB"))
                    {
                        if (dxfFile[i].Equals("AcDbTextStyleTableRecord"))
                        {
                            List<string> entry = new List<string>();
                            while (!dxfFile.Equals("  0"))
                            {
                                entry.Add(dxfFile[i]);
                                i++;
                            }
                            styles.Add(entry);
                        }
                        i++;
                    }
                }
                i++;
            }
        }
    }
}
