using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System;
using Common.Utilities;
using System.Collections.Generic;
using OpenQA.Selenium.Interactions;
using AutoIt;

namespace UI.Common
{
    #region "Possible problems"
    // The IWebDriver object must implement or wrap a driver that implements IHasInputDevices: Due to wrapped WebDriver
    // The IWebElement object must implement or wrap an element that implements ILocatable.": Due to wrapped Element
    //      To solve: Put the actions inside the wrappers
    #endregion

    public enum Operation
    { CLICK }

    [Flags]
    public enum Options_QSReplacement : short
    {
        QUERYSTRING = 1,
        ANCHOR = 2,
        ALL = QUERYSTRING | ANCHOR
    }

    public class WebDriver : IWebDriver
    {
        // TODO: changed from -999, don't remember why it was set to -999 figure out why
        public static int IFRAME_INDEX_ERROR = -1;

        public static bool IsActive = true;

        private Object m_lock;
        protected IWebDriver driver;
        // Didn't want users to have to pass in a timespan each time. 
        // This will use a default timespan until the user changes it
        public TimeSpan Timespan_To_Wait { get; set; }
        public static TimeSpan TimespanForStabilization = TimeSpan.FromSeconds(1);

        public WebDriver(IWebDriver driver, Object _lock)
        {
            this.driver = driver;
            this.m_lock = _lock;
            this.Timespan_To_Wait = TimeSpan.FromMinutes(1);
        }

        // http://blog.wedoqa.com/2014/09/how-to-open-a-link-in-a-new-tab-or-window-and-switch-to-it-with-selenium-webdriver/
        public void OpenInNewWindow(IWebElement element)
        {
            if (element is WebElement)
                element = ((WebElement)element).Element;
            Actions actionOpenLinkInNewWindow = new Actions(driver);
            actionOpenLinkInNewWindow.MoveToElement(element)
                       .KeyDown(Keys.Shift)
                       .Click(element)
                       .KeyUp(Keys.Shift)
                       .Perform();
            WaitUntilTimeout(null, TimespanForStabilization);
        }

        public void OpenInNewTab(IWebElement element)
        {
            if (element is WebElement)
                element = ((WebElement)element).Element;
            Actions actionOpenLinkInNewTab = new Actions(driver);
            actionOpenLinkInNewTab.MoveToElement(element)
                        .KeyDown(Keys.Command)
                        .KeyDown(Keys.Shift)
                        .Click(element)
                        .KeyUp(Keys.Command)
                        .KeyUp(Keys.Shift)
                        .Perform();
            WaitUntilTimeout(null, TimespanForStabilization);
        }

        public double WaitUntil(Func<bool> waitUntilFunction)
        {
            return WaitUntilTimeout(waitUntilFunction, TimeSpan.FromMinutes(30));
        }

        public double WaitUntilTimeout(Func<bool> waitUntilFunction, string comment)
        {
            double duration = WaitUntilTimeout(waitUntilFunction, this.Timespan_To_Wait);
            Console.WriteLine(string.Format("\tSTATISTICS:\t{0}\t{1}", comment, duration));
            return duration;
        }

        public double WaitUntilTimeout(Func<bool> waitUntilFunction)
        {
            return WaitUntilTimeout(waitUntilFunction, this.Timespan_To_Wait);
        }

        public virtual WebElement FindElement_NoWait(By by)
        {
            lock (m_lock)
            {
                WebElement foundElement = null;
                try
                {
                    IWebElement element = this.driver.FindElement(by);
                    foundElement = new WebElement(element, m_lock);
                }
                catch {}
                return foundElement;
            }
        }

        public IWebElement Do(Operation operation, By constraint)
        {
            IWebElement element = FindElement(constraint);

            // https://groups.google.com/forum/#!topic/selenium-users/5Y5WC523jXE
            // element.Click(), when it launches a file import dialog, 
            // The function got stuck in the click method.
            // TODO: Post this to Google Group so that it can be solved with ClickNoWait 
            // similar to WatiN's functionality
            if (operation == Operation.CLICK)
                element.Click(); // WebElement will call ClickAndStabilize

            return element;
        }

        // http://darrellgrainger.blogspot.com/2012/04/frames-and-webdriver.html 
        /// <summary>
        /// IMPORTANT: To find elements in iframe, such as elements in a dialog, you must first 
        /// SwitchTo() that frame, but the focus will remain on the iFrame until you switch out
        /// like is done at the end of this function.
        /// Instead of duplicating code for each dialog class, we call DoActionEvenForNestedElements
        /// NOTE: posted to Google Group. If good response is received, update method (last checked 11/2012)
        /// Putting this in web driver because we want this to be commonly used
        /// Whenever an element cannot be found with the current web driver,
        /// the user should try to call this function
        /// Taking in an action instead of returning an element because if we found the element
        /// in the nested iframe, and we switch back to default content, WebDriver will think
        /// that the element has gone stale, thus we need to pass in an action
        /// so that all of the actions to be done for that element can be done
        /// before the switch out of the iframe
        /// Saw the need for this when did file upload. The upload control is a nested iframe
        /// We don't want to know how many nested iframes there are in the web page
        /// We just want to be able to find the element, and do something with it.
        /// Reason for DataStore design and serialization in web driver code: 
        /// https://groups.google.com/forum/#!topic/webdriver/me6jz9xTYuQ
        /// Per Brian, to add thread safe code:
        /// 1. Anything that might use the web driver needs to be encapsulated in a lock statement, 
        /// use the same lock object. |_ HN: Be careful because this might introduce deadlock. 
        /// This was what I read from online threads regarding multi-threading.
        /// 2. This includes objects returned (i.e. IWebDriver, etc.), that might use the driver 
        /// object in their implementation. These will need to be encapsulated in a wrapper class, 
        /// with the original element and a lock object, so that locking can be implemented for the 
        /// functions that might have a reference to the driver object and uses it in their implementation.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>Nested iframe index (for use with Javascript). IFRAME_INDEX_ERROR if there was an error</returns>
        public int DoActionEvenForNestedElements(Action action)
        {
            // If we have no guess for the located of the nested elements
            // we default to -1, which is the main window (not in any iframe)
            int defaultGuess = -1;
            return DoActionEvenForNestedElements(action, defaultGuess);
        }

        /// <summary>
        /// This overload allows user to start a specified frame where element is
        /// likely to be found. Saves a lot of time if guess is correct, does not
        /// negatively impact if guess is incorrect
        /// </summary>
        /// <param name="action"></param>
        /// <param name="indexGuess"></param>
        /// <returns></returns>
        public int DoActionEvenForNestedElements(Action action, int indexGuess)
        {
            TimeSpan originalTimeSpanToWait = this.Timespan_To_Wait;
            // Shorten the wait time because need to look through all iFrames to find one matching
            this.Timespan_To_Wait = TimeSpan.FromSeconds(5);

            // index of -1 means the overall window, not in an iframe
            // index of 0 is the first frame
            ReadOnlyCollection<IWebElement> iframes = this.FindElements(By.CssSelector("iframe"));

            int foundIFrame = IFRAME_INDEX_ERROR;
            bool isFirstGuess = true;

            // Try to find the frame multiple times because the wait might be too short to find it
            // in the first pass.
            bool success = false;
            DateTime endTime = DateTime.Now.Add(originalTimeSpanToWait);
            while (DateTime.Now < endTime && !success)
            {
                // Start with indexGuess, then try -1... until all of the frames, for the specified timeout
                for (int i = indexGuess; i < iframes.Count && !success; i++)
                {
                    if (i >= 0)
                    { this.SwitchTo().Frame(i); }

                    // Try/catch because even if there was an error or not,
                    // we need to see if we should switch back to default content or not
                    try
                    {
                        action.Invoke();
                        foundIFrame = i;
                        success = true;
                    }
                    catch { }

                    if (i >= 0) // Regardless of when we find it, we must switch back to DefaultContent at the end
                    { this.SwitchTo().DefaultContent(); }

                    // If we are guessing, and it's the first guess, we re-set the counter to -1 to start at the beginning
                    // Otherwise, even if it's the indexGuess, we will continue looking at the next iFrame
                    if (isFirstGuess && indexGuess != -1)
                    {
                        i = -1;
                        isFirstGuess = false;
                    }
                }
            }

            this.Timespan_To_Wait = originalTimeSpanToWait;
            return foundIFrame;
        }

        public static double WaitUntilTimeout(Func<bool> waitUntilFunction, TimeSpan timespanToWait)
        {
            //WebDriverWait wait = new WebDriverWait(this.driver, timespanToWait);
            //wait.Until(waitUntilFunction);

            // If add max timespan, we might get exception
            DateTime startTime = DateTime.Now;
            DateTime endTime = startTime.Add(timespanToWait);
            bool success = false;
            while (DateTime.Now < endTime && !success && IsActive)
            {
                if (waitUntilFunction != null)
                    success = waitUntilFunction.Invoke();
                System.Threading.Thread.Sleep(500);
            }

            TimeSpan elapsed = DateTime.Now.Subtract(startTime);

            // If waitUntilFunction is null, user just wants to wait, not do anything
            if (waitUntilFunction == null)
                success = true;

            if (!success)
                throw new TimeoutException();

            return elapsed.TotalMilliseconds;
        }

        public void RunScript(string script)
        {
            ExecuteJScript<object>(script);
        }

        public T ExecuteJScript<T>(string script)
        {
            object returnValue = null;
            lock (m_lock)
            {
                try
                {
                    returnValue = ((IJavaScriptExecutor)this.driver).ExecuteScript(script);
                }
                catch { }
                // If error, ignore it - will return null
            }
            if (returnValue == null)
                return default(T);
            return ((T)returnValue);
        }

        #region "Using the base methods"

        public string CurrentWindowHandle
        { get { lock (m_lock) { return this.driver.CurrentWindowHandle; } } }

        public ReadOnlyCollection<string> WindowHandles
        { get { lock (m_lock) { return this.driver.WindowHandles; } } }

        public string PageSource
        { get { lock (m_lock) { return this.driver.PageSource; } } }

        public string Title
        { get { lock (m_lock) { return this.driver.Title; } } }

        public string Url
        {
            get { lock (m_lock) { return this.driver.Url; } }
            set { lock (m_lock) { this.driver.Url = value; } }
        }

        public void Close()
        { lock (m_lock) { this.driver.Close(); } }

        public void Quit()
        { lock (m_lock) { this.driver.Quit(); } }


        public IOptions Manage()
        { lock (m_lock) { return new Options(this.driver.Manage(), m_lock); } }

        public INavigation Navigate()
        { lock (m_lock) { return new Navigation(this.driver.Navigate(), m_lock); } }

        // If wrap this, will get 'OpenQA.Selenium.NoSuchFrameException' 
        public ITargetLocator SwitchTo()
        { return this.driver.SwitchTo(); }

        public virtual void Dispose()
        { lock (m_lock) { this.driver.Dispose(); } }

        public virtual IWebElement FindElement(string elementAt)
        {
            IWebElement element = null;
            // Will always try css selector first, then id, then xpath
            try { element = FindElement(By.CssSelector(elementAt)); }
            catch
            {
                try { element = FindElement(By.Id(elementAt)); }
                catch { element = FindElement(By.XPath(elementAt)); }
            }
            return element;
        }

        public virtual IWebElement FindElement(By by)
        {
            IWebElement element = null;
            WaitUntilTimeout(() => CanFindActiveElement(by, ref element));
            return new WebElement(element, m_lock);
        }

        protected bool CanFindActiveElement(By by, ref IWebElement retElement)
        {
            // Just wait until this element is not null
            // NOTE: If issues result from CanFindActiveElement, look into adding check for visibility
            // If element doesn't have visibility attribute, is it worth it to look at the attribute?
            // Because if not coded by WAS devs, sometimes, we won't know if the element is actually visible
            // So far, current implementation seems sufficient

            // Console.WriteLine("about to get lock for CanFindActive " + DateTime.Now);
            lock (m_lock)
            {
                try
                {
                    // Console.WriteLine("Looking for element by: " + by + " at " + DateTime.Now);
                    // 11/5/2012: Changed this to look through a list of elements
                    // Saw in the case of button clicking, there might be hidden dialogs
                    // and there are visible dialogs, thus, need to find all buttons
                    // if they are active, then return them to the caller
                    ReadOnlyCollection<IWebElement> elements = this.driver.FindElements(by);
                    foreach (IWebElement element in elements)
                    {
                        retElement = element;
                        // Wait a little bit to make sure that the element is attached to its DOM
                        // So that we won't get StaleElementException
                        WaitUntilTimeout(null, new TimeSpan(0, 0, 0, 0, 5));
                        if (element.Displayed && element.Enabled)
                            return true;
                    }
                }
                catch { } // https://groups.google.com/forum/?fromgroups=#!topic/webdriver/lUgHRB1-vTI
            }
            return false;
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(string elementAt)
        {
            ReadOnlyCollection<IWebElement> elements = null; 
            // Will always try css selector first, then id, then xpath
            try { elements = FindElements(By.CssSelector(elementAt)); }
            catch
            {
                try { elements = FindElements(By.Id(elementAt)); }
                catch { elements = FindElements(By.XPath(elementAt)); }
            }
            return elements;
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            lock (m_lock)
            {
                ReadOnlyCollection<IWebElement> elements = this.driver.FindElements(by);
                List<IWebElement> retElements = new List<IWebElement>();
                foreach (IWebElement iElement in elements)
                    retElements.Add(new WebElement(iElement, m_lock));
                return new ReadOnlyCollection<IWebElement>(retElements);
            }
        }

        #endregion

        // For window controls that are not in WebDriver
        #region "AutoItX"
        public void SwitchToWindowHandle(string windowHandle)
        {
            SwitchTo().Window(windowHandle);
        }

        public void MinimizeWindowByTitle(string title)
        {
            AutoItX.WinSetState(title, "", AutoItX.SW_MINIMIZE);
        }

        public void MaximizeWindowByTitle(string title)
        {
            AutoItX.WinSetState(title, "", AutoItX.SW_MAXIMIZE);
        }
        #endregion
    }

    public class WebElement : IWebElement
    {
        private Object m_lock;
        protected IWebElement element;
        public IWebElement Element { get { return element; } }

        public WebElement(IWebElement iWebElement, Object _lock)
        {
            m_lock = _lock;
            this.element = iWebElement;
        }

        #region "Using the base methods"
        public void Clear()
        { lock (m_lock) { element.Clear(); } }

        public void Click()
        {
            lock (m_lock)
            {
                element.Click(); 
                WebDriver.WaitUntilTimeout(null, WebDriver.TimespanForStabilization);
            }
        }

        public bool Displayed
        { get { lock (m_lock) { return element.Displayed; }; } }

        public bool Enabled
        { get { lock (m_lock) { return element.Enabled; }; } }

        public string GetAttribute(string attributeName)
        {
            lock (m_lock)
            {
                //if (attributeName.Equals("src"))
                //{ Console.WriteLine("looking for a src"); }
                return element.GetAttribute(attributeName);
            }
        }

        public string GetCssValue(string propertyName)
        { lock (m_lock) { return element.GetCssValue(propertyName); } }

        public System.Drawing.Point Location
        { get { lock (m_lock) { return element.Location; } } }

        public bool Selected
        { get { lock (m_lock) { return element.Selected; } } }

        public void SendKeys(string text)
        { lock (m_lock) { element.SendKeys(text); } }

        public System.Drawing.Size Size
        { get { lock (m_lock) { return element.Size; } } }

        public void Submit()
        { lock (m_lock) { element.Submit(); } }

        public string TagName
        { get { lock (m_lock) { return element.TagName; } } }

        public string Text
        { get { lock (m_lock) { return element.Text; } } }

        public IWebElement FindElement(By by)
        {
            lock (m_lock)
            {
                IWebElement retElement = element.FindElement(by);
                return new WebElement(retElement, m_lock);
            }
        }

        public virtual ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            lock (m_lock)
            {
                ReadOnlyCollection<IWebElement> elements = this.element.FindElements(by);
                List<IWebElement> retElements = new List<IWebElement>();
                foreach (IWebElement iElement in elements)
                { retElements.Add(new WebElement(iElement, m_lock)); }
                return new ReadOnlyCollection<IWebElement>(retElements);
            }
        }

        public string GetProperty(string propertyName)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class Options : IOptions
    {
        private Object m_lock;
        protected IOptions options;

        public Options(IOptions iOptions, Object _lock)
        {
            m_lock = _lock;
            this.options = iOptions;
        }

        #region "Using the base methods"
        public ICookieJar Cookies
        { get { lock (m_lock) { return options.Cookies; } } }

        public IWindow Window
        { get { lock (m_lock) { return options.Window; } } }

        public ILogs Logs => throw new NotImplementedException();

        public ITimeouts Timeouts()
        { lock (m_lock) { return options.Timeouts(); } }

        #endregion
    }

    public class TargetLocator : ITargetLocator
    {
        private Object m_lock;
        protected ITargetLocator locator;

        public TargetLocator(ITargetLocator iTargetLocator, Object _lock)
        {
            m_lock = _lock;
            this.locator = iTargetLocator;
        }

        #region "Using the base methods"
        public IWebElement ActiveElement()
        { lock (m_lock) { return new WebElement(locator.ActiveElement(), m_lock); } }

        public IAlert Alert()
        { lock (m_lock) { return locator.Alert(); } }

        public IWebDriver DefaultContent()
        { lock (m_lock) { return new WebDriver(locator.DefaultContent(), m_lock); } }

        public IWebDriver Frame(int frameIndex)
        { lock (m_lock) { return new WebDriver(locator.Frame(frameIndex), m_lock); } }

        public IWebDriver Frame(IWebElement frameElement)
        {
            lock (m_lock)
            {
                WebElement element = new WebElement(frameElement, m_lock);
                return new WebDriver(locator.Frame(element), m_lock);
            }
        }

        public IWebDriver Frame(string frameName)
        { lock (m_lock) { return new WebDriver(locator.Frame(frameName), m_lock); } }

        public IWebDriver Window(string windowName)
        { lock (m_lock) { return new WebDriver(locator.Frame(windowName), m_lock); } }

        public IWebDriver ParentFrame()
        { lock (m_lock) { return new WebDriver(locator.ParentFrame(), m_lock); } }

        #endregion
    }

    public class Navigation : INavigation
    {
        private Object m_lock;
        protected INavigation navigaton;

        public Navigation(INavigation iNavigaton, Object _lock)
        {
            m_lock = _lock;
            this.navigaton = iNavigaton;
        }

        #region "Using the base methods"
        public void Back()
        { lock (m_lock) { navigaton.Back(); } }

        public void Forward()
        { lock (m_lock) { navigaton.Forward(); } }

        public void GoToUrl(string url)
        { lock (m_lock) { navigaton.GoToUrl(url); } }

        public void GoToUrl(Uri url)
        { lock (m_lock) { navigaton.GoToUrl(url); } }

        public void Refresh()
        { lock (m_lock) { navigaton.Refresh(); } }

        #endregion
    }
}
