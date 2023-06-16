using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Work3
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location,
                   iconsDirectoryPath = Path.GetDirectoryName(assemblyLocation) + @"\icons\";

            string tabName = "Работа №3";

            application.CreateRibbonTab(tabName);

            #region 1. Основное семейство
            {
                RibbonPanel panel = application.CreateRibbonPanel(tabName, "Основное семейство");

                panel.AddItem(new PushButtonData(nameof(ModifyFamilyParameters), "Размер рамки", assemblyLocation, typeof(ModifyFamilyParameters).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "green.png"))
                });
            }
            #endregion

            #region 2. Коннекторы и дополнительные элементы
            {
                RibbonPanel panel = application.CreateRibbonPanel(tabName, "Коннекторы и дополнительные элементы");
                panel.AddItem(new PushButtonData(nameof(ToggleButtonCommandKK), "КК", assemblyLocation, typeof(ToggleButtonCommandKK).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "red.png"))
                });
                panel.AddItem(new PushButtonData(nameof(ToggleButtonCommandDrive), "Привод", assemblyLocation, typeof(ToggleButtonCommandDrive).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "orange.png"))
                });
                panel.AddItem(new PushButtonData(nameof(ModifyDuctConnectors), "Коннекторы воздуховода", assemblyLocation, typeof(ModifyDuctConnectors).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "blue.png"))
                });
            }
            #endregion

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
