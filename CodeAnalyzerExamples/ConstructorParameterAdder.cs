using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static SummaryAddApp.Program;

namespace SummaryAddApp
{
    /// <summary>
    /// If a class is using AutoMapper:
    ///     adds `using Automapper;` to using declarations
    ///     adds `private IMapper _mapper;` to class properties
    ///     adds `IMapper mapper` parameter and `_mapper = mapper;` assignment to class constructor
    /// </summary>
    public class ConstructorParameterAdder : CSharpSyntaxRewriter
    {
        public ConstructorParameterAdder() : base(true)
        {

        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var walker = new UsesMapperWalker();
            walker.Visit(node);
            if (walker.UsesMapper)
            {
                // rewrite constructor and add using AutoMapper;
                var rewriter = new UsingAdder();
                node = (CompilationUnitSyntax)rewriter.VisitCompilationUnit(node);
            }
            return base.VisitCompilationUnit(node);
        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            return base.VisitConstructorDeclaration(node);
        }

        public override SyntaxNode VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            return base.VisitConstructorInitializer(node);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var walker = new UsesMapperWalker();
            walker.Visit(node);
            if (walker.UsesMapper)
            {
                // rewrite constructor and add using AutoMapper;
                var rewriter = new ConstructorParameterAdderInner();
                node = (ClassDeclarationSyntax)rewriter.VisitClassDeclaration(node);
            }
            return base.VisitClassDeclaration(node);
        }
    }

    public class UsingAdder : CSharpSyntaxRewriter
    {
        public UsingAdder() : base(true)
        {

        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            node = node.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("AutoMapper")));

            return base.VisitCompilationUnit(node);
        }
    }

    public class ConstructorParameterAdderInner : CSharpSyntaxRewriter
    {
        public ConstructorParameterAdderInner() : base(true)
        {

        }

        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            node = node.AddParameterListParameters(
                SyntaxFactory.Parameter(SyntaxFactory.Identifier("mapper")).WithType(SyntaxFactory.ParseTypeName("IMapper"))
            ).AddBodyStatements(
                SyntaxFactory.ExpressionStatement(SyntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, SyntaxFactory.IdentifierName("_mapper"), SyntaxFactory.IdentifierName("mapper")))
            );

            return base.VisitConstructorDeclaration(node);
        }

        public override SyntaxNode VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            return base.VisitConstructorInitializer(node);
        }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var member = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("IMapper"), SyntaxFactory.SeparatedList(new[] { SyntaxFactory.VariableDeclarator(SyntaxFactory.Identifier("_mapper")) })))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword), SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword));
            node = node.WithMembers(node.Members.Insert(0, member));

            return base.VisitClassDeclaration(node);
        }
    }
}
