namespace Webbott.DOMSelectors
{
    public class TagNameSelector : WebbottSelectorBase
    {
        public string TagName { get; private set; }


        public TagNameSelector(string tagName)
            => TagName = tagName;


        public override string ToString()
            => $"TagName:{TagName}";


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
            => $"var {variableName} = document.getElementsByTagName(\"{TagName}\")[{arrayIndex}];";
    }
}
