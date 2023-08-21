using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SummaryAddApp
{
    /// <summary>
    /// Returns if a node has a `[ResponseType]` attribute
    /// </summary>
    public class ResponseTypeWalker : CSharpSyntaxWalker
    {
        public ResponseTypeWalker() : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            HasAttribute = false;
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            if (node.Name.ToString() == "ResponseType")
            {
                this.HasAttribute = true;
                this.ResponseTypeName = node.ArgumentList.ToString().Replace("(typeof(", "").Replace("))", "");
            }
            base.VisitAttribute(node);
        }

        public bool HasAttribute { get; private set; }
        public string ResponseTypeName { get; private set; }
    }
}
