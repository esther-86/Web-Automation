using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using ExtensionNUnit;

namespace UI.BrowserDialogHandlers
{
    public class Dialog_Import : Dialog_SaveAs
    {
        public Dialog_Import(Process process, string partialWindowTitle)
            : base(process, partialWindowTitle)
        {
        }

        public virtual void Import(string filePath)
        {
            this.FileName.SetValue(filePath);
            this.Button_Open.Click();
        }
    }

    public class Dialog_Import_IExplore : Dialog_Import
    {
        // The file import dialog is not recognized as a window belonging to the ie process.
        // Thus, we needed to override the Import statement.
        public Dialog_Import_IExplore(Process process)
            : base(process, "Internet Explorer")
        {
        }

        // http://stackoverflow.com/questions/10444089/how-to-automate-uploading-a-file-using-webdriver
        public override void Import(string filePath)
        {
            string windowTitle = "Choose File to Upload";
            this.ActivateWindow(windowTitle);
            autoIt.ControlSetText(windowTitle, "", "[CLASS:Edit; INSTANCE:1]", filePath);
            autoIt.ControlClick(windowTitle, "", "[CLASS:Button; INSTANCE:1]");
            autoIt.WinWaitClose(windowTitle, "File &name:", 10);
        }
    }

    public class Dialog_Import_FireFox : Dialog_Import
    {
        public Dialog_Import_FireFox(Process process)
            : base(process, "File Upload") { }
    }

    public class Dialog_Import_Chrome : Dialog_Import
    {
        public Dialog_Import_Chrome(Process process)
            : base(process, "Open") { }
    }
}
