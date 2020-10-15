namespace Webbott.DOMSelectors
{
    public class CustomXPathSelector : CustomExpressionSelector
    {
        public CustomXPathSelector(string customExpression) : base(customExpression) { }


        public override string ToString()
            => $"XPath:{CustomExpression}";

        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
            => $"var {variableName} = document.evaluate(\"{CustomExpression}\", document)[{arrayIndex}];";
    }
}
