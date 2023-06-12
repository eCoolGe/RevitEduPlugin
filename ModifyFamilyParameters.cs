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
    public class ModifyFamilyParameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiDoc.Document;

            try
            {
                // Выбор семейства мышкой
                Reference pickedRef = uiDoc.Selection.PickObject(ObjectType.Element, "Выберите семейство");
                Element pickedElement = doc.GetElement(pickedRef.ElementId);

                // Получение значения параметров и изменение их значений
                if (pickedElement != null && pickedElement is FamilyInstance familyInstance)
                {
                    Parameter heightParameter = familyInstance.LookupParameter("ADSK_Размер_Высота");
                    Parameter widthParameter = familyInstance.LookupParameter("ADSK_Размер_Ширина");
                    if (widthParameter != null || widthParameter != null)
                    {
                        // Получение текущего значения параметров
                        //string widthValue = widthParameter.AsValueString();
                        //TaskDialog.Show("Значение параметра", $"Текущее значение ASDK_Ширина: {widthValue}");

                        using (InputForm inputForm = new InputForm())
                        {
                            if (inputForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                // Изменение значения параметров
                                using (Transaction trans = new Transaction(doc, "Изменение параметра"))
                                {
                                    if (trans.Start() == TransactionStatus.Started)
                                    {
                                        heightParameter.Set(inputForm.intHeight / 304.8);
                                        widthParameter.Set(inputForm.intWidth / 304.8); // Преобразование мм в футы (1 фут = 304.8 мм)
                                        trans.Commit();
                                    }
                                }

                                TaskDialog td = new TaskDialog("Успех! Новые значения -");
                                td.MainContent = $"ASDK_Размер_Высота: {inputForm.intHeight} мм\nASDK_Размер_Ширина: {inputForm.intWidth} мм";
                                TaskDialogResult result = td.Show();
                            }
                        }
                    }
                    else 
                        TaskDialog.Show("Ошибка", "Семейство не содержит таких параметров");    
                }
                else 
                    TaskDialog.Show("Ошибка", "Не удалось выбрать семейство");
                
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
