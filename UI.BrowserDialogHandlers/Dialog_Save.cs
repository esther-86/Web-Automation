using White.Core.UIItems;
using White.Core.UIItems.Finders;
using System.IO;
using System.Diagnostics;
using ExtensionNUnit;

namespace UI.BrowserDialogHandlers
{
    public class Dialog_SaveAs : Dialog_SaveOptions_Base
    {
        public TextBox FileName { get; protected set; }

        public Dialog_SaveAs(Process process, string partialWindowTitle)
            : base(process, partialWindowTitle)
        {
            this.FileName = this.WorkingWindow.Get<TextBox>(SearchCriteria.ByText("File name:"));
        }

        public void Save(string filePath)
        {
            if (File.Exists(filePath))
            { File.Delete(filePath); }
            this.FileName.SetValue(filePath);
            this.Button_Save.Click();
        }
    }
}
