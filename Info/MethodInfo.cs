using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text;

namespace TestGeneratorLib.Info
{
    public class MethodInfo
    {
        public string Name { get; private set; }
        public List<ParameterInfo> Parameters { get; private set; }
        public TypeSyntax ReturnType { get; private set; }

        public MethodInfo(string name, List<ParameterInfo> parameters, TypeSyntax returnType)
        {
            Name = name;
            Parameters = parameters;
            ReturnType = returnType;
        }
    }
}
