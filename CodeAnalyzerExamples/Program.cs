using System;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace SummaryAddApp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\ante.novokmet\source\repos\SummaryAddApp\SummaryAddApp\Class1.cs";
            string code = File.ReadAllText(path);

            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var collector = new SummaryAdder();
            var targetRoot = collector.Visit(root);//.NormalizeWhitespace();

            var workspace = new AdhocWorkspace();
            OptionSet options = workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true);
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, true);
            SyntaxNode formattedNode = Formatter.Format(targetRoot, workspace, options);

            string outputPath = @"C:\Users\ante.novokmet\source\repos\SummaryAddApp\SummaryAddApp\Class1_transformed.cs";
            File.WriteAllText(outputPath, formattedNode.ToFullString());


            var folder = new DirectoryInfo("C:\\TFS\\WFM\\src\\GDi.W4.WebApi\\Controllers\\");
            var matcher = new Matcher();
            matcher.AddInclude("**/*.cs");
            var result = matcher.Execute(new DirectoryInfoWrapper(folder));
            foreach (var csFile in result.Files)
            {
                TransformFile(Path.Combine(folder.FullName, csFile.Path));
            }
        }

        public static void TransformFile(string sourcePath, string destinationPath = null)
        {
            destinationPath ??= sourcePath;
            Console.WriteLine($"Current: {sourcePath}");

            string sourceCode = File.ReadAllText(sourcePath);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = (CompilationUnitSyntax)tree.GetRoot();

            var commenter = new ConstructorParameterAdder();
            var targetRoot = commenter.Visit(root);

            var workspace = new AdhocWorkspace();
            OptionSet options = workspace.Options;
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInMethods, true);
            options = options.WithChangedOption(CSharpFormattingOptions.NewLinesForBracesInProperties, true);
            SyntaxNode formattedNode = Formatter.Format(targetRoot, workspace, options);

            string targetCode = formattedNode.ToFullString();
            File.WriteAllText(destinationPath, targetCode, System.Text.Encoding.UTF8);
        }
    }
}
