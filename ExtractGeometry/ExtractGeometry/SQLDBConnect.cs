using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;


namespace ExtractGeometry
{
    /// <summary>
    /// Class for connecting to SQL Server
    /// </summary>
    public class SQLDBConnect
    {
        public static SqlConnection connect;

        public void ConnectDB()
        {
            //Check if database is connected and copy the connection string to the below SQLConnection method..
            connect = new SqlConnection("Data Source=DESKTOP-FNRCLIA;Initial Catalog=RevitDB;Integrated Security=True");
            connect.Open();
        }

        public SqlCommand Query(string sqlQuery)
        {
            SqlCommand cmd = new SqlCommand(sqlQuery, connect);
            return cmd;
        }
            
    }
}
