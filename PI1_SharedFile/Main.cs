﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PI1_UI;
using System.Linq;

namespace PI1_SharedFile
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    // Running interface class
    public class Main : IExternalApplication
    {

        /// <summary>
        /// Implement this method to execute some tasks when Autodesk Revit shuts down.
        /// </summary>
        /// <param name="application">A handle to the application being shut down.</param>
        /// <returns>
        /// Indicates if the external application completes its work successfully.
        /// </returns>
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        /// <summary>
        /// Implement this method to create tab, ribbon and button or add elements if tab and ribbon was created when Autodesk Revit starts.
        /// </summary>
        /// <param name="application">A handle to the application being started.</param>
        /// <returns>
        /// Indicates if the external application completes its work successfully.
        /// </returns>
        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "PI1";
            string ribbonPanelName = RibbonName.Name(RibbonNameType.Instruments);
            RibbonPanel ribbonPanel = null;

            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch { }

            try
            {
                ribbonPanel = application.CreateRibbonPanel(tabName, ribbonPanelName);
            }
            catch
            {
                ribbonPanel = application.GetRibbonPanels(tabName)
                    .FirstOrDefault(panel => panel.Name.Equals(ribbonPanelName));
            }

            var btnAssociatingParametersToGlobalData = new RevitPushButtonData
            {
                Label = "Публикация в\nSHARED",
                Panel = ribbonPanel,
                ToolTip = "Публикует текущий файл в папку SHARED проекта, отправляя старый опубликованный файл в папку ARCHIVE",
                CommandNamespacePath = Command.GetPath(),
                ImageName = "icon_PI1_SharedFile_16x16.png",
                LargeImageName = "icon_PI1_SharedFile_32x32.png"
            };

            var btnAssociatingParametersToGlobal = RevitPushButton.Create(btnAssociatingParametersToGlobalData);

            return Result.Succeeded;
        }
    }
}
