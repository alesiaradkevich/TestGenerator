using System;
using System.Collections.Generic;
using System.Text;

namespace TestGeneratorLib.Info
{
    public class ClassInfo
    {
        public string Name { get; private set; }
        public List<MethodInfo> Methods { get; private set; }

        public ClassInfo(string name, List<MethodInfo> methods)
        {
            Name = name;
            Methods = methods;
        }
    }
}
