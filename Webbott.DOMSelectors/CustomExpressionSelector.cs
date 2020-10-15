namespace Webbott.DOMSelectors
{
    public abstract class CustomExpressionSelector : WebbottSelectorBase
    {
        public string CustomExpression { get; private set; }


        public CustomExpressionSelector(string customExpression)
            => CustomExpression = customExpression;
    }
}
