 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXF_Viewer
{
    class EntityInfo
    {
        /// <summary>
        /// gets the information for each entity 
        /// </summary>
        /// <param name="lineIn">takes in the entire dxf file to be read in</param>
        /// <returns>List of strings of everything in entity section</returns>
        public List<string> getEntities(List<string> lineIn)
        {
            int j = 0;
            List<String> initialEntList = new List<String>();

            //Looks for the section marked entities, until then just passes through each line
            while (lineIn[j] != "ENTITIES")
            {
                j++;
            }
            //Once it hits the entities section until it hits endsec it will add each line
            //to the initial entity list called initialEntList 
            while (lineIn[j] != "ENDSEC")
            {
                initialEntList.Add(lineIn[j]);
                j++;

                if (lineIn[j] == "ENDSEC")
                    break;
            }
            //Will return a list which only contains entity information
            return initialEntList;
        }

    }
}
