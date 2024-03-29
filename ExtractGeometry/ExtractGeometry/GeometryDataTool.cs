﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using Autodesk.Revit;
using System.Data.SqlClient;


namespace ExtractGeometry
{
    /// <summary>
    /// The wall geometry data extraction specialized class
    /// </summary>
    class WallGeometryDataTool : GeometryDataTool
    {
       
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Walls";
        }
    }
    /// <summary>
    /// The floor geometry data extraction specialized class
    /// </summary>
    class FloorGeometryDataTool : GeometryDataTool
    {
        
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = collector.OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType().ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Floors";
        }
    }
    /// <summary>
    /// The framing geometry data extraction specialized class
    /// </summary>
    class FramingGeometryDataTool : GeometryDataTool
    {
        
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = collector.OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Structural Framing";
        }
    }
    /// <summary>
    /// The column geometry data extraction specialized class
    /// </summary>
    class ColumnGeometryDataTool : GeometryDataTool
    {
        
        protected override void CollectElements()
        {
            FilteredElementCollector collector = new FilteredElementCollector(m_doc);
            m_elementsToProcess = collector.OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements();
        }

        protected override string GetElementTypeName()
        {
            return "Structural Columns";
        }
    }

    abstract class GeometryDataTool
    {
        #region Fields
        protected Document m_doc;
        protected Autodesk.Revit.ApplicationServices.Application m_app;

        /// <summary>
        /// The list of elements for geometry extraction
        /// </summary>
        protected IList<Element> m_elementsToProcess;

        /// <summary>
        /// The query string for inserting to current element table
        /// Contains: ElementId, UniqueId, TypeName, USAGE, Structural Type, Material
        /// </summary>
        protected string m_elementInsertQuery;

        /// <summary>
        /// The query string for inserting to current element and their curves table
        /// Contains: Element Id, Curve Id
        /// </summary>
        protected string m_elementCurveInsertQuery;

        /// <summary>
        /// The query string for inserting curves and their start and end points table
        /// Contains: Curve Id, Start point, End point
        /// </summary>
        protected string m_elementCurvePointInsertQuery;

        /// <summary>
        /// The query string for inserting points table
        /// Contains: Point Id, X, Y, Z
        /// </summary>
        protected string m_elementPointInsertQuery;

        /// <summary>
        /// The query string for inserting to model creation table;
        /// </summary>
        protected string m_elementModelCreationInsertQuery;
        #endregion

        #region Result Storage 
        /// <summary>
        /// All Collections of curves to be inserted to curve table
        /// Lists of points, the first one in the list is always the start point, the last one in the list is always the end point. 
        /// [curveID, lists of points]
        /// </summary>
        private Dictionary<ElementId, ElementInfo> m_elementCollections = new Dictionary<ElementId, ElementInfo>();

        /// <summary>
        /// list of elements that's deleted after sorting out the host element.
        /// </summary>
        private List<ElementId> m_hostedElements = new List<ElementId>();

        /// <summary>
        /// All collection of warnings geneerated due to failure to delete elements in advance of geometry extraction.
        /// </summary>
        private List<String> m_warningsForGeometryDataExtraction = new List<String>();
        #endregion


        /// <summary>
        /// override this to set queries for geometry extraction
        /// </summary>
        public void SetInsertQueries()
        {
            //insert query for inserting to element table;
            m_elementInsertQuery = "INSERT INTO " + "dbo.Elements" +
                "(Id, UniqueId, TypeName, USAGE, Width) " +
                "VALUES (@param1, @param2, @param3, @param4, @param5)";

            //query for inserting to element curve table
            m_elementCurveInsertQuery = "INSERT INTO " + "dbo.ElementAndEdges" +
                "(ElementId, EdgeId, isLocationCurve) " +
                "VALUES (@param1, @param2, @param3)";

            //insert query for inserting to point curve table
            m_elementCurvePointInsertQuery = "INSERT INTO " + "dbo.EdgesAndVertexes" +
                "(VertexId, EdgeId, Status) " +
                "VALUES (@param1, @param2, @param3)";

            //insert query for inserting to point table
            m_elementPointInsertQuery = "INSERT INTO " + "dbo.Vertexes" +
                "(Id, X, Y, Z) " +
                "VALUES (@param1, @param2, @param3, @param4)";

            m_elementModelCreationInsertQuery = "INSERT INTO " + "dbo.ModelCreationInfo" +
                "(ElementId, LevelId, WallTypeId, Height, BaseOffset, Flip, TopConstraint, TopOffset, isStructural)" +
                "VALUES (@param1, @param2, @param3, @param4, @param5, @param6, @param7, @param8, @param9)";
        }

        /// <summary>
        /// override this to populate the list of elements for geometry extraction
        /// </summary>
        protected abstract void CollectElements();

        /// <summary>
        /// override this to return the name of the element type collected for this geometry extraction
        /// </summary>
        protected abstract String GetElementTypeName();

        /// <summary>
        /// Sets the document for the GeometryData class
        /// </summary>
        /// <param name="d"></param>
        public void SetDocument(Document d)
        {
            m_doc = d;
            m_app = d.Application;
        }

        /// <summary>
        /// Execute the geometry data extraction
        /// </summary>
        public void ExtractGeometryData()
        {
            CollectElements();
            RetrieveGeometryData();
        }

        /// <summary>
        /// Retrieve geometry data of targeted elements (with openings removed and joints disallowed)
        /// geometry data includes (edges and vertices) 
        /// </summary>
        private void RetrieveGeometryData()
        {
            try
            {
                using (Transaction t = new Transaction(m_doc))
                {
                    t.SetName("Delete all cutting and hoseted elements. And disallow joints");
                    t.Start();
                    CollectHostedFamilyInstanceElements();
                    DeleteOpeningAndHostedElement();
                    DisallowJoint();
                    m_doc.Regenerate();
                    foreach (Element e in m_elementsToProcess)
                    {
                        RetrieveGeometryDataOfElement(e, m_app);
                    }
                    t.RollBack();
                }
            }
            catch(Exception ex)
            {
                TaskDialog.Show("Deletion Failed", ex.ToString());
            }
            
        }

        /// <summary>
        /// retrieves and store geometry data for a given element
        /// </summary>
        /// <param name="e"></param>
        private void RetrieveGeometryDataOfElement(Element e, Autodesk.Revit.ApplicationServices.Application app)
        {
            ElementInfo eInfo = new ElementInfo();
            ModelCreation modelCreationInfo = new ModelCreation();
            List<(XYZ, int, string)> pointInfo = new List<(XYZ, int, string)>();
            List<(Curve, bool)> edgeInfo = new List<(Curve, bool)>();
            List<Curve> edges = GetEdges(e, app);
            foreach(var edge in edges)
            {
                edgeInfo.Add((edge, false));
            }

            //Retrieve element information from model 
            //previously used to use a switch statement on category
            //so that we can treat each type of revit data differently.
            switch (e.Category.Name)
            {
                case "Walls":
                    Wall wall = (Wall)e;
                    modelCreationInfo.LevelId = wall.LevelId;
                    LocationCurve wallLine = wall.Location as LocationCurve;
                    modelCreationInfo.BaseCurve = wallLine.Curve;
                    //Add curve to edges List to be stored in edges category of result storage and confirm that wallline curve is location curve
                    edges.Add(wallLine.Curve);
                    edgeInfo.Add((wallLine.Curve, true));
                    modelCreationInfo.WallTypeId = wall.WallType.Id;
                    modelCreationInfo.flip = wall.Flipped;
                    break;
                case "Structural Framing":
                    //Collect model creation info for framings
                    break;

                case "Structural Columns":
                    //Collect model creation info for columns
                    break;
                case "Floors":
                    //Collect model creation info for floors
                    break;
            }
            
            List<XYZ> vertexes = new List<XYZ>();
            for (int i = 0; i < edges.Count; i++)
            {
                XYZ startPoint = edges[i].GetEndPoint(0);
                XYZ endPoint = edges[i].GetEndPoint(1);
                (XYZ, int, string) t1 = (startPoint, i, "Start Point");
                (XYZ, int, string) t2 = (endPoint, i, "End Point");
                pointInfo.Add(t1);
                pointInfo.Add(t2);

                //Remove duplicate points
                //points can be duplicated because an endpoint of a curve could be a startpoint of another curve.
                if (vertexes.Contains(startPoint) == false)
                {
                    vertexes.Add(startPoint);
                }
                if (vertexes.Contains(endPoint) == false)
                {
                    vertexes.Add(endPoint);
                }
            }

            //Stores the data to storage class
            foreach(Parameter para in e.Parameters)
            {
                if(para.Definition.Name == "Family and Type")
                {
                    eInfo.familyNameAndType = GetParameterValue(para);
                }
                else if (para.Definition.Name == "Top Constraint")
                {
                    modelCreationInfo.TopConstraint = para.AsElementId();
                }
                else if (para.Definition.Name == "Top Offset")
                {
                    modelCreationInfo.TopOffset = para.AsDouble();
                }
                else if(para.Definition.Name == "Structural")
                {
                    modelCreationInfo.isStructural = para.AsInteger() == 1;
                }
                //else if(para.Definition.Name == "")
                //{
                //
                //}
                else
                {
                    continue;
                }
            }

            eInfo.vertexes = vertexes;
            eInfo.edges = edgeInfo;
            eInfo.vertexInfoToEdge = pointInfo;
            eInfo.ModelCreationInfo = modelCreationInfo;

            //stores data to result storage region
            m_elementCollections.Add(e.Id, eInfo);
        }

        private string GetParameterValue(Parameter para)
        {
            string defValue = string.Empty;
            //Use different method to get parameter data according to the storage type
            switch (para.StorageType)
            {
                case StorageType.Double:
                    //Convert number into metric
                    defValue = para.AsValueString();
                    break;
                case StorageType.ElementId:
                    //find out the ValueString of the Element. Usually contains the actual family name and type in the revit model.Check for exceptions. 
                    defValue = para.AsValueString();
                    //find out the name of the element
                    //ElementId id = para.AsElementId();
                    //if(id.IntegerValue >= 0)
                    //{
                    //    defValue = document.GetElement(id).Name;
                    //    
                    //}
                    //else
                    //{
                    //    defValue = id.IntegerValue.ToString();
                    //}
                    break;
                case StorageType.Integer:
                    if (ParameterType.YesNo == para.Definition.ParameterType)
                    {
                        if (para.AsInteger() == 0)
                        {
                            defValue = "False";
                        }
                        else
                        {
                            defValue = "True";
                        }
                    }
                    else
                    {
                        defValue = para.AsInteger().ToString();
                    }
                    break;
                case StorageType.String:
                    defValue = para.AsValueString();
                    break;
                default:
                    defValue = "Unexposed parameter.";
                    break;
            }
            return defValue;
        }

        /// <summary>
        /// Retrieves the edge object from a selected elements
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private List<Curve> GetEdges(Element e, Autodesk.Revit.ApplicationServices.Application app)
        {
            Options opt = app.Create.NewGeometryOptions();

            if(null != opt)
            {
                opt.ComputeReferences = true;
                opt.DetailLevel = ViewDetailLevel.Fine;
            }

            GeometryElement geoElement = e.get_Geometry(opt);

            //confirm if geometry element contains geometry solid object or geometry instances object
            int solidNum = geoElement.AsQueryable().Count(p => null != (p as Solid));
            int geoNum = geoElement.AsQueryable().Count(p => null != (p as GeometryInstance));

            List<Curve> curves = new List<Curve>();

            if (geoNum >= 1 )
            {
                foreach (GeometryInstance geomIns in geoElement)
                {
                    GeometryElement geomEle = geomIns.GetInstanceGeometry();
                    foreach (GeometryObject geomObj in geomEle)
                    {
                        Solid geomSolid = geomObj as Solid;
                        if (null != geomSolid)
                        {
                            foreach(Edge geomEdge in geomSolid.Edges)
                            {
                                Curve curve = geomEdge.AsCurve();
                                curves.Add(curve);
                            }
                        }
                    }
                }
            }
            else if (solidNum >= 1 && geoNum == 0)
            {
                foreach (GeometryObject geomObj in geoElement)
                {
                    Solid geomSolid = geomObj as Solid;
                    if (null != geomSolid)
                    {
                        foreach (Edge geomEdge in geomSolid.Edges)
                        {
                            Curve curve = geomEdge.AsCurve();
                            curves.Add(curve);
                        }
                    }
                }
            }
            else
            {
                TaskDialog.Show("Extract Geometry", "Can't find solid element to extract geometry.");
            }

            return curves;
        }

        /// <summary>
        /// Disallow joints for wall and framing elements. 
        /// </summary>
        public void DisallowJoint()
        {
            foreach(Element e in m_elementsToProcess)
            {
                try
                {
                    switch (e.Category.Name)
                    {
                        case "Walls":
                            Wall wall = e as Wall;
                            WallUtils.DisallowWallJoinAtEnd(wall, 0);
                            WallUtils.DisallowWallJoinAtEnd(wall, 1);
                            break;
                        case "Structural Framing":
                            FamilyInstance beam = e as FamilyInstance;
                            StructuralFramingUtils.DisallowJoinAtEnd(beam, 0);
                            StructuralFramingUtils.DisallowJoinAtEnd(beam, 1);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Error", ex.ToString());
                }
            }       
        }

        /// <summary>
        /// Delete openings and all hosted elements for the targeted elements.
        /// </summary>
        /// <returns></returns>
        private void DeleteOpeningAndHostedElement()
        {
            //what categories of openings are we interested in?
            BuiltInCategory[] bics = new BuiltInCategory[]
            {
                BuiltInCategory.OST_CeilingOpening,
                BuiltInCategory.OST_FloorOpening,
                BuiltInCategory.OST_ShaftOpening,
                BuiltInCategory.OST_RoofOpening,
                BuiltInCategory.OST_ArcWallRectOpening,
                BuiltInCategory.OST_ColumnOpening,
                BuiltInCategory.OST_SWallRectOpening,
            };

            IList<ElementFilter> a = new List<ElementFilter>(bics.Count());

            foreach (BuiltInCategory bic in bics)
            {
                a.Add(new ElementCategoryFilter(bic));
            }

            LogicalOrFilter categoryFilter = new LogicalOrFilter(a);

            FilteredElementCollector collector = new FilteredElementCollector(m_doc);

            ICollection<Element> cuttingElementList = collector.WherePasses(categoryFilter).ToElements();

            //Delete Cutting Elements
            foreach (Element e in cuttingElementList)
            {
                ICollection<ElementId> deletedElements = m_doc.Delete(e.Id);

                //log failed deletion attempts to the output. (These may be other situations where deletion is not possible but
                //the failure doesnt really affect the results.)
                if (deletedElements == null || deletedElements.Count < 1)
                {
                    m_warningsForGeometryDataExtraction.Add(
                        String.Format("   The tool was unable to delete the {0} named {2} (id {1})", e.GetType().Name, e.Id, e.Name));
                }
            }

            //Delete Hosted ELements
            foreach (ElementId eId in m_hostedElements)
            {
                ICollection<ElementId> deletedElements = m_doc.Delete(eId);

                //log failed deletion attempts to the output. (These may be other situations where deletion is not possible but
                //the failure doesnt really affect the results.)
                if (deletedElements == null || deletedElements.Count < 1)
                {
                    m_warningsForGeometryDataExtraction.Add(
                        String.Format("   The tool was unable to delete the element id {0})", eId));
                }
            }
        }

        /// <summary>
        /// Some cutting elements are created as generic models by the user. They are not apart of the builtInCategory element in Revit.
        /// They are collected in this function and used in the DeleteOpeningAndHostedElement method.
        /// </summary>
        /// <returns></returns>
        private void CollectHostedFamilyInstanceElements()
        {
            FilteredElementCollector famInsCollector = new FilteredElementCollector(m_doc);
            famInsCollector.OfClass(typeof(FamilyInstance));
            
            BuiltInCategory[] bics = new BuiltInCategory[]
            {
                BuiltInCategory.OST_Doors,
                BuiltInCategory.OST_Windows,
                BuiltInCategory.OST_GenericModel,
            };

            IList<ElementFilter> a = new List<ElementFilter>(bics.Count());

            foreach (BuiltInCategory bic in bics)
            {
                a.Add(new ElementCategoryFilter(bic));
            }

            LogicalOrFilter categoryFilter = new LogicalOrFilter(a);

            IList<Element> genericModels = famInsCollector.WherePasses(categoryFilter).WhereElementIsNotElementType().ToElements();

            foreach (FamilyInstance famInsE in genericModels)
            {
                foreach(Element e in m_elementsToProcess)
                {
                    if(e.Id.ToString() == famInsE.Host.Id.ToString())
                    {
                        m_hostedElements.Add(famInsE.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Exporting results to a SQL server
        /// </summary>
        /// <param name="sqlConnection"></param>
        public void ReportResults(SQLDBConnect sqlConnection)
        {
            //Retrieve data from storage class and insert into SQL Database
            foreach (var elementCollection in m_elementCollections)
            {
                ElementId eId = elementCollection.Key;
                Element e = m_doc.GetElement(eId);
                using (SqlCommand command = sqlConnection.Query(m_elementInsertQuery))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@param1", int.Parse(eId.ToString()));
                        command.Parameters.AddWithValue("@param2", e.UniqueId);
                        command.Parameters.AddWithValue("@param3", elementCollection.Value.familyNameAndType.ToString());
                        command.Parameters.AddWithValue("@param4", e.Category.Name.ToString());
                        //command.Parameters.AddWithValue("@param5", model.ToString());
                        command.Parameters.AddWithValue("@param5", "8");
                        //command.Parameters.AddWithValue("@param6", curves.Count.ToString());
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("SQL Insert Error", ex.ToString());
                    }
                }

                //Insert data into Element curve table
                for (int i = 0; i < elementCollection.Value.edges.Count; i++)
                {
                    using (SqlCommand command = sqlConnection.Query(m_elementCurveInsertQuery))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@param1", int.Parse(eId.ToString()));
                            command.Parameters.AddWithValue("@param2", i);
                            command.Parameters.AddWithValue("@param3", elementCollection.Value.edges[i].Item2);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("SQL Insert Error", ex.ToString());
                        }
                    }
                }

                //Insert data into vertexes table 
                for (int i = 0; i < elementCollection.Value.vertexes.Count; i++)
                {
                    using (SqlCommand command = sqlConnection.Query(m_elementPointInsertQuery))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@param1", i);
                            command.Parameters.AddWithValue("@param2", elementCollection.Value.vertexes[i].X.ToString());
                            command.Parameters.AddWithValue("@param3", elementCollection.Value.vertexes[i].Y.ToString());
                            command.Parameters.AddWithValue("@param4", elementCollection.Value.vertexes[i].Z.ToString());
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("SQL Insert Error", ex.ToString());
                        }
                    }
                }

                //Insert point relation data to curve data table
                for (int i = 0; i < elementCollection.Value.vertexInfoToEdge.Count; i++)
                {
                    XYZ currentPoint = elementCollection.Value.vertexInfoToEdge[i].Item1;
                    int curveIndex = elementCollection.Value.vertexInfoToEdge[i].Item2;
                    string currentPointStatus = elementCollection.Value.vertexInfoToEdge[i].Item3;

                    using (SqlCommand command = sqlConnection.Query(m_elementCurvePointInsertQuery))
                    {
                        try
                        {
                            command.Parameters.AddWithValue("@param1", elementCollection.Value.vertexes.IndexOf(currentPoint));
                            command.Parameters.AddWithValue("@param2", curveIndex);
                            command.Parameters.AddWithValue("@param3", currentPointStatus);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            TaskDialog.Show("SQL Insert Error", ex.ToString());
                        }
                    }
                }

                //Insert Data into model creation info
                using (SqlCommand command = sqlConnection.Query(m_elementModelCreationInsertQuery))
                {
                    try
                    {
                        command.Parameters.AddWithValue("@param1", int.Parse(eId.ToString()));
                        if (elementCollection.Value.ModelCreationInfo.LevelId != null)
                            command.Parameters.AddWithValue("@param2", int.Parse(elementCollection.Value.ModelCreationInfo.LevelId.ToString()));
                        else
                        {
                            command.Parameters.AddWithValue("@param2", null);
                            continue;
                        }

                        if (elementCollection.Value.ModelCreationInfo.WallTypeId != null)
                            command.Parameters.AddWithValue("@param3", int.Parse(elementCollection.Value.ModelCreationInfo.WallTypeId.ToString()));
                        else
                        {
                            command.Parameters.AddWithValue("@param3", null);
                            continue;
                        }
                        command.Parameters.AddWithValue("@param4", elementCollection.Value.ModelCreationInfo.Height);
                        command.Parameters.AddWithValue("@param5", elementCollection.Value.ModelCreationInfo.Offset);
                        command.Parameters.AddWithValue("@param6", elementCollection.Value.ModelCreationInfo.flip);
                        if (elementCollection.Value.ModelCreationInfo.TopConstraint != null)
                            command.Parameters.AddWithValue("@param7", int.Parse(elementCollection.Value.ModelCreationInfo.TopConstraint.ToString()));
                        else
                        {
                            command.Parameters.AddWithValue("@param7", null);
                            continue;
                        }
                        command.Parameters.AddWithValue("@param8", elementCollection.Value.ModelCreationInfo.TopOffset);
                        command.Parameters.AddWithValue("@param9", elementCollection.Value.ModelCreationInfo.isStructural);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        TaskDialog.Show("SQL Insert Error", ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// A storage class for the extracted element informations
        /// </summary>
        class ElementInfo
        {
            /// <summary>
            /// string of family name and type of an element.
            /// </summary>
            public string familyNameAndType { get; set; }

            /// <summary>
            /// Material of an element
            /// </summary>
            public string material { get; set; }

            /// <summary>
            /// Lists of edges that a element contains
            /// </summary>
            public List<(Curve, bool)> edges { get; set; }

            /// <summary>
            /// List that stores points, relations to the curve at index, and it's associated position to whether it is a startpoint or endpoint
            /// </summary>
            public List<(XYZ, int, string)> vertexInfoToEdge { get; set; }

            public List<XYZ> vertexes { get; set; }

            //add in the ModelCreationInfo class as element info
            //this will be used to extract to database later
            public ModelCreation ModelCreationInfo { get; set;}
        }

        /// <summary>
        /// information about model creations
        /// Store all of the information about 
        /// walls, columns, floors, framing elements in one class
        /// </summary>
        class ModelCreation
        {
            #region Shared Creation Info
            //Level Information
            public ElementId LevelId { get; set; }
            #endregion

            #region Wall Specific Creation Info
            //Wall centerline that determines the direction and length of wall
            public Curve BaseCurve { get; set; }

            //Stores if this wall is structural element
            public bool isStructural { get; set; }

            //typeId
            public ElementId WallTypeId { get; set; }

            //height 
            public double Height { get; set; }

            //base offset of the wall  
            public double Offset { get; set; }

            //flip wether a wall is flip in its orientation or not
            public bool flip { get; set; }  

            //curve defining profile of a wall
            public IList<Curve> Profile { get; set; }

            //Top constraint
            public ElementId TopConstraint { get; set; }

            //Top Offset
            public double TopOffset { get; set; }
            #endregion

        }
    }
}
