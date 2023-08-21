using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static SummaryAddApp.Program;

namespace SummaryAddApp
{
    /// <summary>
    /// Adds a summary to properties of classes and interfaces that do not have it 
    /// </summary>
    public class SummaryAdder : CSharpSyntaxRewriter
    {
        public SummaryAdder() : base(true)
        {

        }

        private SyntaxTrivia CreateSimpleSummary(string content)
        {
            var comment = SyntaxFactory.DocumentationCommentTrivia(SyntaxKind.SingleLineDocumentationCommentTrivia)
                .AddContent(
                    SyntaxFactory.XmlElement(
                        SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("summary")),
                        SyntaxFactory.List<XmlNodeSyntax>().Add(
                            SyntaxFactory.XmlText().AddTextTokens(
                                SyntaxFactory.XmlTextNewLine(Environment.NewLine),
                                SyntaxFactory.XmlTextLiteral(content),
                                SyntaxFactory.XmlTextNewLine(Environment.NewLine)
                            )
                        ),
                        SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("summary"))
                    )//,
                     //SyntaxFactory.XmlElement(
                     //    SyntaxFactory.XmlElementStartTag(SyntaxFactory.XmlName("param"))
                     //        .AddAttributes(SyntaxFactory.XmlTextAttribute("name", "args123")),
                     //    SyntaxFactory.List<XmlNodeSyntax>().Add(
                     //        SyntaxFactory.XmlText()
                     //    ),
                     //    SyntaxFactory.XmlElementEndTag(SyntaxFactory.XmlName("param"))
                     //)
                )
                .WithLeadingTrivia(SyntaxFactory.DocumentationCommentExterior("/// "))
                .WithTrailingTrivia(SyntaxFactory.EndOfLine(Environment.NewLine));
            return SyntaxFactory.Trivia(comment);
        }

        public override SyntaxNode VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            var walker = new HasSummaryWalker();
            walker.Visit(node);

            if (!walker.HasSummary)
            {
                var comment = CreateSimpleSummary(node.Identifier.ValueText);
                node = node.WithLeadingTrivia(SyntaxFactory.TriviaList(comment));
            }

            return base.VisitInterfaceDeclaration(node);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var walker = new HasSummaryWalker();
            walker.Visit(node);

            if (!walker.HasSummary)
            {
                var comment = CreateSimpleSummary(node.Identifier.ValueText);
                node = node.WithLeadingTrivia(SyntaxFactory.TriviaList(comment));
            }

            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var walker = new HasSummaryWalker();
            walker.Visit(node);

            //if (!walker.HasSummary)
            //{
            //    var comment = CreateSimpleSummary(node.Identifier.ValueText);
            //    node = node.WithLeadingTrivia(SyntaxFactory.TriviaList(comment));
            //}

            var responseTypeWalker = new ResponseTypeWalker();
            responseTypeWalker.Visit(node);
            if (responseTypeWalker.HasAttribute)
            {
                node = node.WithReturnType(SyntaxFactory.IdentifierName(responseTypeWalker.ResponseTypeName));
            }

            return base.VisitMethodDeclaration(node);
        }

        public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            var walker = new HasSummaryWalker();
            walker.Visit(node);

            //if (!walker.HasSummary)
            //{
            //    var comment = CreateSimpleSummary(node.Identifier.ValueText);
            //    node = node.WithLeadingTrivia(SyntaxFactory.TriviaList(comment));
            //}

            return base.VisitPropertyDeclaration(node);
        }
    }
}
