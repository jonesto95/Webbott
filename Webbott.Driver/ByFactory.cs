using OpenQA.Selenium;

using Webbott.DOMSelectors;
using Webbott.Support;

namespace Webbott.Driver
{
    internal static class ByFactory
    {
        public static By By(this WebbottSelectorBase selector)
        {
            if (selector is ClassNameSelector classNameSelector)
                return By(classNameSelector);

            if (selector is CustomCssSelector customCssSelector)
                return By(customCssSelector);

            if (selector is CustomXPathSelector customXPathSelector)
                return By(customXPathSelector);
            
            if (selector is CssSelector cssSelector)
                return By(cssSelector);

            if (selector is IdSelector idSelector)
                return By(idSelector);

            if (selector is LinkTextSelector linkTextSelector)
                return By(linkTextSelector);

            if (selector is NameSelector nameSelector)
                return By(nameSelector);

            if (selector is TagNameSelector tagNameSelector)
                return By(tagNameSelector);

            if (selector is XPathSelector xPathSelector)
                return By(xPathSelector);

            throw UnknownSelectorTypeException.ThrowNew(selector.GetType());
        }


        public static By By(this ClassNameSelector selector)
            => OpenQA.Selenium.By.ClassName(selector.ClassName);


        public static By By(this CustomCssSelector selector)
            => OpenQA.Selenium.By.CssSelector(selector.CustomExpression);


        public static By By(this CustomXPathSelector selector)
            => OpenQA.Selenium.By.XPath(selector.CustomExpression);


        public static By By(this CssSelector selector)
            => OpenQA.Selenium.By.CssSelector(selector.ToFunctionalString());


        public static By By(this IdSelector selector)
            => OpenQA.Selenium.By.Id(selector.Id);


        public static By By(this LinkTextSelector selector)
        {
            if(selector.IsPartial)
            {
                return OpenQA.Selenium.By.PartialLinkText(selector.LinkText);
            }
            else
            {
                return OpenQA.Selenium.By.LinkText(selector.LinkText);
            }
        }


        public static By By(this NameSelector selector)
            => OpenQA.Selenium.By.Name(selector.Name);


        public static By By(this TagNameSelector selector)
            => OpenQA.Selenium.By.TagName(selector.TagName);


        public static By By(this XPathSelector selector)
            => OpenQA.Selenium.By.XPath(selector.ToFunctionalString());

    }
}
