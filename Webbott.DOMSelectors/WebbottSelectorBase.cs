namespace Webbott.DOMSelectors
{
    public abstract class WebbottSelectorBase
    {
        public string DeclareJavascriptVariable(string variableName)
            => DeclareJavascriptVariable(variableName, 0);

        public abstract string DeclareJavascriptVariable(string variableName, int arrayIndex);
    }
}
