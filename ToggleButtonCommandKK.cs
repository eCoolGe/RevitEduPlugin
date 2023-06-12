using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work3
{
    [Transaction(TransactionMode.Manual)]
    public class ToggleButtonCommandKK : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            // Получение выбранного элемента
            Reference reference = uiDoc.Selection.PickObject(ObjectType.Element);
            Element element = doc.GetElement(reference.ElementId);

            // Получение параметра семейства
            Parameter parameter = element.LookupParameter("Видимость_КК"); // Замените "Имя параметра" на фактическое имя параметра

            if (parameter != null && parameter.IsReadOnly == false && parameter.StorageType == StorageType.Integer)
            {
                using (Transaction transaction = new Transaction(doc, "Инвертирование параметра"))
                {
                    transaction.Start();

                    // Получение текущего значения параметра
                    int value = parameter.AsInteger();

                    // Установка противоположного значения параметра
                    int oppositeValue = (value == 0) ? 1 : 0;
                    parameter.Set(oppositeValue);

                    transaction.Commit();
                }

                // Обновление графического интерфейса Revit
                uiDoc.RefreshActiveView();
            }

            return Result.Succeeded;
        }
    }
}
