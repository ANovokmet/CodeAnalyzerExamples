using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SummaryAddApp
{
    /// <summary>
    /// Returns if a node already has a summary defined
    /// </summary>
    public class HasSummaryWalker : CSharpSyntaxWalker
    {
        public HasSummaryWalker() : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            HasSummary = false;
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
            if (node.StartTag.Name.LocalName.ValueText == "summary")
            {
                this.HasSummary = true;
                return;
            }
            base.VisitXmlElement(node);
        }

        public bool HasSummary { get; private set; }
    }
}
