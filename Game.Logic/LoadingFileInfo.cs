using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Logic
{
    public class LoadingFileInfo
    {
        public int Type;
        public string Path;
        public string ClassName;

        public LoadingFileInfo(int type, string path, string className)
        {
            Type = type;
            Path = path;
            ClassName = className;
        }
    }
}
