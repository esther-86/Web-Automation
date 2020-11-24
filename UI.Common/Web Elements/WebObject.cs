using System.IO;
using System.Text;

namespace UI.Common
{
    public class WebObject
    {
        public WebDriver WebDriver { get; protected set; }
        public int IFrameIndex { get; protected set; }
        public string SimulateScript { get; protected set; }

        public WebObject(WebDriver webDriver, int iFrameIndex)
        {
            this.WebDriver = webDriver;
            this.IFrameIndex = iFrameIndex;
            this.SimulateScript = File.ReadAllText(@".\Javascripts\Simulate.js");
        }

        public string GetMouseSelectJS(string getElementByCommand)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("simulate({0}, 'mouseover', {{shiftKey: true}});", getElementByCommand));
            sb.AppendLine(string.Format("simulate({0}, 'mousedown', {{shiftKey: true}});", getElementByCommand));
            sb.AppendLine(string.Format("simulate({0}, 'mouseup', {{shiftKey: true}});", getElementByCommand));
            return sb.ToString();
        }

        public void MouseSelect(string getElementByCommand)
        {  RunScript(GetMouseSelectJS(getElementByCommand)); }

        public void ShiftSelect(string getElementByCommand)
        {
            // Using the mouse event because sometimes, the code requires mouse event, and not click events
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(GetMouseSelectJS(getElementByCommand));
            sb.AppendLine(string.Format("simulate({0}, 'click', {{shiftKey: true}});", getElementByCommand));
            RunScript(sb.ToString());
        }

        protected void RunScript(string jScript)
        {
            // TODO: Make this a relative path, and try to only register it when needed, to reduce re-registrations
            // From testing, it doesn't seem like removing the re-registration will work.
            // Maybe ask WAS developers to include this function in the WAS build instead?
            this.WebDriver.ExecuteJScript<string>(this.SimulateScript + jScript);
        }
    }
}
