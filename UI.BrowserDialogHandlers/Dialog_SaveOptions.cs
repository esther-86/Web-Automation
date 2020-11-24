using System;
using White.Core.UIItems;
using White.Core.UIItems.Finders;
using System.Diagnostics;
using System.Windows.Automation;
using ExtensionNUnit;

namespace UI.BrowserDialogHandlers
{
    public class MUIItem
    {
        protected UIItem item = null;
        protected Dialog_Base dialog = null;

        public MUIItem(Dialog_Base dialog, UIItem item)
        {
            this.dialog = dialog;
            this.item = item;
        }

        public void Click()
        {
            // TODO: Find a way to do this (activate window) in White
            this.dialog.ActivateWindow(this.dialog.WorkingWindow.Title);
            item.Click();
        }
    }

    public abstract class Dialog_SaveOptions_Base : Dialog_Base
    {
        public MUIItem Button_Open { get; protected set; }
        public MUIItem Button_Save { get; protected set; }
        public MUIItem Button_Cancel { get; protected set; }

        public Dialog_SaveOptions_Base(Process process, string partialWindowTitle)
            : base(process, partialWindowTitle)
        {
            this.Button_Open = new MUIItem(this, this.WorkingWindow.Get<Button>(SearchCriteria.ByText("Open")));
            this.Button_Save = new MUIItem(this, this.WorkingWindow.Get<Button>(SearchCriteria.ByText("Save")));
            this.Button_Cancel = new MUIItem(this, this.WorkingWindow.Get<Button>(SearchCriteria.ByText("Cancel")));
        }

        public virtual void HandleDownload(Options_HandleWindow optionsDownload, string downloadPath)
        {
            switch (optionsDownload)
            {
                case Options_HandleWindow.SAVE:
                    this.Button_Save.Click();
                    break;
                case Options_HandleWindow.CANCEL:
                    this.Button_Cancel.Click();
                    break;
                case Options_HandleWindow.OPEN:
                    this.Button_Open.Click();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public class Dialog_SaveOptions_FireFox : Dialog_SaveOptions_Base
    {
        public Button Button_OK { get; protected set; }

        public Dialog_SaveOptions_FireFox(Process process)
            : base(process, "Opening")
        {
            this.Button_Open = new MUIItem(this, this.WorkingWindow.Get<RadioButton>(SearchCriteria.ByText("Open with")));
            this.Button_Save = new MUIItem(this, this.WorkingWindow.Get<RadioButton>(SearchCriteria.ByText("Save File")));

            this.Button_OK = this.WorkingWindow.Get<Button>(SearchCriteria.ByText("OK"));
            // TODO: Get the checkbox to work. Right now, it's null.
            // this.Automatic = this.Window.Get<CheckBox>(SearchCriteria.ByText("Do this automatically"));
        }

        public override void HandleDownload(Options_HandleWindow optionsDownload, string downloadPath)
        {
            base.HandleDownload(optionsDownload, downloadPath);

            if (optionsDownload == Options_HandleWindow.SAVE || optionsDownload == Options_HandleWindow.OPEN)
            { this.Button_OK.Click(); }

            if (optionsDownload == Options_HandleWindow.SAVE)
            {
                Dialog_SaveAs dialogSave = new Dialog_SaveAs(this.Process, "Enter name of file to save to");
                dialogSave.Save(downloadPath);
            }
        }
    }

    public class Dialog_SaveOptions_Chrome : Dialog_SaveOptions_Base
    {
        public Dialog_SaveOptions_Chrome(Process process)
            : base(process, "Save As") { }

        public override void HandleDownload(Options_HandleWindow optionsDownload, string downloadPath)
        {
            if (optionsDownload == Options_HandleWindow.SAVE)
            {
                Dialog_SaveAs dialogSave = new Dialog_SaveAs(this.Process, "Save As");
                dialogSave.Save(downloadPath);
                return;
            }

            base.HandleDownload(optionsDownload, downloadPath);
        }
    }

    public class Dialog_SaveOptions_IExplore : Dialog_SaveOptions_Base
    {
        public Button Button_Close { get; protected set; }
        public new AutomationElement Button_Save { get; protected set; }
        public AutomationElement Button_Save_Dropdown { get; protected set; }

        // http://support.microsoft.com/kb/2561716
        // http://social.technet.microsoft.com/Forums/en/ieitprocurrentver/thread/21798a87-8c04-4995-a988-ed9275062f82
        // http://social.technet.microsoft.com/Forums/en/ieitprocurrentver/thread/eb1361f8-65ad-471c-814f-99635c10b2d3
        public Dialog_SaveOptions_IExplore(Process process)
            : base(process, "Internet Explorer")
        {
            this.Button_Close = this.WorkingWindow.Get<Button>(SearchCriteria.ByText("Close"));
            // TODO: Try to implement it as a custom control
            // http://white.codeplex.com/wikipage?title=Custom%20UI%20Item&referringTitle=Home
        }

        public override void HandleDownload(Options_HandleWindow optionsDownload, string downloadPath)
        {
            // Override it here, because the save button for IE9 cannot be used
            if (optionsDownload == Options_HandleWindow.SAVE)
            {
                ClickSaveAs(downloadPath);
                // Handled the save already, no need to call the base method
                return;
            }

            base.HandleDownload(optionsDownload, downloadPath);
        }

        // http://www.autoitscript.com/forum/topic/127987-handling-ie9-file-download-activex-prompts/
        public void ClickSaveAs(string downloadPath)
        {
            string winHandle = autoIt.WinGetHandle("[Class:IEFrame]");
            String winTitle = "[HANDLE:" + winHandle + "]";

            //get a handle to the control (IE9 download info bar)
            string ctrlHandle = autoIt.ControlGetHandle(winTitle, "", "[Class:DirectUIHWND]");
            string ctrlTitle = "[HANDLE:" + ctrlHandle + "]";

            //must have this line in here in order to get a handle to the control
            autoIt.WinWaitActive(ctrlTitle, "[CLASS:DirectUIHWND]", 10);

            // Get the x, y coordinates of the control
            int x = autoIt.ControlGetPosWidth(winTitle, "", "[Class:DirectUIHWND]") - 135;  //will differ depending on size of control
            int y = autoIt.ControlGetPosHeight(winTitle, "", "[Class:DirectUIHWND]") - 25; //will differ depending on size of control

            // Save Prompt
            autoIt.ControlFocus(winTitle, "Do you want to open or save", "[CLASS:DirectUIHWND]");
            autoIt.WinActivate(winTitle, "Do you want to open or save");
            autoIt.ControlFocus(winTitle, "Do you want to open or save", "[CLASS:DirectUIHWND]");
            autoIt.ControlClick(winTitle, "", "[Class:DirectUIHWND]", "primary", 1, x, y); // activates the save button
            autoIt.ControlSend(winTitle, "", "[Class:DirectUIHWND]", "{Down}", 0); // down arrow 
            autoIt.ControlSend(winTitle, "", "[Class:DirectUIHWND]", "a", 0); // select "save as"

            // Save as dialog
            autoIt.WinActivate("Save As", "Save");
            autoIt.WinWaitActive("Save As", "Save", 10);
            autoIt.ControlSetText("Save As", "", "Edit1", downloadPath);
            autoIt.ControlClick("Save As", "", "&Save", "left", 1, 5, 5);
            autoIt.ControlClick("Confirm Save As", "", "&Yes", "left", 1, 0, 0);

            // Close the info bar
            autoIt.WinActivate(winTitle, "");
            x = autoIt.ControlGetPosWidth(winTitle, "", "[Class:DirectUIHWND]") - 15;
            y = autoIt.ControlGetPosHeight(winTitle, "", "[Class:DirectUIHWND]") - 23;
            autoIt.WinActivate(ctrlTitle, "");
            autoIt.ControlClick(winTitle, "", "[Class:DirectUIHWND]", "", 1, x, y);
            int success = autoIt.ControlSend(winTitle, "", "[Class:DirectUIHWND]", "{Enter}", 0);
        }
    }
}
