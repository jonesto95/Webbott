using System;
using System.Collections.Generic;
using System.Text;

namespace Webbott.DOMSelectors
{
    public class IdSelector : WebbottSelectorBase
    {
        public string Id { get; private set; }


        public IdSelector(string id)
            => Id = id;


        public override string ToString()
            => $"Id:{Id}";


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
            => $"var {variableName} = document.getElementById(\"{Id}\");";
    }
}
