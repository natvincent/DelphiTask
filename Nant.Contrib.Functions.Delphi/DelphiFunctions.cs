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
        public string DelphiPath(string version = "")
        {
            DelphiFinder finder = new DelphiFinder(version, 0, Project);
            return finder.DelphToolDir;
        }  
        
        [Function("bds-path")]  
        public string BDSPath(int version)
        {
            DelphiFinder finder = new DelphiFinder("", version, Project);
            return finder.DelphToolDir;
        }
    }
}
