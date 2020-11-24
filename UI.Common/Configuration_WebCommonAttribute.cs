using Common.Utilities;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace UI.Common
{
    [Flags]
    public enum Browser : short
    {
        IEXPLORE = 1,
        FIREFOX = 2,
        CHROME = 4,
        ALL = IEXPLORE | FIREFOX | CHROME
    }

    [Flags]
    public enum WindowType : short
    {
        Same = 1,
        New = 2,
        Incognito = 4,
        NewTab = 8
    }

    public enum Options_HandleWindow
    { IMPORT, OPEN, SAVE, CANCEL }

    [XmlRootAttribute("Gotos")]
    public class GotoCollection : CustomSerialization
    {
        [XmlElement("Goto")]
        public Goto[] Gotos { get; set; }
    }

    public class Goto : CustomSerialization
    {
        [XmlAttribute(AttributeName = "Url")]
        public string Url { get; set; }

        [XmlAttribute(AttributeName = "Browser")]
        public Browser Browser { get; set; }

        [XmlArray("WebActions")]
        [XmlArrayItem("TypeText", Type=typeof(TypeText))]
        [XmlArrayItem("ClickPageResultsAndBackBtn", Type = typeof(ClickPageResultsAndBackBtn))]
        public WebAction[] WebActions { get; set; }

        // Default values
        public Goto()
        {
            Browser = Browser.CHROME;
        }

        public override string ToString()
        {
            string xmlActions = "";
            foreach (WebAction webAction in WebActions)
                xmlActions = xmlActions.AddValueToString(",", webAction.ToString());
            return string.Format("[Url: {0}] [XmlActions: {1}]", Url, xmlActions);
        }

        public void Run(WebDriver webDriver)
        {
            // URL has to start with proper protocol. Else, it will not work for FireFox
            // https://groups.google.com/forum/?fromgroups#!topic/webdriver/6JmrwY2hxwg
            if (!string.IsNullOrEmpty(Url))
                webDriver.Navigate().GoToUrl(Url);
        }
    }
}
