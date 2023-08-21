using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SummaryAddApp
{
    /// <summary>
    /// Returns if a class uses `Mapper`
    /// </summary>
    public class UsesMapperWalker : CSharpSyntaxWalker
    {
        public UsesMapperWalker() : base(SyntaxWalkerDepth.StructuredTrivia)
        {
            UsesMapper = false;
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if (node.Expression.ToString() == "Mapper")
            {
                this.UsesMapper = true;
                return;
            }
            base.VisitMemberAccessExpression(node);
        }

        public bool UsesMapper { get; private set; }
    }
}
