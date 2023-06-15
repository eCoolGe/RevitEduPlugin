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
    public class ToggleButtonCommandDrive : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            FamilyManager fm = doc.FamilyManager;
            FamilyInstance family = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_MechanicalEquipment)
                .Cast<FamilyInstance>()
                .First(it => it.Name == "ВЕЗА_КПУ-2Н_ЭП_КП");

            // Получение параметра семейства
            FamilyParameter parameter = fm.get_Parameter("Видимость_Привод"); // Замените "Имя параметра" на фактическое имя параметра

            if (parameter != null && parameter.IsReadOnly == false && parameter.StorageType == StorageType.Integer)
            {
                using (Transaction transaction = new Transaction(doc, "Инвертирование параметра"))
                {
                    transaction.Start();

                    
                    // Получение текущего значения параметра
                    bool value = family.LookupParameter("Видимые")?.AsInteger() == 1;

                    // Установка противоположного значения параметра
                    int oppositeValue = !value ? 1 : 0;
                    fm.Set(parameter,oppositeValue);

                    transaction.Commit();
                }

                // Обновление графического интерфейса Revit
                uiDoc.RefreshActiveView();
            }

            return Result.Succeeded;
        }
    }
}
