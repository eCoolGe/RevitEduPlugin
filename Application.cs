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

            #region 1. Мой первый плагин
            {
                RibbonPanel panel = application.CreateRibbonPanel(tabName, "Первый плагин");

                panel.AddItem(new PushButtonData(nameof(Class1), "Приветствие", assemblyLocation, typeof(Class1).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "green.png"))
                });
            }
            #endregion

            #region 2. Кнопка изменения размеров и две другие кнопки
            {
                RibbonPanel panel = application.CreateRibbonPanel(tabName, "Изменить семейство");
                panel.AddItem(new PushButtonData(nameof(ModifyFamilyParameters), "Изменить размер", assemblyLocation, typeof(ModifyFamilyParameters).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "red.png"))
                });
                panel.AddItem(new PushButtonData(nameof(ToggleButtonCommandKK), "КК", assemblyLocation, typeof(ToggleButtonCommandKK).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "blue.png"))
                });
                panel.AddItem(new PushButtonData(nameof(ToggleButtonCommandDrive), "Привод", assemblyLocation, typeof(ToggleButtonCommandDrive).FullName)
                {
                    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "orange.png"))
                });

                //panel.AddItem(new PushButtonData(nameof(AddRemoveConnectorsCommand), "Изменить размер", assemblyLocation, typeof(AddRemoveConnectorsCommand).FullName)
                //{
                //    LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "red.png"))
                //});
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
