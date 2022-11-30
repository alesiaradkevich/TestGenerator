using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestGeneratorLib.Info;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
namespace TestGeneratorLib
{
    public class TestGenerator
    {
        private List<string> SourceFiles;
        private string DestinationFolder;
        private ExecutionDataflowBlockOptions MaxFilesToLoad;
        private ExecutionDataflowBlockOptions MaxTasksToExecute;
        private ExecutionDataflowBlockOptions MaxFilesToWrite;

        public TestGenerator(List<string> sourceFiles, string destinationFolder,
            int maxFilesToLoad, int maxTasksToExecute, int maxFilesToWrite)
        {
            SourceFiles = sourceFiles;
            DestinationFolder = destinationFolder;
            MaxFilesToLoad = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxFilesToLoad };
            MaxFilesToWrite = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxFilesToWrite };
            MaxTasksToExecute = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxTasksToExecute };
        }

        public Task Generate()
        {
            var loadFiles = new TransformBlock<string, ClassFileInfo>
                (new Func<string, Task<ClassFileInfo>>(LoadContent), MaxFilesToLoad);
            var getTestClasses = new TransformBlock<ClassFileInfo, List<ClassFileInfo>>
                (new Func<ClassFileInfo, Task<List<ClassFileInfo>>>(GenerateTests), MaxTasksToExecute);
            var writeResult = new ActionBlock<List<ClassFileInfo>>
                (async input => { await FillTests(input); }, MaxFilesToWrite); 
            loadFiles.LinkTo(getTestClasses, new DataflowLinkOptions() { PropagateCompletion = true });
            getTestClasses.LinkTo(writeResult, new DataflowLinkOptions() { PropagateCompletion = true });

            foreach (var sourceFile in SourceFiles)
            {
                loadFiles.Post(sourceFile);
            }
            loadFiles.Complete();

            return writeResult.Completion;
        }

        private async Task<ClassFileInfo> LoadContent(string sourceFile)
        {
            string content;
            var reader = new StreamReader(new FileStream(sourceFile, FileMode.Open, FileAccess.Read));
            content = await reader.ReadToEndAsync();
            return new ClassFileInfo(sourceFile, content);
        }

        private async Task<List<ClassFileInfo>> GenerateTests(ClassFileInfo fileInfo)
        {
            var root = await CSharpSyntaxTree.ParseText(fileInfo.Content).GetRootAsync();
            return GenerateFromTree(root);
        }

        private List<ClassFileInfo> GenerateFromTree(SyntaxNode root)
        {
            var classes = new List<ClassDeclarationSyntax>(root.DescendantNodes().OfType<ClassDeclarationSyntax>());
            var usings = new List<UsingDirectiveSyntax>(root.DescendantNodes().OfType<UsingDirectiveSyntax>());
            var namespaces = new List<NamespaceDeclarationSyntax>(root.DescendantNodes().OfType<NamespaceDeclarationSyntax>());
            var nsInfo = new List<NsInfo>();
            foreach (var ns in namespaces)
            {
                var innerNsClasses = new List<ClassInfo>();
                foreach (var innerClass in classes)
                {
                    innerNsClasses.Add(new ClassInfo(innerClass.Identifier.ToString(), GetMethods(innerClass)));
                }
                nsInfo.Add(new NsInfo(ns.Name.ToString(), innerNsClasses));
            }
            return CodeGenerator.Generate(nsInfo, usings);
        }

        public List<MethodInfo> GetMethods(ClassDeclarationSyntax innerClass)
        {
            var methods = innerClass.DescendantNodes().OfType<MethodDeclarationSyntax>();
            var result = new List<MethodInfo>();

            foreach (var method in methods)
            {
                result.Add(new MethodInfo(method.Identifier.ToString(), GetParameters(method), method.ReturnType));
            }
            return result;
        }

        public List<ParameterInfo> GetParameters(MethodDeclarationSyntax method)
        {
            return method.ParameterList.Parameters.Select(param => new ParameterInfo(param.Identifier.Value.ToString(), param.Type)).ToList();
        }

        private async Task FillTests(List<ClassFileInfo> fileInfos)
        {

        }
    }

}
