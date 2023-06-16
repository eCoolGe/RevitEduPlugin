using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using Autodesk.Revit.DB.Electrical;

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

            Solid solid = GetSolid.Selected(doc, family, 5);
            Face geoFace = solid.Faces.get_Item(2);
            Reference geoRef = geoFace.Reference;

            // Получение параметра семейства
            FamilyParameter parameter = fm.get_Parameter("Видимость_Привод"); // Замените "Имя параметра" на фактическое имя параметра

            if (parameter != null && parameter.IsReadOnly == false && parameter.StorageType == StorageType.Integer)
            {
                using (Transaction transaction = new Transaction(doc, "Инвертирование параметра"))
                {
                    transaction.Start();

                    ConnectorElement elConnector;

                    // Получение текущего значения параметра
                    bool value = family.LookupParameter("Видимые")?.AsInteger() == 1;

                    // Установка противоположного значения параметра
                    int oppositeValue = !value ? 1 : 0;

                    if (!value)
                    {
                        elConnector = ConnectorElement.CreateElectricalConnector(doc, ElectricalSystemType.PowerBalanced, geoRef);
                        elConnector.LookupParameter("Напряжение")?.Set(40);
                    }
                    else
                    {
                        elConnector = new FilteredElementCollector(doc).OfClass(typeof(ConnectorElement))
                                            .OfCategory(BuiltInCategory.OST_ConnectorElem)
                                            .Cast<ConnectorElement>()
                                            .First(it => it.LookupParameter("Напряжение")?.AsDouble() == 40);
                        doc.Delete(elConnector.Id);
                    }


                    fm.Set(parameter, oppositeValue);

                    transaction.Commit();
                }

                // Обновление графического интерфейса Revit
                uiDoc.RefreshActiveView();
            }

            return Result.Succeeded;
        }
    }
}
