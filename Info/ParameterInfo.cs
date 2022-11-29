using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Text;

namespace TestGeneratorLib.Info
{
    public class ParameterInfo
    {
        public string Name { get; private set; }
        public TypeSyntax Type { get; private set; }

        public ParameterInfo (string name, TypeSyntax type)
        {
            Name = name;
            Type = type;
        }
    }
}
