using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;
using System.IO;

namespace ExtractGeometry
{
    public class Application : IExternalApplication
    {
        /// <summary>
        /// Implement the on shut-down event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }

        /// <summary>
        /// Implement the on startup event
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = RibbonPanel(application);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            if (panel.AddItem(new PushButtonData("Extract Geometry", "Extract Geometry", thisAssemblyPath, "ExtractGeometry.Command")) is PushButton button)
            {
                button.ToolTip = "Extract the geometry information from Revit";

                Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "SQL.jpg"));
                BitmapImage bitmap = new BitmapImage(uri);
                button.LargeImage = bitmap;
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Funcation that creates ribbon panel
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "S-Develop";
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                a.CreateRibbonPanel(tab, "Commands");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel panel in panels.Where(panel => panel.Name == "Commands"))
            {
                ribbonPanel = panel;
            }

            return ribbonPanel;

        }
    }
}
