using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using NAnt.Core.Attributes;
using NAnt.Core.Util;
using NAnt.Core;
using Nant.Contrib.Tasks.Delphi;

namespace Delphi.Nant.Contrib.Tasks.Delphi
{
    enum ResourceType { ICON, RCDATA, CURSOR, BITMAP, FONT, CUSTOM };  

    class ResourceEntry : Element
    {
        private string _name;
        private ResourceType _type;
        private FileInfo _file;
        private string _customType;

        [TaskAttribute("name", Required = true)]
        public new string Name
        {
            get { return _name;  }
            set { _name = value; }
        }

        [TaskAttribute("type", Required = true)]
        public ResourceType Type
        {
            get { return _type;  }
            set { _type = value; }
        }

        [TaskAttribute("customtype", Required = false)]
        public string CustomType
        {
            get { return _customType; }
            set { _customType = value; }
        }

        [TaskAttribute("file", Required = true)]
        public FileInfo File
        {
            get { return _file;  }
            set { _file = value; }
        }

        override public string ToString()
        {
            string type = _type.ToString();
            if (_type == ResourceType.CUSTOM)
            {
                type = _customType;
            }
            return string.Format("{0} {1} {2}", _name, type, string.Format("\"{0}\"", _file.FullName));
        }
    }

    class ResourceList : List<ResourceEntry> { }

    [TaskName("buildresource")]
    class BuildResourceTask : DelphiTool
    {
        private VersionInfoTask _versionInfo = null;// new VersionInfoTask();
        private ResourceList _resources = new ResourceList();
        private FileInfo _resourceScript;

        [TaskAttribute("file", Required = true)]
        public FileInfo ResourceFile
        {
            get { return _resourceScript; }
            set { _resourceScript = value; }
        }

        [BuildElement("versioninfo")]
        public VersionInfoTask VersionInfo
        {
            get { return _versionInfo; }
            set { _versionInfo = value;  }
        }

        [BuildElementArray("resource")]
        public ResourceList Resources
        {
            get { return _resources; }
        }

        public override string ProgramArguments
        {
            get
            {
                FileInfo target = new FileInfo(Path.ChangeExtension(_resourceScript.FullName, "rc"));

                if (!target.Directory.Exists)
                {
                    target.Directory.Create();
                    target.Directory.Refresh();
                }

                using (StreamWriter writer = new StreamWriter(target.FullName, false, Encoding.Default))
                {
                    foreach (ResourceEntry resource in _resources)
                    {
                        writer.WriteLine(resource.ToString());
                    }
                }

                if (_versionInfo != null)
                {
                    _versionInfo.Target = target;
                    _versionInfo.Append = true;
                    _versionInfo.Project = Project;
                    _versionInfo.Execute();
                }

                return string.Format("\"{0}\"", target.FullName);

            }
        }

        protected override string ToolName
        {
            get
            {
                return "brcc32.exe";
            }
        }

    }
}
