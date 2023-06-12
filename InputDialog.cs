using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work3
{
    [Transaction(TransactionMode.Manual)]
    public class InputDialog : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                using (var inputForm = new InputForm())
                {
                    if (inputForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        int value1 = inputForm.intHeight;
                        int value2 = inputForm.intWidth;

                        // Здесь вы можете использовать полученные значения value1 и value2
                        // для выполнения необходимых операций
                        TaskDialog.Show("Успех", $"Данные успешно записаны. {value1} {value2}");
                    }
                }

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
