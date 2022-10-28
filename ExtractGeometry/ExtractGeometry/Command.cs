using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;

namespace ExtractGeometry
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;

            using(System.Windows.Forms.Form form = new SQLGeometryForm(doc))
            {
                if (form.ShowDialog() == DialogResult.OK)
                    return Result.Succeeded;
                else
                    return Result.Cancelled;
            }
        }
    }
}
