using System.Collections.Generic;

namespace Webbott.DOMSelectors
{
    public class CssSelector : WebbottSelectorBase
    {
        private readonly List<CssSelectorNode> Nodes;
        private readonly List<CssSelectorConjunction> Conjunctions;

        public CssSelector(string tagName)
        {
            Nodes = new List<CssSelectorNode>
            {
                new CssSelectorNode(tagName)
            };
            Conjunctions = new List<CssSelectorConjunction>();
        }


        public CssSelector WithClass(params string[] classes)
        {
            Nodes[^1].Classes(classes);
            return this;
        }


        public CssSelector NthChild(string expression)
        {
            Nodes[^1].NthChild(expression);
            return this;
        }
        

        // --------------------------
        // ATTRIBUTE EVALUATION METHODS
        // --------------------------

        public CssSelector AttributeContainsWord(string attribute, string value)
        {
            Nodes[^1].AttributeContainsWord(attribute, value);
            return this;
        }


        public CssSelector AttributeContains(string attribute, string value)
        {
            Nodes[^1].AttributeContains(attribute, value);
            return this;
        }


        public CssSelector AttributeStartsWith(string attribute, string value)
        {
            Nodes[^1].AttributeStartsWith(attribute, value);
            return this;
        }


        public CssSelector AttributeEndsWith(string attribute, string value)
        {
            Nodes[^1].AttributeEndsWith(attribute, value);
            return this;
        }


        // --------------------------
        // CONJUNCTION METHODS
        // --------------------------

        public CssSelector Or()
            => AddConjunction("*", ConjunctionType.Or);


        public CssSelector Or(string tagName)
            => AddConjunction(tagName, ConjunctionType.Or);


        public CssSelector Descendant()
            => AddConjunction("*", ConjunctionType.Descendant);


        public CssSelector Descendant(string tagName)
            => AddConjunction(tagName, ConjunctionType.Descendant);


        public CssSelector DirectChild()
            => AddConjunction("*", ConjunctionType.DirectChild);


        public CssSelector DirectChild(string tagName)
            => AddConjunction(tagName, ConjunctionType.DirectChild);


        public CssSelector FirstProceedingSibling()
            => AddConjunction("*", ConjunctionType.FirstProceedingSibling);


        public CssSelector FirstProceedingSibling(string tagName)
            => AddConjunction(tagName, ConjunctionType.FirstProceedingSibling);


        public CssSelector AllProceedingSiblings()
            => AddConjunction("*", ConjunctionType.AllProceedingSiblings);


        public CssSelector AllProceedingSiblings(string tagName)
            => AddConjunction(tagName, ConjunctionType.AllProceedingSiblings);


        private CssSelector AddConjunction(string tagName, ConjunctionType conjunctionType)
        {
            Nodes.Add(new CssSelectorNode(tagName));
            Conjunctions.Add(
                new CssSelectorConjunction(conjunctionType)
            );
            return this;
        }


        public override string ToString()
            => $"Css:{ToFunctionalString()}";


        public string ToFunctionalString()
        {
            string result = "";
            int i = 0;
            result += Nodes[i].ToString();
            while (i < Conjunctions.Count)
            {
                result += Conjunctions[i].ToString();
                i++;
                result += Nodes[i].ToString();
            }
            return result;
        }


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
        {
            string selector = ToString().Replace("\"", "\\\"");
            return $"var {variableName} = document.querySelectorAll(\"{selector}\")[{arrayIndex}];";
        }
    }

    internal class CssSelectorNode
    {
        public string TagName { get; private set; }
        private string nthChildExpression;
        private string id;
        private List<string> classList = new List<string>();
        private List<string> attributeEvaluators = new List<string>();

        public CssSelectorNode() 
            => TagName = "*";

        public CssSelectorNode(string tagName) 
            => TagName = tagName;

        public void Id(string id)
            => this.id = id;


        public void Classes(string[] classes)
            => classList.AddRange(classes);


        public void HasAttribute(string attribute)
            => attributeEvaluators.Add($"[{attribute}]");


        public void AttributeEquals(string attribute, string value)
            => attributeEvaluators.Add($"[{attribute}='{value}']");


        public void AttributeContains(string attribute, string value)
            => attributeEvaluators.Add($"[{attribute}*='{value}']");


        public void AttributeContainsWord(string attribute, string value)
            => attributeEvaluators.Add($"[{attribute}~='{value}']");


        public void AttributeStartsWith(string attribute, string value)
            => attributeEvaluators.Add($"[{attribute}^='{value}']");


        public void AttributeEndsWith(string attribute, string value)
            => attributeEvaluators.Add($"[{attribute}$='{value}']");


        public void NthChild(string expression)
            => nthChildExpression = expression;


        public override string ToString()
        {
            string result = TagName;
            if (id != null)
                result += $"#{id}";

            if (classList.Count > 0)
                for (int i = 0; i < classList.Count; i++)
                    result += $".{classList[i]}";

            if (attributeEvaluators.Count > 0)
                for (int i = 0; i < attributeEvaluators.Count; i++)
                    result += attributeEvaluators[i];

            if (nthChildExpression != null)
                result += $":nth-child({nthChildExpression})";

            return result;
        }
    }

    internal class CssSelectorConjunction
    {
        public ConjunctionType ConjunctionType { get; private set; }

        public CssSelectorConjunction(ConjunctionType conjunctionType)
            => ConjunctionType = conjunctionType;


        public override string ToString()
        {
            return ConjunctionType switch
            {
                ConjunctionType.Or => " , ",
                ConjunctionType.Descendant => " ",
                ConjunctionType.DirectChild => " > ",
                ConjunctionType.FirstProceedingSibling => " + ",
                ConjunctionType.AllProceedingSiblings => " ~ ",
                _ => " ",
            };
        }
    }

    public enum ConjunctionType
    {
        Or,
        DirectChild,
        Descendant,
        FirstProceedingSibling,
        AllProceedingSiblings
    }
}
