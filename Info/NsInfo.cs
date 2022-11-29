using System;
using System.Collections.Generic;
using System.Text;

namespace TestGeneratorLib.Info
{
    public class NsInfo
    {
        public string Name { get; private set; }
        public List<ClassInfo> InnerClasses { get; private set; }

        public NsInfo(string name, List<ClassInfo> innerClasses)
        {
            Name = name;
            InnerClasses = innerClasses;
        }
    }
}
