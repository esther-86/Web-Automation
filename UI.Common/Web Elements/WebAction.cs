using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using UI.Common;
using System.Linq;
using Common.Utilities;

/// <summary>
/// Some pre-defined actions... inheritable and reuseable so that new actions can be built from this
///     As long as they inherit WebAction and they can be deserialized, they can be used to customize their own interaction with the web driver
/// </summary>
namespace UI.Common
{
    public abstract class WebAction : CustomSerialization
    {
        public abstract void Run(WebDriver driver);
    }

    public class TypeText : WebAction
    {
        [XmlAttribute(AttributeName = "TypeIntoElementAt")]
        public string TypeIntoElementAt { get; set; }

        [XmlAttribute(AttributeName = "TextValue")]
        public string TextValue { get; set; }

        [XmlAttribute(AttributeName = "ShouldSubmit")]
        public bool ShouldSubmit { get; set; }

        public override void Run(WebDriver driver)
        {
            IWebElement element = driver.FindElement(TypeIntoElementAt); 
            element.SendKeys(TextValue);
            if (ShouldSubmit)
            {
                element.Submit();
                // Google's search is rendered dynamically with JavaScript.
                // Wait for the page to load, timeout after 5 seconds
                driver.WaitUntilTimeout(() => Regex.IsMatch(driver.Title, "^" + TextValue, RegexOptions.IgnoreCase));
            }
        }
    }

    public class SpecialClickData
    {
        public SpecialClick SpecialClick { get; private set; }
        public string WindowHandle { get; set; }
        public DateTime WindowCreatedTime { get; set; }
        public string WindowTitle { get; set; }

        public SpecialClickData(SpecialClick sc)
        {
            SpecialClick = sc;
        }
    }

    public class ClickPageResultsAndBackBtn : SpecialClick
    {
#if FULL_VERSION
        [XmlAttribute(AttributeName = "Pages")]
#endif
        public int Pages { get; set; }

        [XmlAttribute(AttributeName = "ClickOnElementAt")]
        public string ClickOnElementAt { get; set; }

        [XmlAttribute(AttributeName = "ClickNextAt")]
        public string ClickNextAt { get; set; }

        /// <summary>
        /// Contains a list of special links that we shouldn't click and back, but should click, then 
        /// </summary>
#if FULL_VERSION
        [XmlElement("SpecialClick")]
#endif
        public SpecialClick[] SpecialClicks { get; set; }

        // Default values
        public ClickPageResultsAndBackBtn()
        {
            Pages = 1;
            ClickOnElementAt = "div.yuRUbf > a";
            SpecialClicks = new SpecialClick[] { };
        }

        public override void DoPostProcessing()
        {
            base.DoPostProcessing();
            // It need to do post processing for the child elements also
            foreach (SpecialClick sc in SpecialClicks)
                sc.DoPostProcessing();
        }

        ReadOnlyCollection<IWebElement> WaitForTimeoutThenBackBtn(WebDriver driver, int waitMs, string currentTitle)
        {
            WebDriver.WaitUntilTimeout(null, TimeSpan.FromMilliseconds(waitMs));
            driver.Navigate().Back();
            driver.WaitUntilTimeout(() => Regex.IsMatch(driver.Title, currentTitle, RegexOptions.IgnoreCase));
            // When click back, all of the previous links are stale... need to re-get the list of links
            //  Back shouldn't re-run the search. 
            //  If it does, previously saved link information will no longer be valid
            //  In that case, will need to create a new algorithm: TODO
            return driver.FindElements(ClickOnElementAt);
        }

        Dictionary<int, SpecialClickData> ClickResults(WebDriver driver)
        {
            string currentTitle = driver.Title;
            string currentWindowHandle = driver.CurrentWindowHandle;

            // http://stackoverflow.com/questions/24120263/find-element-in-selenium-using-xpath-or-css-selector
            // <div class="rc" data-hveid="128"><h3 class="r"><a href="http://schoolofdents.com/" 
            ReadOnlyCollection<IWebElement> resultLinks = driver.FindElements(ClickOnElementAt);
            Dictionary<int, SpecialClickData> dict_resultId_specialClick = new Dictionary<int, SpecialClickData>();

            int resultId = 0;
            foreach (IWebElement resultLink in resultLinks)
            {
                string href = resultLink.GetAttribute("href");

                // TODO: Determine if the case where there are more than one special click matches - if it's valid, and should be handled
                foreach (SpecialClick specialClick in SpecialClicks)
                {
                    // If user provided a regex and it matches, or the provided Url is a part of it, save the special click info
                    if (Regex.IsMatch(href, specialClick.Url) || href.Contains(specialClick.Url))
                        dict_resultId_specialClick.Add(resultId, new SpecialClickData(specialClick));
                }
                resultId++;
            }

            // Go through all of the special clicks first (so that if they open a new window)...
            //      they open it all at once and the last one will be the one used for the regular link and back clicking
            foreach (KeyValuePair<int, SpecialClickData> kv in dict_resultId_specialClick)
            {
                IWebElement resultLink = resultLinks[kv.Key];
                SpecialClickData specialClickData = kv.Value;
                SpecialClick specialClick = specialClickData.SpecialClick;

                specialClick.Run(driver, resultLink);
                specialClickData.WindowHandle = driver.WindowHandles.Last();
                specialClickData.WindowCreatedTime = DateTime.Now;
                specialClickData.WindowTitle = driver.Title;

                // If the special click uses the same window... there might be special wait condition
                //      wait for the specified amount of time, then click back button, and refresh the resultLinks
                if (specialClick.Window == WindowType.Same)
                    resultLinks = WaitForTimeoutThenBackBtn(driver, specialClick.WaitMs, currentTitle);
                else
                {
                    // Switch to the new window/tab/etc.
                    driver.SwitchToWindowHandle(specialClickData.WindowHandle);
                    driver.MinimizeWindowByTitle(driver.Title);
                    specialClickData.WindowTitle = driver.Title;

                    // Switch to the existing window
                    driver.SwitchToWindowHandle(currentWindowHandle);
                    resultLinks = driver.FindElements(ClickOnElementAt);
                }
            }

            int nonSpecialClickCount = 5; // Default, only 5 clicks allowed
#if FULL_VERSION
            nonSpecialClickCount = resultLinks.Count; // Full version, number of clicks follow number of links
#endif
            // Only do the non-special clicks (should be same window)
            for (resultId = 0; resultId < nonSpecialClickCount; resultId++)
            {
                // If the result id is a special click, ignore it
                if (dict_resultId_specialClick.Keys.Contains(resultId))
                    continue;

                IWebElement resultLink = resultLinks[resultId];
                // Is not a special click so we can just click it, wait for ms, then back
                resultLink.Click();
                resultLinks = WaitForTimeoutThenBackBtn(driver, this.WaitMs, currentTitle);
            }

            return dict_resultId_specialClick;
        }

        public override void Run(WebDriver driver)
        {
            // Less wait should be at the beginning so we can close these windows first
            List<SpecialClickData> specialClicks = new List<SpecialClickData>();
            for (int pageIndex = 0; pageIndex < Pages; pageIndex++)
            {
                specialClicks.AddRange(ClickResults(driver).Values);

                // Don't click next on the last page
                if (pageIndex != Pages - 1)
                {
                    // Goto the next page
                    IWebElement next = driver.FindElement(ClickNextAt);
                    next.Click();
                }
            }

            specialClicks = specialClicks.OrderBy(o => o.SpecialClick.WaitMs).ToList();
            // After everything is done, close it after the time elapsed
            foreach (SpecialClickData scd in specialClicks)
            {
                TimeSpan elapsedTime = DateTime.Now.Subtract(scd.WindowCreatedTime);
                double waitLeft = scd.SpecialClick.WaitMs - elapsedTime.TotalMilliseconds;
                // If still need to wait... do that...
                if (waitLeft > 0)
                    WebDriver.WaitUntilTimeout(null, TimeSpan.FromMilliseconds(waitLeft));

                try
                {
                    // try close the window... if it fails (already closed), it's OK
                    driver.SwitchToWindowHandle(scd.WindowHandle);
                    driver.MaximizeWindowByTitle(scd.WindowHandle);
                    driver.Close();
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// Click and open in different window mode... with different wait settings.
    ///     Url gives the special Url - as a Regex, or a part of a Url. If it matches, it should be treated special
    /// </summary>
    public class SpecialClick : WebAction
    {
        [XmlAttribute(AttributeName = "Url")]
        public string Url { get; set; }

        private static Random Random = new Random();
        private int minMs;
        private int maxMs;
        [XmlAttribute(AttributeName = "WaitMsString")]
        public string WaitMsString { get; set; }

        public int WaitMs { get { return Random.Next(minMs, maxMs); } }

        [XmlAttribute(AttributeName = "Window")]
        public WindowType Window { get; set; }

        // Default values
        public SpecialClick()
        {
            minMs = 1000;
            maxMs = minMs;
            WaitMsString = minMs.ToString();

            Window = WindowType.Same;
        }

        public override void DoPostProcessing()
        {
            base.DoPostProcessing();

            // Split the WaitMs string to min/max int values
            string[] values = WaitMsString.Split('-');
            // Something went wrong with the splitting - use default values
            if (values.Length == 0)
                return;
            int.TryParse(values[0], out minMs);
            // Only when the length is 2 can we can use the second value
            if (values.Length == 2)
                int.TryParse(values[1], out maxMs);
            // max has to be greater than min
            if (maxMs < minMs)
                maxMs = minMs;
        }

        public override void Run(WebDriver driver)
        {
            throw new NotImplementedException();
        }

        public void Run(WebDriver driver, IWebElement element)
        {
            switch (Window)
            {
                case WindowType.Same:
                    element.Click();
                    break;
                case WindowType.New:
                    driver.OpenInNewWindow(element);
                    break;
                case WindowType.NewTab:
                    driver.OpenInNewTab(element);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
