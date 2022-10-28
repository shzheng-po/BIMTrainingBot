using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Data.SqlClient;


namespace ExtractGeometry
{
    public partial class SQLGeometryForm : System.Windows.Forms.Form
    {
        Document Doc;
        SQLDBConnect sqlConnection = new SQLDBConnect();
        string database = "RevitDB";

        public SQLGeometryForm(Document doc)
        {
            InitializeComponent();
            this.Doc = doc;
        }

        /// <summary>
        /// It names wall geometry form, but it should actually be SQLForms.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SQLGeometryForm_Load(object sender, EventArgs e)
        {
            //Connects to the sql server
            this.sqlConnection.ConnectDB();
        }

        /// <summary>
        /// Button used for creating tables... for beams columns and much more
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTableCreate_Click(object sender, EventArgs e)
        {
            //Check if point, curve, wall, and their associated link tables exists in the data base
            bool elementDoesExist = TableExists(database, "Elements");
            bool vertexesDoesExist = TableExists(database, "Vertexes");
            bool elementAndEdgesDoesExits = TableExists(database, "ElementAndEdges");
            bool EdgesAndVertexesDoesExist = TableExists(database, "EdgesAndVertexes");

            //This may need to get modified to -- if any doesn't exists, just create that table instead. Probably create a function to check this. 
            //Provide warning if any of the above table exists
            if(elementDoesExist)
            {
                TaskDialog.Show("SQL Table Error", " Elements Table already exists.");
            }
            //If none exists, create table.
            else
            {
                try
                {
                    string elementTableQuery = "CREATE TABLE dbo.Elements " +
                        "(" +
                        "Id        INT NOT NULL PRIMARY KEY, " +
                        "UniqueId VARCHAR(255), " +
                        "TypeName VARCHAR(255), " +
                        "DISCIPLINE   VARCHAR(255), " +
                        //"AnalyticalModel VARCHAR(255), " +
                        "USAGE        NVARCHAR(255), " +
                        "Width VARCHAR(255), " +
                        "EdgeExtracted VARCHAR(255), " +
                        ")";

                    SqlCommand command = sqlConnection.Query(elementTableQuery);
                    command.ExecuteNonQuery();

                    //TaskDialog.Show("Create SQL Table", "Wall table created");
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("SQL Error", ex.ToString());
                }
            }

            if (vertexesDoesExist)
            {
                TaskDialog.Show("SQL Table Error", "Vertexes Table already exists.");
            }
            //If none exists, create table.
            else
            {
                try
                {
                    string vertexesTableQuery = "CREATE TABLE dbo.Vertexes" +
                                            "(" +
                                            "Id        INT, " +
                                            "UniqueId   VARCHAR(255)	, " +
                                            "X          VARCHAR(255)    NOT NULL, " +
                                            "Y          NVARCHAR(255)   NOT NULL, " +
                                            "Z          NVARCHAR(255)	NOT NULL, " +
                                            ")";

                    SqlCommand command = sqlConnection.Query(vertexesTableQuery);
                    command.ExecuteNonQuery();

                    //TaskDialog.Show("Create SQL Table", "Point table created");
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("SQL Error", ex.ToString());
                }
            }

            if (elementAndEdgesDoesExits)
            {
                TaskDialog.Show("SQL Table Error", "Element and edges table already exists.");
            }
            //If none exists, create table.
            else
            {
                try
                {
                    string elementAndEdgesQuery = "CREATE TABLE dbo.ElementAndEdges" +
                                                "(" +
                                                "ElementId   INT," +
                                                "EdgeId  INT, " +
                                                ")";

                    SqlCommand command = sqlConnection.Query(elementAndEdgesQuery);
                    command.ExecuteNonQuery();

                    //TaskDialog.Show("Create SQL Table", "Wall curve table created");
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("SQL Error", ex.ToString());
                }
            }

            if (EdgesAndVertexesDoesExist)
            {
                TaskDialog.Show("SQL Table Error", "Edges and vertexes table already exists.");
            }
            //If none exists, create table.
            else
            {
                try
                {
                    string edgesAndVertexesQuery = "CREATE TABLE dbo.EdgesAndVertexes" +
                                                "(" +
                                                "VertexId   INT," +
                                                "EdgeId   INT," +
                                                "Status  VARCHAR(255), " +
                                                ")";

                    SqlCommand command = sqlConnection.Query(edgesAndVertexesQuery);
                    command.ExecuteNonQuery();

                    //TaskDialog.Show("Create SQL Table", "Point curve table created");
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("SQL Error", ex.ToString());
                }
            }

            TaskDialog.Show("Create SQL Table", "Tables are created.");

        }

        /// <summary>
        /// Check if table exists for the provided database and name of table
        /// </summary>
        /// <param name="database"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool TableExists(string database, string name)
        {
            try
            {
                string existsQuery = "select " +
                    "case " +
                    "when exists((select * FROM [" + database + "].sys.tables " + "WHERE name = '" + name + "')) then 1 " +
                    "else 0 " +
                    "end";

                SqlCommand command = sqlConnection.Query(existsQuery);
                return (int)command.ExecuteScalar() == 1;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.ToString());
                return true;
            }
        }

        /// <summary>
        /// Execute geometry extraction methods for all geometry element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void ExecuteExtractionsWith<T>() where T : GeometryDataTool, new()
        {
            T collector = new T();
            collector.SetInsertQueries();
            collector.SetDocument(this.Doc);
            collector.ExtractGeometryData();
            collector.ReportResults(sqlConnection);
        }

        /// <summary>
        /// Export Wall Data Button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportData_Click(object sender, EventArgs e)
        {
            ExecuteExtractionsWith<WallGeometryDataTool>();
            ExecuteExtractionsWith<FloorGeometryDataTool>();
            ExecuteExtractionsWith<ColumnGeometryDataTool>();
            ExecuteExtractionsWith<FramingGeometryDataTool>();

            TaskDialog.Show("Data Export", "Data are successfully added!");
        }

        private void btnDropTable_Click(object sender, EventArgs e)
        {
            bool elementDoesExist = TableExists(database, "Elements");
            bool vertexesDoesExist = TableExists(database, "Vertexes");
            bool elementAndEdgesDoesExits = TableExists(database, "ElementAndEdges");
            bool EdgesAndVertexesDoesExist = TableExists(database, "EdgesAndVertexes");

            if(elementDoesExist && vertexesDoesExist && elementAndEdgesDoesExits && EdgesAndVertexesDoesExist)
            {
                try
                {
                    string elementTableQuery = "DROP TABLE dbo.Elements " + "\n" +
                                               "DROP TABLE dbo.ElementAndEdges" + "\n" +
                                               "DROP TABLE dbo.Vertexes" + "\n" +
                                               "DROP TABLE dbo.EdgesAndVertexes";

                    SqlCommand command = sqlConnection.Query(elementTableQuery);
                    command.ExecuteNonQuery();

                    //TaskDialog.Show("Create SQL Table", "Wall table created");
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("SQL Error", ex.ToString());
                }

                TaskDialog.Show("Delete SQL Table", "Tables are being deleted!");
            }
            else
            {
                TaskDialog.Show("Delete Table", "Table does not exist in the SQL server");
            }
        }
    }
}
