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
using Autodesk.Revit.UI;

namespace ExtractGeometry
{
    public partial class WallForm : System.Windows.Forms.Form
    {
        Document Doc;
        public WallForm(Document doc)
        {
            InitializeComponent();
            this.Doc = doc;
        }

        private void WallCount_Click(object sender, EventArgs e)
        {
            ICollection<Element> walls = new FilteredElementCollector(Doc, Doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Walls).ToElements();

            TaskDialog.Show("Wall Count", walls.Count.ToString() + " walls");
        }
        /// <summary>
        /// This has to be here to create the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WallForm_Load(object sender, EventArgs e)
        {
        }
    }
}
