using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;
using System.Linq;

namespace Work3
{

    [Transaction(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commanddata, ref string message, ElementSet elements)
        {
            TaskDialog.Show("сообщение", "привет, мир!");

            return Result.Succeeded;
        }
    }
}
