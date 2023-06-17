using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Work3
{
    [Transaction(TransactionMode.Manual)]
    internal class ModifyDuctConnectors : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                FamilyInstance family = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .Cast<FamilyInstance>()
                .First(it => it.Name == "Рамочный каркас");

                List<ConnectorElement> ductConnectors = new FilteredElementCollector(doc)
                    .OfClass(typeof(ConnectorElement))
                    .OfCategory(BuiltInCategory.OST_ConnectorElem)
                    .Cast<ConnectorElement>()
                    .Where(it => it.Domain == Domain.DomainHvac)
                    .ToList();

                double familyHeight = family.LookupParameter("ADSK_Размер_Высота").AsDouble();
                double familyWidth = family.LookupParameter("ADSK_Размер_Ширина").AsDouble();

                Solid solid = GetSolid.Selected(doc, family, 1);
                Face geoFace1 = solid.Faces.get_Item(0);
                Face geoFace2 = solid.Faces.get_Item(1);
                Reference geoRef1 = geoFace1.Reference;
                Reference geoRef2 = geoFace2.Reference;
                using (Transaction transaction = new Transaction(doc, "Инвертирование параметра"))
                {
                    transaction.Start();
                    if (ductConnectors.Count == 0)
                    {
                        ConnectorElement ductConnector1 = ConnectorElement.CreateDuctConnector(doc, DuctSystemType.Global, ConnectorProfileType.Rectangular, geoRef1);
                        ConnectorElement ductConnector2 = ConnectorElement.CreateDuctConnector(doc, DuctSystemType.Global, ConnectorProfileType.Rectangular, geoRef2);

                        // Изменение ширины и высоты коннекторов
                        ductConnector1.LookupParameter("Высота")?.Set(familyHeight);
                        ductConnector1.LookupParameter("Ширина")?.Set(familyWidth);
                        ductConnector2.LookupParameter("Высота")?.Set(familyHeight);
                        ductConnector2.LookupParameter("Ширина")?.Set(familyWidth);

                        // Соединение коннекторов
                        ductConnector1.SetLinkedConnectorElement(ductConnector2);
                    }
                    else
                    {
                        foreach (var connector in ductConnectors)
                        {
                            doc.Delete(connector.Id);
                        }
                    }

                    transaction.Commit();
                }
                // Обновление графического интерфейса Revit
                uiDoc.RefreshActiveView();

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
