using System;
using System.Collections.Generic;

namespace Webbott.DOMSelectors
{
    public class XPathSelector : WebbottSelectorBase
    {
        private List<XPathNode> Nodes { get; set; }


        // --------------------------
        // CONSTRUCTOR METHODS
        // --------------------------

        public XPathSelector() 
            => CreateNodeList("*", false);


        public XPathSelector(string tag)
            => CreateNodeList(tag, false);


        public XPathSelector(string tag, bool isAbsolute) 
            => CreateNodeList(tag, isAbsolute);


        private void CreateNodeList(string tag, bool isAbsolute)
        {
            Nodes = new List<XPathNode>()
            {
                new XPathNode(tag, isAbsolute)
            };
        }

        // --------------------------
        // CHILD NODE METHODS
        // --------------------------

        public XPathSelector Child()
            => AddChild("*", false);


        public XPathSelector Child(string tagName)
            => AddChild(tagName, false);


        public XPathSelector Child(string tagName, bool directDescendant)
            => AddChild(tagName, directDescendant);


        private XPathSelector AddChild(string tagName, bool directDescendant)
        {
            var newNode = new XPathNode(tagName, directDescendant);
            Nodes.Add(newNode);
            return this;
        }

        // --------------------------
        // MISC METHODS
        // --------------------------

        public XPathSelector Where(string attribute)
        {
            attribute = ValidateAttribute(attribute);
            Nodes[^1].Clauses[^1].Attribute = attribute;
            return this;
        }


        public XPathSelector WhereNot(string attribute)
        {
            attribute = ValidateAttribute(attribute);
            Nodes[^1].Clauses[^1].Not = true;
            Nodes[^1].Clauses[^1].Attribute = attribute;
            return this;
        }


        public XPathSelector Index(int index)
        {
            Nodes[^1].Index = index;
            return this;
        }


        // --------------------------
        // VALUE TRANSFORMATION METHODS
        // --------------------------

        public XPathSelector ToUpper()
            => Translate("abcdefghijklmnopqrstuvwxyz", "ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        public XPathSelector ToLower()
            => Translate("ABCDEFGHIJKLMNOPQRSTUVWXYZ", "abcdefghijklmnopqrstuvwxyz");


        public XPathSelector Translate(string inputChars, string outputChars)
        {
            if (inputChars == null)
                throw new ArgumentNullException(nameof(inputChars));

            if (outputChars == null)
                throw new ArgumentNullException(nameof(outputChars));

            inputChars = ValidateValue(inputChars);
            outputChars = ValidateValue(outputChars);
            string attribute = Nodes[^1].Clauses[^1].Attribute;
            Nodes[^1].Clauses[^1].Attribute = $"translate({attribute}, {inputChars}, {outputChars})";
            return this;
        }


        // --------------------------
        // VERB METHODS
        // --------------------------

        public XPathSelector DoesNotExist()
            => SetAction(Action.DoesNotExist, "");


        public XPathSelector Contains(string value)
            => SetAction(Action.Contains, value);


        public XPathSelector StartsWith(string value)
            => SetAction(Action.StartsWith, value);


        public XPathSelector EndsWith(string value)
            => SetAction(Action.EndsWith, value);


        public XPathSelector IsEqualTo(string value)
            => SetAction(Action.Equals, value);


        private XPathSelector SetAction(Action action, string value)
        {
            value = ValidateValue(value);
            Nodes[^1].Clauses[^1].Action = action;
            Nodes[^1].Clauses[^1].Value = value;
            return this;
        }


        // --------------------------
        // CONJUNCTION METHODS
        // --------------------------

        public XPathSelector And(string attribute)
            => AddConjunction(attribute, false, "and");


        public XPathSelector AndNot(string attribute)
            => AddConjunction(attribute, true, "and");


        public XPathSelector Or(string attribute)
            => AddConjunction(attribute, false, "or");


        public XPathSelector OrNot(string attribute)
            => AddConjunction(attribute, true, "or");


        private XPathSelector AddConjunction(string attribute, bool not, string conjunctionType)
        {
            attribute = ValidateAttribute(attribute);
            var clause = new XPathClause
            {
                Attribute = attribute,
                Not = not
            };
            Nodes[^1].Clauses.Add(clause);
            Nodes[^1].Conjunctions.Add(new XPathConjunction(conjunctionType));
            return this;
        }


        // --------------------------
        // STRING VALIDATION METHODS
        // --------------------------

        private string ValidateAttribute(string attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException(nameof(attribute));

            if (!attribute.Equals("text()", StringComparison.OrdinalIgnoreCase)
                && !attribute.Equals("position()", StringComparison.OrdinalIgnoreCase)
                && !attribute.Equals("last()", StringComparison.OrdinalIgnoreCase)
                && !attribute.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                attribute = $"@{attribute}";

            return attribute;
        }


        private string ValidateValue(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            char delimiter = '"';
            if (value.Contains('"'))
                delimiter = '\'';

            value = $"{delimiter}{value}{delimiter}";
            return value;
        }


        // --------------------------
        // OVERRIDE, TOSTRING METHODS
        // --------------------------

        public override string ToString()
            => $"XPath:{ToFunctionalString()}";


        public string ToFunctionalString()
        {
            string result = "";
            for (int i = 0; i < Nodes.Count; i++)
                result += Nodes[i].ToString();

            return result;
        }


        public override string DeclareJavascriptVariable(string variableName, int arrayIndex)
        {
            string _xPath = $"{ToFunctionalString().Replace("\"", "\\\"")}";
            return $"var {variableName} = document.evaluate(\"{_xPath}\", document)[{arrayIndex}];";
        }
    }

    internal class XPathNode
    {
        public string TagName { get; private set; }

        public int Index { get; set; }

        public bool IsAbsolute { get; private set; }

        public List<XPathClause> Clauses { get; private set; }

        public List<XPathConjunction> Conjunctions { get; private set; }


        public XPathNode(string tagName, bool isAbsolute)
        {
            TagName = tagName;
            IsAbsolute = isAbsolute;
            Clauses = new List<XPathClause>
            {
                new XPathClause()
            };
            Conjunctions = new List<XPathConjunction>();
        }


        public override string ToString()
        {
            string result = "/";
            if (!IsAbsolute)
                result += "/";

            result += TagName;
            if(Clauses[0].Attribute != null)
            {
                result += "[";
                int i = 0;
                result += Clauses[i].ToString();
                while(i < Conjunctions.Count)
                {
                    result += Conjunctions[i].ToString();
                    i++;
                    result += Clauses[i].ToString();
                }
                result += "]";
            }
            if (Index > 0)
                result += $"[{Index}]";

            return result;
        }
    }

    internal class XPathClause
    {
        public string Attribute { get; set; }

        public Action Action { get; set; }

        public string Value { get; set; }

        public bool Not { get; set; }

        public override string ToString()
        {
            string result = "";
            switch(Action)
            {
                case Action.Contains:
                    result = $"contains({Attribute}, {Value})";
                    break;

                case Action.Equals:
                    result = $"{Attribute}={Value}";
                    break;

                case Action.DoesNotExist:
                    result = $"not({Attribute})";
                    break;

                case Action.StartsWith:
                    result = $"starts-with({Attribute}, {Value})";
                    break;

                case Action.EndsWith:
                    result = $"ends-with({Attribute}, {Value})";
                    break;
            }
            if (Not)
                result = $"not({result})";

            return result;
        }
    }


    internal class XPathConjunction
    {
        private string Conjunction { get; set; }

        public XPathConjunction(string conjunction)
            => Conjunction = conjunction;

        public override string ToString()
        {
            return $" {Conjunction} ";
        }
    }

    public enum Action
    {
        Equals,
        Contains,
        DoesNotExist,
        StartsWith,
        EndsWith
    }
}
