using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace UI.Common
{
    // TODO: Serialize this
    public class WebElement_Select : SelectElement
    {
        IWebElement Element { get; set; }
        public WebElement_Select(IWebElement element)
            : base(element)
        {
            this.Element = element;
        }

        public new void SelectByText(string regex)
        {
            IWebElement matchedOption = this.Options
                .Where(o => Regex.IsMatch(o.Text, regex, RegexOptions.IgnoreCase)).First();
            matchedOption.Click();
        }

        public void ShowOptions()
        {
            this.Element.Click();
        }
    }
}
