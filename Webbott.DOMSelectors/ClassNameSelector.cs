using System;
using System.Collections.Generic;
using System.Text;

namespace Webbott.DOMSelectors
{
    public class ClassNameSelector : WebbottSelectorBase
    {
        public string ClassName { get; private set; }

        public ClassNameSelector(string className)
            => ClassName = className;


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
        {
            XPathSelector xPathSelector = new XPathSelector()
                .Where("class").IsEqualTo(ClassName)
                .Or("class").Contains($"{ClassName} ");
            return xPathSelector.DeclareJavascriptVariable(variableName, arrayIndex);
        }


        public override string ToString() => $"ClassName:{ClassName}";
    }
}
