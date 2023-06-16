using Autodesk.Revit.DB;
using System.Linq;

namespace Work3
{
    internal class GetSolid
    {
        public static Solid Selected(Document doc, FamilyInstance family, int select)
        {
            Options opt = new Autodesk.Revit.DB.Options
            {
                ComputeReferences = true,
                IncludeNonVisibleObjects = true,
                View = doc.ActiveView,
            };
            GeometryElement geoElement = family.get_Geometry(opt);
            GeometryInstance geoInstance = geoElement.First(it => it is GeometryInstance) as GeometryInstance;
            GeometryElement geoSymbol = geoInstance.GetSymbolGeometry();

            Solid geoSolid = geoSymbol.First(it => it is Solid) as Solid;

            int i = 1;
            foreach (var s in geoSymbol)
            {
                if (s is Solid)
                {
                    if (i == select)
                    {
                        geoSolid = s as Solid;
                        break;
                    }
                    else { i++; }
                }
            }
            return geoSolid;
        }
    }
}
