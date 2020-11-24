using System;
using System.Diagnostics;
using ExtensionNUnit;

namespace UI.BrowserDialogHandlers
{
    public class Program
    {
        // Requirement: Install autoIt on the computer, and reference White dlls correctly.
        // Firefox	
        // |_ Settings > General > Downloads section: Select ‘Always ask me where to save files’
        // |_ Unchecked ‘Show the Downloads window…’
        // IE
        // |_ HKEY_CURRENT_USER\Software\Policies\Microsoft\Internet Explorer\Restrictions
        // |_ DWORD AlwaysPromptWhenDownload = 1
        // Chrome
        // |_ http://support.google.com/chrome/bin/answer.py?hl=en&answer=95574&topic=14681&ctx=topic
        // |_ Enable ‘Ask where to save each file…’
        // TODO: Convert all to Auto-It and use wrapper class for them
        static void Main(string[] args)
        {
            Browser browser = (Browser)Enum.Parse(typeof(Browser), args[0]);
            // import or Options_Download
            Options_HandleWindow optionHandleWindow = (Options_HandleWindow)
                            Enum.Parse(typeof(Options_HandleWindow), args[1]);
            string filePath = args[2];

            string browserId = browser.ToString();
            Type importDialogType, downloadDialogType;

            if (browser == Browser.IEXPLORE)
            {
                importDialogType = typeof(Dialog_Import_IExplore);
                downloadDialogType = typeof(Dialog_SaveOptions_IExplore);
            }
            else if (browser == Browser.FIREFOX)
            {
                importDialogType = typeof(Dialog_Import_FireFox);
                downloadDialogType = typeof(Dialog_SaveOptions_FireFox);
            }
            else if (browser == Browser.CHROME)
            {
                importDialogType = typeof(Dialog_Import_Chrome);
                downloadDialogType = typeof(Dialog_SaveOptions_Chrome);
            }
            else { throw new NotImplementedException(); }

            // This will continue to loop until a dialog is shown and the action completes successfully
            // The problem is that we cannot launch the process after the click button 
            // (because the click method doesn't return until the browser dialog is closed)
            // Thus, need to call this exe before calling click. 
            // This exe should wait until there's a file upload with matching criteria, 
            // do work, and continue. 
            // How long to wait for this process to complete should depend on the caller
            bool success = false;
            while (!success)
            {
                // Re-retrieving the process list because a process might have been closed during the time
                Process[] processes = Process.GetProcessesByName(browserId);
                foreach (Process process in processes)
                {
                    try
                    {
                        object[] parameters = { process };
                        if (optionHandleWindow == Options_HandleWindow.IMPORT)
                        {
                            Dialog_Import d = (Dialog_Import)Activator.CreateInstance(importDialogType, parameters);
                            d.Import(filePath);
                            success = true;
                        }
                        else
                        {
                            Dialog_SaveOptions_Base d = (Dialog_SaveOptions_Base)Activator.CreateInstance(downloadDialogType, parameters);
                            d.HandleDownload(optionHandleWindow, filePath);
                            success = true;
                        }
                    }
                    catch (Exception ex)
                    { Console.WriteLine(string.Format("ERROR: {0}\n{1}\n{2}", ex.Message, ex.InnerException, ex.StackTrace)); }
                    // catch { }
                }

                System.Threading.Thread.Sleep(500);
            }
        }
    }
}