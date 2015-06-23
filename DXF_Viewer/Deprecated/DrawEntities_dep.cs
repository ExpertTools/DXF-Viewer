using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;
using DXF.GeneralInformation;

namespace DXF_Viewer
{
    /// <summary>
    /// Takes the entity information for each entity and goes to the particular entity
    /// to make the correct path to be drawn
    /// </summary>
    class DrawEntities : Entity_Old
    {
        HatchEntity hatch = new HatchEntity();
        InsertEntity insert = new InsertEntity();
        LwPolylineEntity lWPolyline = new LwPolylineEntity();
        MTextEntity mtext = new MTextEntity();
        PolyLineEntity polyline = new PolyLineEntity();
        //TextEntity text = new TextEntity();
        DimensionEntity dim = new DimensionEntity();
        public List<LayerInfo_dep> layerList{get; set;}
        public Path EntityPath { get; set; }

        /// <summary>
        /// Takes in the entity information and creates the paths
        /// </summary>
        /// <param name="shapeList">a List of entities to get the path for</param>
        /// <param name="layerListIn">layer information to get color and type correct</param>
        /// <param name="xOffsetIn">the x offset of the entity to be drawn if it is a block</param>
        /// <param name="yOffsetIn">the y offset of the entity to be drawn if it is a block</param>
        /// <returns>The path created by the entity information it was given</returns>
        public Path drawShapes(List<String> shapeList,  
           List<LayerInfo_dep> layerListIn, double height, 
           double minX, double minY, 
           double xOffsetIn, double yOffsetIn,
           Viewer topLevelViewer, Schematic drawing)   
        {

            //DrawingInfo drawing = new DrawingInfo(xOffsetIn, yOffsetIn, minY, minX, height, layerListIn);
            
            layerList = layerListIn;
            int j = 0;
            while (j < shapeList.Count)
            {
                //if the entity is an arc it will get the arc path and make entity path equal that path
                if (shapeList[j] == "ARC")
                {
                    //EntityPath = arc.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn, topLevelViewer);
                    EntityPath = new ArcEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                }
                //if the entity is a circle it will get the circle path and make entity path equal that path
                if (shapeList[j] == "CIRCLE")
                {
                    //EntityPath = circle.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn, topLevelViewer);
                    EntityPath = new CircleEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                    break;
                }
                //if the entity is an ellipse it will get the ellipse path and make entity path equal that path
                if (shapeList[j] == "ELLIPSE")
                {
                    //EntityPath = ellipse.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn, topLevelViewer);
                    EntityPath = new EllipseEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                }
                //if the entity is a hatch it will get the hatch path and make entity path equal that path
                if (shapeList[j] == "HATCH")
                {
                    EntityPath = hatch.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn);
                    j++;
                }
                //if the entity is a line it will get the line path and make entity path equal that path
                if (shapeList[j] == "LINE")
                {
                    //EntityPath = line.getInfo(shapeList, layerListIn, 
                    //   height, minX, minY, xOffsetIn, yOffsetIn, topLevelViewer);
                    EntityPath = new LineEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                }
                //if the entity is an lwPolyline it will get the lwPolyline path and make entity path equal that path
                if (shapeList[j] == "LWPOLYLINE")
                {
                    //EntityPath = lWPolyline.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn);
                    j++;
                }
                
                //if the entity is an mtext entity it will get the mtext path and make entity path equal that path
                if (shapeList[j] == "MTEXT")
                {
                    //EntityPath = mtext.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn);
                    EntityPath = new MTextEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                }
                //if the entity is a point it will get the point path and make entity path equal that path
                if (shapeList[j] == "POINT")
                {
                    // Ignore points!
                    //EntityPath = new PointEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                }
                //if the entity is a polyline it will get the polyline path and make entity path equal that path
                if (shapeList[j] == "POLYLINE")
                {
                    EntityPath = polyline.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn);
                }
                //if the entity is a solid it will get the solid path and make entity path equal that path
                if (shapeList[j] == "SOLID")
                {
                    //EntityPath = solid.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn);
                    EntityPath = new SolidEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                }
                //if the entity is a text entity it will get the text path and make entity path equal that path
                if (shapeList[j] == "TEXT")
                {
                    //EntityPath = new TextEntity(drawing, topLevelViewer).getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn);
                    EntityPath = new TextEntity(drawing, topLevelViewer).parse(shapeList).draw();
                    j++;
                }
                if (shapeList[j] == "DIMENSION")
                {
                    EntityPath = dim.getInfo(shapeList, layerListIn, height, minX, minY, xOffsetIn, yOffsetIn);
                    j++;
                }
                j++;
                break;
            } 
        return EntityPath;
        }

    }
}
