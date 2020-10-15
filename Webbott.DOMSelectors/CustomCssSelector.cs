namespace Webbott.DOMSelectors
{
    public class CustomCssSelector : CustomExpressionSelector
    {
        public CustomCssSelector(string customExpression) : base(customExpression) { }


        public override string ToString()
            => $"Css:{CustomExpression}";


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
            => $"var {variableName} = document.querySelectorAll(\"{CustomExpression}\")[{arrayIndex}];";
    }
}
