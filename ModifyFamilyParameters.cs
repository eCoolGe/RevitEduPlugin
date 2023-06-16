using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Linq;

namespace Work3
{
    [Transaction(TransactionMode.Manual)]
    public class ModifyFamilyParameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;
            FamilyManager fm = doc.FamilyManager;

            try
            {
                FamilyInstance family = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                .OfCategory(BuiltInCategory.OST_GenericModel)
                .Cast<FamilyInstance>()
                .First(it => it.Name == "Рамочный каркас");

                double familyHeight = family.LookupParameter("ADSK_Размер_Высота").AsDouble() * 304.8;
                double familyWidth = family.LookupParameter("ADSK_Размер_Ширина").AsDouble() * 304.8;

                FamilyParameter heightParameter = fm.get_Parameter("ADSK_Размер_Высота");
                FamilyParameter widthParameter = fm.get_Parameter("ADSK_Размер_Ширина");

                if (widthParameter != null || widthParameter != null)
                {
                    using (InputForm inputForm = new InputForm(familyHeight, familyWidth))
                    {
                        if (inputForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            // Изменение значения параметров
                            using (Transaction trans = new Transaction(doc, "Изменение параметров"))
                            {
                                if (trans.Start() == TransactionStatus.Started)
                                {
                                    // Преобразование мм в футы (1 фут = 304.8 мм)
                                    fm.Set(heightParameter, inputForm.doubleHeight / 304.8);
                                    fm.Set(widthParameter, inputForm.doubleWidth / 304.8);
                                    trans.Commit();
                                }
                            }

                            TaskDialog td = new TaskDialog("Успех! Новые значения -");
                            td.MainContent = $"ASDK_Размер_Ширина: {inputForm.doubleWidth} мм\nASDK_Размер_Высота: {inputForm.doubleHeight} мм";
                            TaskDialogResult result = td.Show();
                        }
                    }
                }
                else
                    TaskDialog.Show("Ошибка", "Семейство не содержит таких параметров");

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
