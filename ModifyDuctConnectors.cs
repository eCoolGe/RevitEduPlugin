using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;

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

            //Reference pickedRef = uiDoc.Selection.PickObject(ObjectType.Element, "Выберите семейство");
            //Element pickedElement = doc.GetElement(pickedRef.ElementId);

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

            double famHeight = family.LookupParameter("ADSK_Размер_Высота").AsDouble();
            double famWidth = family.LookupParameter("ADSK_Размер_Ширина").AsDouble();

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
                    if (i == 1)
                    {
                        geoSolid = s as Solid;
                        break;
                    }
                    else { i++; }
                }
            }

            Face geoFace1 = geoSolid.Faces.get_Item(0);
            Face geoFace2 = geoSolid.Faces.get_Item(1);
            Reference geoRef1 = geoFace1.Reference;
            Reference geoRef2 = geoFace2.Reference;
            using (Transaction transaction = new Transaction(doc, "Инвертирование параметра"))
            {
                transaction.Start();
                if (ductConnectors.Count == 0)
                {
                    ConnectorElement ductConnector1 = ConnectorElement.CreateDuctConnector(doc, DuctSystemType.Global, ConnectorProfileType.Rectangular, geoRef1);
                    ConnectorElement ductConnector2 = ConnectorElement.CreateDuctConnector(doc, DuctSystemType.Global, ConnectorProfileType.Rectangular, geoRef2);

                    ductConnector1.LookupParameter("Высота")?.Set(famHeight);
                    ductConnector1.LookupParameter("Ширина")?.Set(famWidth);

                    ductConnector2.LookupParameter("Высота")?.Set(famHeight);
                    ductConnector2.LookupParameter("Ширина")?.Set(famWidth);

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

            //foreach (GeometryInstance obj in geoElement)
            //{
            //    Solid s = (obj.GetSymbolGeometry() as GeometryElement).First() as Solid;
            //    TaskDialog.Show("сообщение", $"{s}");
            //    if (s != null)
            //    {
            //        foreach (Face f in s.Faces)
            //        {
            //            PlanarFace pf = f as PlanarFace;
            //            if (pf != null)
            //            {
            //                using (Transaction transaction = new Transaction(doc, "Инвертирование параметра"))
            //                {
            //                    transaction.Start();

            //                    Reference reference = pf.Reference;
            //                    if (reference != null)
            //                    {
            //                        var elConnector = ConnectorElement.CreateElectricalConnector(doc, ElectricalSystemType.PowerBalanced, reference);
            //                        //var elConnector = ConnectorElement.CreateDuctConnector(doc, DuctSystemType.Fitting, ConnectorProfileType.Rectangular, reference);

            //                    }
            //                    TaskDialog.Show("сообщение", "да!");

            //                    transaction.Commit();
            //                }
            //                // Обновление графического интерфейса Revit
            //                uiDoc.RefreshActiveView();
            //            }
            //        }
            //    }
            //}
            return Result.Succeeded;
        }
    }
}
