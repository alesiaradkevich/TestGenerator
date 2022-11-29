using System;
using System.Collections.Generic;
using System.Text;

namespace TestGeneratorLib.Info
{
    public class ClassFileInfo
    {
        public string Name { get; private set; }
        public string Content { get; private set; }
        public ClassFileInfo(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
