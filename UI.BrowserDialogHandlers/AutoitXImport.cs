using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace AutoItXImport
{
    internal static class AutoItX
    {
        //NOTE: This is based on AutoItX v3.3.0.0 which is a Unicode version
        //NOTE: My comments usually have "jcd" appended where I am uncertain.
        //NOTE: Optional parameters are not supported in C# yet so fill in all fields even if just "".
        //NOTE: Be prepared to play around a bit with which fields need values and what those value are.
        //NOTE: In previous versions we used byte[] to return values like this:
        //byte[] returnclip = new byte[200]; //at least twice as long as the text string expected +2 for null, (Unicode is 2 bytes)
        //AutoItX3Declarations.AU3_ClipGet(returnclip, returnclip.Length);
        //clipdata = System.Text.Encoding.Unicode.GetString(returnclip).TrimEnd('\0');
        //Now we are returning Unicode we can use StringBuilder instead like this:
        //StringBuilder clip = new StringBuilder(); //passing a parameter here will not work, we must asign a length
        //clip.Length = 200; //the number of chars expected plus 1 for the terminating null
        //AutoItX3Declarations.AU3_ClipGet(clip,clip.Length);
        //MessageBox.Show(clip.ToString());
        //NOTE: The big advantage of using AutoItX3 like this is that you don't have to register
        //the dll with windows and more importantly you get away from the many issues involved in
        //publishing the application and the binding to the dll required.
        //The below constants were found by Registering AutoItX3.dll in Windows
        //, adding AutoItX3Lib to References in IDE
        //,declaring an instance of it like this:
        // AutoItX3Lib.AutoItX3 autoit;
        // static AutoItX3Lib.AutoItX3Class autoit;
        //,right clicking on the AutoItX3Class and then Goto Definitions
        //and seeing the equivalent of the below in the MetaData Window.
        //So far it is working
        //NOTE: easier way is to use "DLL Export Viewer" utility and get it to list Properties also
        //"DLL Export Viewer" is from http://www.nirsoft.net
        // Definitions
        public const int AU3_INTDEFAULT = -2147483647; // "Default" value for _some_ int parameters (largest negative number)
        public const int error = 1;
        public const int SW_HIDE = 2;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 4;
        public const int SW_RESTORE = 5;
        public const int SW_SHOW = 6;
        public const int SW_SHOWDEFAULT = 7;
        public const int SW_SHOWMAXIMIZED = 8;
        public const int SW_SHOWMINIMIZED = 9;
        public const int SW_SHOWMINNOACTIVE = 10;
        public const int SW_SHOWNA = 11;
        public const int SW_SHOWNOACTIVATE = 12;
        public const int SW_SHOWNORMAL = 13;
        public const int version = 110; //was 109 if previous non-unicode version
        /////////////////////////////////////////////////////////////////////////////////
        //// Exported functions of AutoItXC.dll
        /////////////////////////////////////////////////////////////////////////////////
        //AU3_API void WINAPI AU3_Init(void);
        //Uncertain if this is needed jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_Init();
        //AU3_API long AU3_error(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_error();
        //AU3_API long WINAPI AU3_AutoItSetOption(LPCWSTR szOption, long nValue);
        /// <summary>
        /// Changes the operation of various AutoIt functions/parameters.
        /// </summary>
        /// <param name="Option">The option to change. See Remarks in help documentation.
        /// CaretCoordMode
        /// MouseClickDelay
        /// MouseClickDownDelay
        /// MouseClickDragDelay
        /// MouseCoordMode
        /// PixelCoordMode
        /// SendAttachMode
        /// SendCapslockMode
        /// SendKeyDelay
        /// SendKeyDownDelay
        /// WinDetectHiddenText
        /// WinSearchChildren
        /// WinTextMatchMode
        /// WinTitleMatchMode
        /// WinWaitDelay
        /// </param>
        /// <param name="Value">The parameter (varies by option). See Remarks in help documentation.</param>
        /// <returns></returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_AutoItSetOption([MarshalAs(UnmanagedType.LPWStr)] string Option, int Value);
        //AU3_API void WINAPI AU3_BlockInput(long nFlag);
        /// <summary>
        /// Disable/enable the mouse and keyboard.
        /// </summary>
        /// <param name="Flag">1 = Disable user input
        /// 0 = Enable user input</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_BlockInput(int Flag);
        //AU3_API long WINAPI AU3_CDTray(LPCWSTR szDrive, LPCWSTR szAction);
        /// <summary>
        /// Opens or closes the CD tray.
        /// </summary>
        /// <param name="Drive">The drive letter of the CD tray to control, in the format D:, E:, etc.</param>
        /// <param name="Action">Specifies if you want the CD tray to be open or closed: "open" or "closed"</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if drive is locked via CD burning software or if the drive letter is not a CD drive. </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_CDTray([MarshalAs(UnmanagedType.LPWStr)] string Drive
        , [MarshalAs(UnmanagedType.LPWStr)] string Action);
        //AU3_API void WINAPI AU3_ClipGet(LPWSTR szClip, int nBufSize);
        //Use like this:
        //StringBuilder clip = new StringBuilder();
        //clip.Length = 4;
        //AutoItX3Declarations.AU3_ClipGet(clip,clip.Length);
        //MessageBox.Show(clip.ToString());
        /// <summary>
        /// Get text from the clipboard into a string.
        /// Use like this:
        ///StringBuilder clip = new StringBuilder();
        ///clip.Length = 4;
        ///AutoItX3Declarations.AU3_ClipGet(clip,clip.Length);
        ///MessageBox.Show(clip.ToString());
        /// </summary>
        /// <param name="Clip">Retrieves text from the clipboard.</param>
        /// <param name="BufSize">Maximum length of string provided</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ClipGet([MarshalAs(UnmanagedType.LPWStr)]StringBuilder Clip, int BufSize);
        //AU3_API void WINAPI AU3_ClipPut(LPCWSTR szClip);
        /// <summary>
        /// Writes text to the clipboard.
        /// </summary>
        /// <param name="Clip">The text to write to the clipboard.</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ClipPut([MarshalAs(UnmanagedType.LPWStr)] string Clip);
        //AU3_API long WINAPI AU3_ControlClick(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szButton, long nNumClicks, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nX
        //, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nY);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlClick([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Button, int NumClicks, int X, int Y);
        //AU3_API void WINAPI AU3_ControlCommand(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szCommand, LPCWSTR szExtra, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlCommand([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Command, [MarshalAs(UnmanagedType.LPWStr)] string Extra
        , [MarshalAs(UnmanagedType.LPWStr)] StringBuilder Result, int BufSize);
        //AU3_API void WINAPI AU3_ControlListView(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szCommand, LPCWSTR szExtra1, LPCWSTR szExtra2, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlListView([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Command, [MarshalAs(UnmanagedType.LPWStr)] string Extral1
        , [MarshalAs(UnmanagedType.LPWStr)] string Extra2, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder Result
        , int BufSize);
        //AU3_API long WINAPI AU3_ControlDisable(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlDisable([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API long WINAPI AU3_ControlEnable(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlEnable([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API long WINAPI AU3_ControlFocus(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        /// <summary>
        /// Sets input focus to a given control on a window.
        /// </summary>
        /// <param name="Title">The title of the window to access..  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">The text of the window to access.</param>
        /// <param name="Control">The control to interact with. See Controls.</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0.
        /// </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlFocus([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API void WINAPI AU3_ControlGetFocus(LPCWSTR szTitle, LPCWSTR szText, LPWSTR szControlWithFocus
        //, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlGetFocus([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder ControlWithFocus
        , int BufSize);
        //AU3_API void WINAPI AU3_ControlGetHandle(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPCWSTR szControl, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlGetHandle([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] StringBuilder RetText, int BufSize);
        //AU3_API long WINAPI AU3_ControlGetPosX(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosX([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API long WINAPI AU3_ControlGetPosY(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosY([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API long WINAPI AU3_ControlGetPosHeight(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosHeight([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API long WINAPI AU3_ControlGetPosWidth(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosWidth([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API void WINAPI AU3_ControlGetText(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPWSTR szControlText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlGetText([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)]StringBuilder ControlText, int BufSize);
        //AU3_API long WINAPI AU3_ControlHide(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlHide([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API long WINAPI AU3_ControlMove(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, long nX, long nY, /*[in,defaultvalue(-1)]*/long nWidth, /*[in,defaultvalue(-1)]*/long nHeight);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlMove([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , int X, int Y, int Width, int Height);
        //AU3_API long WINAPI AU3_ControlSend(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szSendText, /*[in,defaultvalue(0)]*/long nMode);
        /// <summary>
        ///
        /// </summary>
        /// <param name="Title">The title of the window to access..  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">The text of the window to access.</param>
        /// <param name="Control">The control to interact with. See Controls.</param>
        /// <param name="SendText">String of characters to send to the control.</param>
        /// <param name="Mode">flag = 0 (default), Text contains special characters like + to indicate SHIFT and {LEFT} to indicate left arrow.
        /// flag = 1, keys are sent raw.</param>
        /// <returns>Success: Returns 1. Fail returns 0</returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlSend([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string SendText, int Mode);
        //AU3_API long WINAPI AU3_ControlSetText(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szControlText);
        /// <summary>
        /// Sends a string of characters to a control.
        /// </summary>
        /// <param name="Title">The title of the window to access..  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">The text of the window to access.</param>
        /// <param name="Control">The control to interact with. See Controls.</param>
        /// <param name="ControlText">The new text to be set into the control.</param>
        /// <returns></returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlSetText([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string ControlText);
        //AU3_API long WINAPI AU3_ControlShow(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlShow([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);
        //AU3_API void WINAPI AU3_ControlTreeView(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szCommand, LPCWSTR szExtra1, LPCWSTR szExtra2, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlTreeView([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Command, [MarshalAs(UnmanagedType.LPWStr)] string Extra1
        , [MarshalAs(UnmanagedType.LPWStr)] string Extra2, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BufSize);
        //AU3_API void WINAPI AU3_DriveMapAdd(LPCWSTR szDevice, LPCWSTR szShare, long nFlags
        //, /*[in,defaultvalue("")]*/LPCWSTR szUser, /*[in,defaultvalue("")]*/LPCWSTR szPwd
        //, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_DriveMapAdd([MarshalAs(UnmanagedType.LPWStr)] string Device
        , [MarshalAs(UnmanagedType.LPWStr)] string Share, int Flags, [MarshalAs(UnmanagedType.LPWStr)] string User
        , [MarshalAs(UnmanagedType.LPWStr)] string Pwd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BufSize);
        //AU3_API long WINAPI AU3_DriveMapDel(LPCWSTR szDevice);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_DriveMapDel([MarshalAs(UnmanagedType.LPWStr)] string Device);
        //AU3_API void WINAPI AU3_DriveMapGet(LPCWSTR szDevice, LPWSTR szMapping, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_DriveMapGet([MarshalAs(UnmanagedType.LPWStr)] string Device
        , [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Mapping, int BufSize);
        //AU3_API long WINAPI AU3_IniDelete(LPCWSTR szFilename, LPCWSTR szSection, LPCWSTR szKey);
        /// <summary>
        /// Deletes a value from a standard format .ini file.
        /// </summary>
        /// <param name="Filename">The filename of the .ini file.</param>
        /// <param name="Section">The section name in the .ini file.</param>
        /// <param name="Key">Optional: The key name in the in the .ini file. If no key name is given the entire section is deleted.</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if section/key is not found or if INI file is read-only.
        /// </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_IniDelete([MarshalAs(UnmanagedType.LPWStr)] string Filename
        , [MarshalAs(UnmanagedType.LPWStr)] string Section, [MarshalAs(UnmanagedType.LPWStr)] string Key);
        //AU3_API void WINAPI AU3_IniRead(LPCWSTR szFilename, LPCWSTR szSection, LPCWSTR szKey
        //, LPCWSTR szDefault, LPWSTR szValue, int nBufSize);
        /// <summary>
        /// Reads a value from a standard format .ini file.  (See ClipGet for example of StringBuilder use)
        /// </summary>
        /// <param name="Filename">The filename of the .ini file.</param>
        /// <param name="Section">The section name in the .ini file.</param>
        /// <param name="Key">The key name in the in the .ini file.</param>
        /// <param name="Default">The default value to return if the requested key is not found.</param>
        /// <param name="Value">The value returned as a StringBuilder</param>
        /// <param name="BufSize">The size of the string</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_IniRead([MarshalAs(UnmanagedType.LPWStr)] string Filename
        , [MarshalAs(UnmanagedType.LPWStr)] string Section, [MarshalAs(UnmanagedType.LPWStr)] string Key
        , [MarshalAs(UnmanagedType.LPWStr)] string Default, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Value, int BufSize);
        //AU3_API long WINAPI AU3_IniWrite(LPCWSTR szFilename, LPCWSTR szSection, LPCWSTR szKey
        //, LPCWSTR szValue);
        /// <summary>
        /// Writes a value to a standard format .ini file.
        /// </summary>
        /// <param name="Filename">The filename of the .ini file.</param>
        /// <param name="Section">The section name in the .ini file.</param>
        /// <param name="Key">The key name in the in the .ini file.</param>
        /// <param name="Value">The value to write/change.</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if file is read-only.
        /// </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_IniWrite([MarshalAs(UnmanagedType.LPWStr)] string Filename
        , [MarshalAs(UnmanagedType.LPWStr)] string Section, [MarshalAs(UnmanagedType.LPWStr)] string Key
        , [MarshalAs(UnmanagedType.LPWStr)] string Value);
        //AU3_API long WINAPI AU3_IsAdmin(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_IsAdmin();
        //AU3_API long WINAPI AU3_MouseClick(/*[in,defaultvalue("LEFT")]*/LPCWSTR szButton
        //, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nX, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nY
        //, /*[in,defaultvalue(1)]*/long nClicks, /*[in,defaultvalue(-1)]*/long nSpeed);
        /// <summary>
        /// Perform a mouse click operation.
        /// </summary>
        /// <param name="Button">The button to click: "left", "right", "middle", "main", "menu", "primary", "secondary".</param>
        /// <param name="x">(Present position Can be gotten from AU3_MouseGetPosX()) The x/y coordinates to move the mouse to. If no x and y coords are given, the current position is used.</param>
        /// <param name="y">(Present position Can be gotten from AU3_MouseGetPosY()) The x/y coordinates to move the mouse to. If no x and y coords are given, the current position is used.</param>
        /// <param name="clicks">Optional: The number of times to click the mouse. Default is 1.</param>
        /// <param name="speed">Optional: the speed to move the mouse in the range 1 (fastest) to 100 (slowest). A speed of 0 will move the mouse instantly. Default speed is 10.</param>
        /// <returns>No return value</returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseClick([MarshalAs(UnmanagedType.LPWStr)] string Button, int x, int y
        , int clicks, int speed);
        //AU3_API long WINAPI AU3_MouseClickDrag(LPCWSTR szButton, long nX1, long nY1, long nX2, long nY2
        //, /*[in,defaultvalue(-1)]*/long nSpeed);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseClickDrag([MarshalAs(UnmanagedType.LPWStr)] string Button
        , int X1, int Y1, int X2, int Y2, int Speed);
        //AU3_API void WINAPI AU3_MouseDown(/*[in,defaultvalue("LEFT")]*/LPCWSTR szButton);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_MouseDown([MarshalAs(UnmanagedType.LPWStr)] string Button);
        //AU3_API long WINAPI AU3_MouseGetCursor(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseGetCursor();
        //AU3_API long WINAPI AU3_MouseGetPosX(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseGetPosX();
        //AU3_API long WINAPI AU3_MouseGetPosY(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseGetPosY();
        //AU3_API long WINAPI AU3_MouseMove(long nX, long nY, /*[in,defaultvalue(-1)]*/long nSpeed);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseMove(int X, int Y, int Speed);
        //AU3_API void WINAPI AU3_MouseUp(/*[in,defaultvalue("LEFT")]*/LPCWSTR szButton);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_MouseUp([MarshalAs(UnmanagedType.LPWStr)] string Button);
        //AU3_API void WINAPI AU3_MouseWheel(LPCWSTR szDirection, long nClicks);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_MouseWheel([MarshalAs(UnmanagedType.LPWStr)] string Direction, int Clicks);
        //AU3_API long WINAPI AU3_Opt(LPCWSTR szOption, long nValue);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_Opt([MarshalAs(UnmanagedType.LPWStr)] string Option, int Value);
        //AU3_API unsigned long WINAPI AU3_PixelChecksum(long nLeft, long nTop, long nRight, long nBottom
        //, /*[in,defaultvalue(1)]*/long nStep);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_PixelChecksum(int Left, int Top, int Right, int Bottom, int Step);
        //AU3_API long WINAPI AU3_PixelGetColor(long nX, long nY);
        /// <summary>
        /// Returns a pixel color according to x,y pixel coordinates.
        /// </summary>
        /// <param name="X">x coordinate of pixel.</param>
        /// <param name="Y">y coordinate of pixel.</param>
        /// <returns>Success: Returns decimal value of pixel's color.
        ///Failure: Returns -1 if invalid coordinates.
        ///</returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_PixelGetColor(int X, int Y);
        //AU3_API void WINAPI AU3_PixelSearch(long nLeft, long nTop, long nRight, long nBottom, long nCol
        //, /*default 0*/long nVar, /*default 1*/long nStep, LPPOINT pPointResult);
        //Use like this:
        //int[] result = {0,0};
        //try
        //{
        // AutoItX3Declarations.AU3_PixelSearch(0, 0, 800, 000,0xFFFFFF, 0, 1, result);
        //}
        //catch { }
        //It will crash if the color is not found, have not been able to determin why jcd
        //The AutoItX3Lib.AutoItX3Class version has similar problems and is the only function to return an object
        //so contortions are needed to get the data from it ie:
        //int[] result = {0,0};
        //object resultObj;
        //AutoItX3Lib.AutoItX3Class autoit = new AutoItX3Lib.AutoItX3Class();
        //resultObj = autoit.PixelSearch(0, 0, 800, 600, 0xFFFF00,0,1);
        //Type t = resultObj.GetType();
        //if(t == typeof(object[]))
        //{
        //object[] obj = (object[])resultObj;
        //result[0] = (int)obj[0];
        //result[1] = (int)obj[1];
        //}
        //When it fails it returns an object = 1 but when it succeeds it is object[X,Y]
        /// <summary>
        /// Searches a rectangle of pixels for the pixel color provided.  It will crash if the color is not found.  (See AutoItX3Declarations.cs for details)
        /// </summary>
        /// <param name="Left">left coordinate of rectangle.</param>
        /// <param name="Top">top coordinate of rectangle.</param>
        /// <param name="Right">right coordinate of rectangle.</param>
        /// <param name="Bottom">bottom coordinate of rectangle.</param>
        /// <param name="Color">Colour value of pixel to find (in decimal or hex).</param>
        /// <param name="Shade">A number between 0 and 255 to indicate the allowed number of shades of variation of the red, green, and blue components of the colour. Default is 0 (exact match).</param>
        /// <param name="Step">Instead of searching each pixel use a value larger than 1 to skip pixels (for speed). E.g. A value of 2 will only check every other pixel. Default is 1.</param>
        /// <param name="PointResult">Success: Returns a 2 element array containing the pixel's coordinates
        /// Failure: Sets oAutoIt.error to 1 if color is not found. (apparently this is actually causing an exception)
        ///</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_PixelSearch(int Left, int Top, int Right, int Bottom, int Color, int Shade
        , int Step, int[] PointResult);
        //AU3_API long WINAPI AU3_ProcessClose(LPCWSTR szProcess);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessClose([MarshalAs(UnmanagedType.LPWStr)]string Process);
        //AU3_API long WINAPI AU3_ProcessExists(LPCWSTR szProcess);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessExists([MarshalAs(UnmanagedType.LPWStr)]string Process);
        //AU3_API long WINAPI AU3_ProcessSetPriority(LPCWSTR szProcess, long nPriority);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessSetPriority([MarshalAs(UnmanagedType.LPWStr)]string Process, int Priority);
        //AU3_API long WINAPI AU3_ProcessWait(LPCWSTR szProcess, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessWait([MarshalAs(UnmanagedType.LPWStr)]string Process, int Timeout);
        //AU3_API long WINAPI AU3_ProcessWaitClose(LPCWSTR szProcess, /*[in,defaultvalue(0)]*/long nTimeout);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessWaitClose([MarshalAs(UnmanagedType.LPWStr)]string Process, int Timeout);
        //AU3_API long WINAPI AU3_RegDeleteKey(LPCWSTR szKeyname);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RegDeleteKey([MarshalAs(UnmanagedType.LPWStr)]string Keyname);
        //AU3_API long WINAPI AU3_RegDeleteVal(LPCWSTR szKeyname, LPCWSTR szValuename);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RegDeleteVal([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , [MarshalAs(UnmanagedType.LPWStr)]string ValueName);
        //AU3_API void WINAPI AU3_RegEnumKey(LPCWSTR szKeyname, long nInstance, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_RegEnumKey([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , int Instance, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BusSize);
        //AU3_API void WINAPI AU3_RegEnumVal(LPCWSTR szKeyname, long nInstance, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_RegEnumVal([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , int Instance, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BufSize);
        //AU3_API void WINAPI AU3_RegRead(LPCWSTR szKeyname, LPCWSTR szValuename, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_RegRead([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , [MarshalAs(UnmanagedType.LPWStr)]string Valuename, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);
        //AU3_API long WINAPI AU3_RegWrite(LPCWSTR szKeyname, LPCWSTR szValuename, LPCWSTR szType
        //, LPCWSTR szValue);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RegWrite([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , [MarshalAs(UnmanagedType.LPWStr)]string Valuename, [MarshalAs(UnmanagedType.LPWStr)]string Type
        , [MarshalAs(UnmanagedType.LPWStr)]string Value);
        //AU3_API long WINAPI AU3_Run(LPCWSTR szRun, /*[in,defaultvalue("")]*/LPCWSTR szDir
        //, /*[in,defaultvalue(1)]*/long nShowFlags);
        /// <summary>
        /// Runs an external program.
        /// </summary>
        /// <param name="Run">The name of the executable (EXE, BAT, COM, or PIF) to run.</param>
        /// <param name="Dir">Optional: The working directory.</param>
        /// <param name="ShowFlags">Optional: The "show" flag of the executed program:
        ///SW_HIDE = Hidden window
        ///SW_MINIMIZE = Minimized window
        ///SW_MAXIMIZE = Maximized window</param>
        /// <returns>Success: The PID of the process that was launched., Failure: 1</returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_Run([MarshalAs(UnmanagedType.LPWStr)]string Run
        , [MarshalAs(UnmanagedType.LPWStr)]string Dir, int ShowFlags);
        //AU3_API long WINAPI AU3_RunAsSet(LPCWSTR szUser, LPCWSTR szDomain, LPCWSTR szPassword, int nOptions);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RunAsSet([MarshalAs(UnmanagedType.LPWStr)]string User
        , [MarshalAs(UnmanagedType.LPWStr)]string Domain, [MarshalAs(UnmanagedType.LPWStr)]string Password
        , int Options);
        //AU3_API long WINAPI AU3_RunWait(LPCWSTR szRun, /*[in,defaultvalue("")]*/LPCWSTR szDir
        //, /*[in,defaultvalue(1)]*/long nShowFlags);
        /// <summary>
        /// Runs an external program and pauses script execution until the program finishes.
        /// </summary>
        /// <param name="Run">The name of the executable (EXE, BAT, COM, or PIF) to run.</param>
        /// <param name="Dir">Optional: The working directory.</param>
        /// <param name="ShowFlags">Optional: The "show" flag of the executed program:
        ///SW_HIDE = Hidden window
        ///SW_MINIMIZE = Minimized window
        ///SW_MAXIMIZE = Maximized window</param>
        /// <returns>Success: Returns the exit code of the program that was run., Failure: 1</returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RunWait([MarshalAs(UnmanagedType.LPWStr)]string Run
        , [MarshalAs(UnmanagedType.LPWStr)]string Dir, int ShowFlags);
        //AU3_API void WINAPI AU3_Send(LPCWSTR szSendText, /*[in,defaultvalue("")]*/long nMode);
        /// <summary>
        /// Sends simulated keystrokes to the active window.
        /// </summary>
        /// <param name="SendText">The sequence of keys to send.</param>
        /// <param name="Mode">0 (default), Text contains special characters like + and ! to indicate SHIFT and ALT key presses.
        /// flag = 1, keys are sent raw.</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_Send([MarshalAs(UnmanagedType.LPWStr)] string SendText, int Mode);
        //AU3_API void WINAPI AU3_SendA(LPCSTR szSendText, /*[in,defaultvalue("")]*/long nMode);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_SendA([MarshalAs(UnmanagedType.LPStr)] string SendText, int Mode);
        //AU3_API long WINAPI AU3_Shutdown(long nFlags);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_Shutdown(int Flags);
        //AU3_API void WINAPI AU3_Sleep(long nMilliseconds);
        /// <summary>
        /// Pause script execution.
        /// </summary>
        /// <param name="Milliseconds">Amount of time to pause (in milliseconds).</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_Sleep(int Milliseconds);
        //AU3_API void WINAPI AU3_StatusbarGetText(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(1)]*/long nPart, LPWSTR szStatusText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_StatusbarGetText([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text, int Part, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder StatusText, int BufSize);
        //AU3_API void WINAPI AU3_ToolTip(LPCWSTR szTip, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nX
        //, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nY);
        /// <summary>
        /// Creates a tooltip anywhere on the screen.
        /// </summary>
        /// <param name="Tip">The text of the tooltip. (An empty string clears a displaying tooltip)</param>
        /// <param name="X">The x,y position of the tooltip.</param>
        /// <param name="Y">The x,y position of the tooltip.</param>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ToolTip([MarshalAs(UnmanagedType.LPWStr)]string Tip, int X, int Y);
        //AU3_API void WINAPI AU3_WinActivate(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        /// <summary>
        /// Activates (gives focus to) a window.  Note you can use WinActive to check if WinActivate succeeded.
        /// </summary>
        /// <param name="Title">The title of the window to activate.   (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to activate.</param>
        /// <returns>No return value</returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinActivate([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);
        //AU3_API long WINAPI AU3_WinActive(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        /// <summary>
        /// Checks to see if a specified window exists and is currently active.
        /// </summary>
        /// <param name="Title">The title of the window to activate.   (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to activate.</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if window is not active. </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinActive([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);
        //AU3_API long WINAPI AU3_WinClose(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        /// <summary>
        /// Closes a window.
        /// </summary>
        /// <param name="Title">The title of the window to close.  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to close.</param>
        /// <returns></returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinClose([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);
        //AU3_API long WINAPI AU3_WinExists(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        /// <summary>
        /// Checks to see if a specified window exists.
        /// </summary>
        /// <param name="Title">The title of the window to check..  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to check.</param>
        /// <returns></returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinExists([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);
        //AU3_API long WINAPI AU3_WinGetCaretPosX(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetCaretPosX();
        //AU3_API long WINAPI AU3_WinGetCaretPosY(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetCaretPosY();
        //AU3_API void WINAPI AU3_WinGetClassList(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetClassList([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);
        //AU3_API long WINAPI AU3_WinGetClientSizeHeight(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetClientSizeHeight([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);
        //AU3_API long WINAPI AU3_WinGetClientSizeWidth(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetClientSizeWidth([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);
        //AU3_API void WINAPI AU3_WinGetHandle(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetHandle([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);
        //AU3_API long WINAPI AU3_WinGetPosX(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosX([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);
        //AU3_API long WINAPI AU3_WinGetPosY(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosY([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);
        //AU3_API long WINAPI AU3_WinGetPosHeight(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosHeight([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);
        //AU3_API long WINAPI AU3_WinGetPosWidth(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosWidth([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);
        //AU3_API void WINAPI AU3_WinGetProcess(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetProcess([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);
        //AU3_API long WINAPI AU3_WinGetState(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetState([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);
        //AU3_API void WINAPI AU3_WinGetText(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetText([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);
        //AU3_API void WINAPI AU3_WinGetTitle(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetTitle([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);
        //AU3_API long WINAPI AU3_WinKill(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinKill([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);
        //AU3_API long WINAPI AU3_WinMenuSelectItem(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPCWSTR szItem1, LPCWSTR szItem2, LPCWSTR szItem3, LPCWSTR szItem4, LPCWSTR szItem5, LPCWSTR szItem6
        //, LPCWSTR szItem7, LPCWSTR szItem8);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinMenuSelectItem([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Item1
        , [MarshalAs(UnmanagedType.LPWStr)] string Item2, [MarshalAs(UnmanagedType.LPWStr)] string Item3
        , [MarshalAs(UnmanagedType.LPWStr)] string Item4, [MarshalAs(UnmanagedType.LPWStr)] string Item5
        , [MarshalAs(UnmanagedType.LPWStr)] string Item6, [MarshalAs(UnmanagedType.LPWStr)] string Item7
        , [MarshalAs(UnmanagedType.LPWStr)] string Item8);
        //AU3_API void WINAPI AU3_WinMinimizeAll();
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinMinimizeAll();
        //AU3_API void WINAPI AU3_WinMinimizeAllUndo();
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinMinimizeAllUndo();
        //AU3_API long WINAPI AU3_WinMove(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, long nX, long nY, /*[in,defaultvalue(-1)]*/long nWidth, /*[in,defaultvalue(-1)]*/long nHeight);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinMove([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int X, int Y, int Width, int Height);
        //AU3_API long WINAPI AU3_WinSetOnTop(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText, long nFlag);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetOnTop([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Flags);
        //AU3_API long WINAPI AU3_WinSetState(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText, long nFlags);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetState([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Flags);
        //AU3_API long WINAPI AU3_WinSetTitle(LPCWSTR szTitle,/*[in,defaultvalue("")]*/ LPCWSTR szText
        //, LPCWSTR szNewTitle);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetTitle([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string NewTitle);
        //AU3_API long WINAPI AU3_WinSetTrans(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText, long nTrans);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetTrans([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Trans);
        //AU3_API long WINAPI AU3_WinWait(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        /// <summary>
        /// Pauses execution of the script until the requested window exists.
        /// </summary>
        /// <param name="Title">The title of the window to check.  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to check.</param>
        /// <param name="Timeout">Optional: Timeout in seconds</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if timeout occurred.
        /// </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWait([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);
        //AU3_API long WINAPI AU3_WinWaitA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);
        //AU3_API long WINAPI AU3_WinWaitActive(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        /// <summary>
        /// Pauses execution of the script until the requested window is active.
        /// </summary>
        /// <param name="Title">The title of the window to check.  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to check.</param>
        /// <param name="Timeout">Optional: Timeout in seconds</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if timeout occurred.
        /// </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitActive([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);
        //AU3_API long WINAPI AU3_WinWaitActiveA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitActiveA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);
        //AU3_API long WINAPI AU3_WinWaitClose(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        /// <summary>
        /// Pauses execution of the script until the requested window does not exist.
        /// </summary>
        /// <param name="Title">The title of the window to check.  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to check.</param>
        /// <param name="Timeout">Optional: Timeout in seconds</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if timeout occurred.
        /// </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitClose([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);
        //AU3_API long WINAPI AU3_WinWaitCloseA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitCloseA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);
        //AU3_API long WINAPI AU3_WinWaitNotActive(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        /// <summary>
        /// Pauses execution of the script until the requested window is not active.
        /// </summary>
        /// <param name="Title">The title of the window to check.  (See WinTitleMatchMode under AutoItSetOption)</param>
        /// <param name="Text">Optional: The text of the window to check.</param>
        /// <param name="Timeout">Optional: Timeout in seconds</param>
        /// <returns>Success: Returns 1.
        /// Failure: Returns 0 if timeout occurred.
        /// </returns>
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitNotActive([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);
        //AU3_API long WINAPI AU3_WinWaitNotActiveA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitNotActiveA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);

    }
}