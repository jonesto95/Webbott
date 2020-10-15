using System;
using System.Collections.Generic;
using System.Text;

namespace Webbott.DOMSelectors
{
    public class NameSelector : WebbottSelectorBase
    {
        public string Name { get; private set; }


        public NameSelector(string name)
            => Name = name;


        public override string ToString()
            => $"Name:{Name}";


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
            => $"var {variableName} = document.getElementsByName(\"{Name}\")[{arrayIndex}];";
    }
}
