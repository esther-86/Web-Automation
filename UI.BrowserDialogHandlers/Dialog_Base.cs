using System;
using System.Collections.Generic;
using AutoItX3Lib;
using System.Diagnostics;
using White.Core.UIItems.WindowItems;
using White.Core;
using System.Text.RegularExpressions;

namespace UI.BrowserDialogHandlers
{
    /// <summary>
    /// Couldn't get this to click the proper control
    /// autoIt.ControlClick(this.WindowTitle, "Save File", "[CLASS:Radio]");
    /// AutoIt:
    /// |_ PRO: Fast to get a list of all windows with certain titles.
    /// |_ CON: Can only work on one window at a time, and work on the window by the window title
    /// __|____ Hard to click on controls using AutoIt – doesn’t know the syntax. 
    /// __|____ Therefore, using a mix of AutoIt for finding windows, and White for automating the controls on the windows
    /// </summary>
    public abstract class Dialog_Base
    {
        protected static AutoItX3Class autoIt = new AutoItX3Class();

        protected Process Process { get; set; }
        protected Application Application { get; set; }
        
        public Window WorkingWindow { get; protected set; }

        public Dialog_Base(Process process, string partialWindowTitle)
        {
            this.Process = process;
            this.Application = Application.Attach(process);

            Application app = Application.Attach(process);

            List<Window> windows = app.GetWindows();
            foreach (Window window in windows)
            {
                if (!Regex.IsMatch(window.Title, partialWindowTitle, RegexOptions.IgnoreCase))
                { continue; }

                // Found a match
                this.WorkingWindow = window;

                // Dialog exists, thus, try to activate the dialog
                ActivateWindow(this.WorkingWindow.Title);

                // Break because assuming that only one matching window will exist
                // TODO: Determine if the assumption is valid and correct if not
                // Make sure to check for performance if we don't stop early
                return;
            }

            throw new Exception("Dialog doesn't exist");
        }

        public void ActivateWindow(string windowTitle)
        {
            autoIt.AutoItSetOption("WinTitleMatchMode", 2);
            autoIt.WinActivate(windowTitle);
            autoIt.WinWaitActive(windowTitle);
        }
    }
}
