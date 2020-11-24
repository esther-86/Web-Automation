using Common.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Collections.Generic;
using UI.Common;
using System.Linq;

#region "References "
// http://hlai.qhub.com/
// https://bitbucket.org/mysti9uemirage/uitest

// http://code.google.com/p/behaven/
// http://nunit.org/index.php?p=actionAttributes&r=2.6
// https://github.com/nunit/nunit/wiki

// http://msdn.microsoft.com/en-us/library/ee817676.aspx
// http://code.google.com/p/nunit-extension-datadriven/
// http://anoopjshetty.wordpress.com/2012/02/08/creating-test-Automation.Web.Web-framework-using-c-selenium-and-nunit/
// http://code.google.com/p/selenium/wiki/InternetExplorerDriver
// http://www.theautomatedtester.co.uk/selenium_training.htm
// http://stackoverflow.com/questions/866816/dynamically-create-tests-in-nunit
#endregion

namespace UITest
{
    public class Test_WebCommon
    {
        static GotoCollection CurrentConfiguration { get; set; }
        WebDriver WebDriver { get; set; }

        static Test_WebCommon()
        {
            string xml = System.IO.File.ReadAllText(@".\Configuration.xml");
            CurrentConfiguration = XmlTools.Deserialize<GotoCollection>(xml);
        }

        [TearDown]
        public virtual void Teardown()
        {
            if (this.WebDriver == null)
                return;

            this.WebDriver.Quit();
        }

        public static IEnumerable<TestCaseData> GetData()
        {
            foreach (Goto gotoItem in CurrentConfiguration.Gotos)
            {
                // Go through each of the browser enum. For example: gotoItem.Browser might be ALL
                foreach (Browser browser in Enum.GetValues(typeof(Browser)))
                {
                    // Skip the all enum: We only want to see the individual ones
                    if (browser == Browser.ALL)
                        continue;

                    // If specified IEXPLORE but flag looking at is CHROME, don't create a test for it
                    // However, if specified ALL, ALL has CHROME, create a test for it, and one for IEXPLORE too
                    if (!gotoItem.Browser.HasFlag(browser))
                        continue;

                    TestCaseData tcd = new TestCaseData(gotoItem);
                    tcd.SetCategory(browser.ToString());
                    yield return tcd;
                }
            }
        }

        [Test, TestCaseSource("GetData")]
        public void Run(Goto gotoItem)
        {
            Browser browser = gotoItem.Browser;
            IWebDriver iwebDriver = null;
            // Reading the current config to determing the browser, setting up right driver
            if (browser.HasFlag(Browser.IEXPLORE))
                iwebDriver = new InternetExplorerDriver();
            else if (browser.HasFlag(Browser.FIREFOX))
            {
                // Calling specific profile allows the extensions to be run
                // FirefoxProfile firefoxProfile = (new FirefoxProfileManager()).GetProfile("default");
                iwebDriver = new FirefoxDriver();
            }
            else if (browser.HasFlag(Browser.CHROME))
            {   
                // chrome can't be resized after launching
                // so we start it maximized here
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-popup-blocking");
                options.AddArgument("--system-developer-mode");
                iwebDriver = new ChromeDriver(options);
            }

            Object m_lock = new Object();
            this.WebDriver = new WebDriver(iwebDriver, m_lock);

            gotoItem.Run(this.WebDriver);
            int xmlActionCount = 0;
            foreach (WebAction webAction in gotoItem.WebActions)
            {
                try
                {
                    webAction.Run(this.WebDriver);
                    xmlActionCount++;
#if !FULL_VERSION
                    if (xmlActionCount >= 2) // 2 actions total: type and click links
                        break;
#endif
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Error when running web action: {0}; Message: {1} {2}",
                        webAction.ToString(), e.Message, e.GetStackFrame()));
                }
            }
        }
    }
}
