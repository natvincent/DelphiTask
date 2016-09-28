using NAnt.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;

namespace Nant.Contrib.Tasks.Delphi
{
    [FunctionSet("delphi", "Delphi")]
    class DelphiFunctions : NAnt.Core.FunctionSetBase
    {
        public DelphiFunctions(Project project, PropertyDictionary properties) : base(project, properties) { }

        [Function("path")]    
        public static string DelphiPath(string version = "")
        {
            DelphiFinder finder = new DelphiFinder(version);
            return finder.DelphToolDir;
        }    
    }
}
