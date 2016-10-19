using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows;
using Cloudmaster.WCS.Office.Word;
using Cloudmaster.WCS.Office.Excel;
using Cloudmaster.WCS.Model;

namespace Cloudmaster.WCS.Controls.Forms.Commands
{
    public class OfficeCommands
    {
        #region Export Excel

        public static RoutedUICommand ExportExcelCommand = new RoutedUICommand("Export To Excel", "ExportExcelCommand", typeof(Window));

        public static void ExportExcelCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ExcelFormExporter excelFormExporter = new ExcelFormExporter();

            string localFilename = System.IO.Path.GetTempFileName();

            localFilename = localFilename.Substring(0, localFilename.Length - 3);

            localFilename += "xlsx";

            excelFormExporter.Export(localFilename, FormManager.Instance.SelectedForm, false);

            Process process = new Process();

            process.StartInfo.Arguments = "\"" + localFilename + "\"";
            process.StartInfo.FileName = "EXCEL.EXE";

            process.Start();
        }

        public static void ExportExcelCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.SelectedForm != null);
        }

        #endregion

        #region Export Word

        public static RoutedUICommand ExportWordCommand = new RoutedUICommand("Export To Word", "ExportWordCommand", typeof(Window));

        public static void ExportWordCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            WordFormExporter wordFormExporter = new WordFormExporter();

            string localFilename = System.IO.Path.GetTempFileName();

            localFilename = localFilename.Substring(0, localFilename.Length - 3);

            localFilename += "xlsx";

            wordFormExporter.Export(localFilename, FormManager.Instance.SelectedForm);

            Process process = new Process();

            process.StartInfo.Arguments = "\"" + localFilename + "\"";
            process.StartInfo.FileName = "Winword.EXE";

            process.Start();
        }

        public static void ExportWordCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (FormManager.Instance.SelectedForm != null);
        }

        #endregion
         
    }
}
