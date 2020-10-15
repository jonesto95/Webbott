namespace Webbott.DOMSelectors
{
    public class LinkTextSelector : WebbottSelectorBase
    {
        public bool IsPartial { get; private set; }
        public string LinkText { get; private set; }


        public LinkTextSelector(string linkText)
        {
            LinkText = linkText;
            IsPartial = false;
        }


        public LinkTextSelector(string linkText, bool isPartial)
        {
            LinkText = linkText;
            IsPartial = isPartial;
        }


        public override string ToString()
        {
            string result = IsPartial ? "Partial" : "";
            result += $"LinkText:{LinkText}";
            return result;
        }


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
        {
            XPathSelector linkTextXpath = new XPathSelector("a").Where("text()");
            if (IsPartial)
            {
                linkTextXpath = linkTextXpath.Contains(LinkText);
            }
            else
            {
                linkTextXpath = linkTextXpath.IsEqualTo(LinkText);
            }
            return linkTextXpath.DeclareJavascriptVariable(variableName, arrayIndex);
        }
    }
}
