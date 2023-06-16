using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            var opt = new Autodesk.Revit.DB.Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                View = doc.ActiveView,
            };
            GeometryElement geoElement = family.get_Geometry(opt);
            GeometryInstance geoInstance = geoElement.First(it => it is GeometryInstance) as GeometryInstance;
            GeometryElement geoSymbol = geoInstance.GetSymbolGeometry() as GeometryElement;

            Solid geoSolid = geoSymbol.First(it => it is Solid) as Solid;

            int i = 1;
            foreach (var s in geoSymbol)
            {
                if (s is Solid)
                {
                    if (i == 5)
                    {
                        geoSolid = s as Solid;
                        break;
                    }
                    else { i++; }
                }
            }

            Face geoFace = geoSolid.Faces.get_Item(2);
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
