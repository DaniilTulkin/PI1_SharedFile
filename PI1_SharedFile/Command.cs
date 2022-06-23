using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;

namespace PI1_SharedFile
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    // Start command class.
    public class Command : IExternalCommand
    {
        /// <summary>
        /// Overload this method to implement and external command within Revit.
        /// </summary>
        /// <param name="commandData">An ExternalCommandData object which contains reference to Application and View
        /// needed by external command.</param>
        /// <param name="message">Error message can be returned by external command. This will be displayed only if the command status
        /// was "Failed".  There is a limit of 1023 characters for this message; strings longer than this will be truncated.</param>
        /// <param name="elements">Element set indicating problem elements to display in the failure dialog.  This will be used
        /// only if the command status was "Failed".</param>
        /// <returns>
        /// The result indicates if the execution fails, succeeds, or was canceled by user. If it does not
        /// succeed, Revit will undo any changes made by the external command.
        /// </returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            string currentDateTime = DateTime.Now.ToString("yyyy-MM-dd") + "_";
            string userName = "_(user)_" + Environment.UserName;

            string filePath = string.Empty;
            if (doc.IsWorkshared)
            {
                ModelPath centralPath = doc.GetWorksharingCentralModelPath();
                filePath = ModelPathUtils.ConvertModelPathToUserVisiblePath(centralPath);
            }
            else
            {
                filePath = doc.PathName;
            }

            string fileName = Path.GetFileName(filePath);
            string sharedFileName = fileName.Replace("_W0", "_S0");
            string archiveFileName = currentDateTime + sharedFileName.Replace(".rvt", "") + userName + ".rvt";


            string directoryPath = Path.GetDirectoryName(filePath) + @"\";
            string sharedDirectoryPath = directoryPath.Replace("01_WIP", "02_SHARED");
            string archiveDirectoryPath = sharedDirectoryPath + @"ARCHIVE\";

            if (Directory.Exists(sharedDirectoryPath))
            {
                File.Copy(Path.Combine(filePath), Path.Combine(sharedDirectoryPath, sharedFileName), true);
            }
            else
            {
                Directory.CreateDirectory(sharedDirectoryPath);
                File.Copy(Path.Combine(filePath), Path.Combine(sharedDirectoryPath, sharedFileName), true);
            }

            if (Directory.Exists(archiveDirectoryPath))
            {
                File.Copy(Path.Combine(sharedDirectoryPath, sharedFileName), Path.Combine(archiveDirectoryPath, archiveFileName), true);
            }
            else
            {
                Directory.CreateDirectory(archiveDirectoryPath);
                File.Copy(Path.Combine(sharedDirectoryPath, sharedFileName), Path.Combine(archiveDirectoryPath, archiveFileName), true);
            }

            return Result.Succeeded;
        }

        /// <summary>
        /// Gets the path of the current command.
        /// </summary>
        /// <returns></returns>
        public static string GetPath()
        {
            return typeof(Command).Namespace + "." + nameof(Command);
        }
    }
}
