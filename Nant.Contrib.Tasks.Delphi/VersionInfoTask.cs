using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using NAnt.Core.Attributes;
using NAnt.Core.Util;
using NAnt.Core;

namespace Delphi.Nant.Contrib.Tasks.Delphi
{
    public class VersionProperty : Element
    {
        private string _name = "";
        private string _value = "";
        private bool _ifDefined = true;
        private bool _unlessDefined;

        public VersionProperty()
        {

        }

        public VersionProperty(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this._name = name;
        }

        [TaskAttribute("name", Required = true), StringValidator(AllowEmpty = false)]
        public string PropertyName
        {
            get { return _name; }
            set { _name = value; }
        }

        [TaskAttribute("value", Required = true)]
        public string Value
        {
            get { return _value; }
            set { this._value = value; }
        }

        [TaskAttribute("if")]
        public bool IfDefined
        {
            get { return _ifDefined; }
            set { _ifDefined = value; }
        }

        [TaskAttribute("unless")]
        public bool UnlessDefined
        {
            get { return _unlessDefined; }
            set { _unlessDefined = value; }
        }

        public virtual object Clone()
        {
            VersionProperty clone = new VersionProperty();
            CopyTo(clone);
            return clone;
        }
    }

    [Serializable]
    public class VersionPropertyCollection : List<VersionProperty>
    {

    }

    [TaskName("versioninfo")]
    public class VersionInfoTask : Task
    {

        private Version _productVersion;
        private Version _fileVersion;
        private string _fileDescription;
        private string _companyName;
        private string _copyright;
        private string _trademarks;
        private string _originalFilename;
        private string _productName;
        private string _comments;
        private string _internalName;
        private string _privateBuild;
        private string _specialBuild;
        //private string fileFlags;
        private FileInfo _target;
        private bool _isDll;
        private bool _patchBuild;
        private bool _debugBuild;
        private bool _prereleaseBuild;
        private VersionPropertyCollection _properties = new VersionPropertyCollection();
        private bool _append = false;

        public bool Append
        {
            get { return _append; }
            set { _append = value; }
        }

        [TaskAttribute("productname", Required = false)]
        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }

        [BuildElement("productversion")]
        public Version ProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        [BuildElement("fileversion")]
        public Version FileVersion
        {
            get { return _fileVersion; }
            set { _fileVersion = value; }
        }

        [TaskAttribute("companyname", Required = false)]
        public string CompanyName
        {
            get { return _companyName; }
            set { _companyName = value; }
        }

        [TaskAttribute("description", Required = false)]
        public string Description
        {
            get { return _fileDescription; }
            set { _fileDescription = value; }
        }
        [TaskAttribute("copyright", Required = false)]
        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }
        [TaskAttribute("trademarks", Required = false)]
        public string Trademarks
        {
            get { return _trademarks; }
            set { _trademarks = value; }
        }

        [TaskAttribute("originalfilename", Required = false)]
        public string OriginalFilename
        {
            get { return _originalFilename; }
            set { _originalFilename = value; }
        }

        [TaskAttribute("comments", Required = false)]
        public string Comments
        {
            get { return _comments; }
            set { _comments = value; }
        }

        [TaskAttribute("internalname", Required = false)]
        public string InternalName
        {
            get { return _internalName; }
            set { _internalName = value; }
        }

        [TaskAttribute("target", Required = false)]
        [StringValidator(AllowEmpty = false)]
        public FileInfo Target
        {
            get { return _target; }
            set { _target = value; }
        }

       

        [TaskAttribute("isdll", Required = false)]
        public bool IsDLL
        {
            get { return _isDll; }
            set { _isDll = value; }
        }

        [TaskAttribute("patchbuild", Required = false)]
        public bool patchbuild 
        {
            get { return _patchBuild; }
            set { _patchBuild = value; }
        }

        [TaskAttribute("debugBuild", Required = false)]
        public bool debugBuild 
        {
            get { return _debugBuild; }
            set { _debugBuild = value; }
        }

        [TaskAttribute("specialbuild", Required = false)]
        public string specialBuild
        {
            get {return _specialBuild; }
            set {_specialBuild = value; }
        }

        [TaskAttribute("privateBuild", Required = false)]
        public string privateBuild
        {
            get {return _privateBuild; }
            set {_privateBuild = value; }
        }

        [TaskAttribute("prereleaseBuild", Required = false)]
        public bool prereleaseBuild
        {
            get {return _prereleaseBuild; }
            set {_prereleaseBuild = value; }
        }

        [BuildElementCollection("versionproperties", "versionproperty")]
        public VersionPropertyCollection properties
        {
            get { return _properties; }
        }
        /*[TaskAttribute("fileflags")]
        public string FileFlags
        {
            get { return _fileFlags; }
            set { _fileFlags = value; }
        }*/

        private void WriteProperty(StreamWriter writer, string Name, string Value)
        {
            if (!(Value == "") && !(Value == null))
            {
                writer.WriteLine(String.Format("{0,17} {1,-20} \"{2}\\0\"", "VALUE", "\"" + Name + "\",", Value));
            }
        }

        protected override void ExecuteTask()
        {
            if (String.IsNullOrEmpty(Target.FullName))
            {
                throw new ValidationException("Target filename is required for version information.");
            }

            if (!Target.Directory.Exists)
            {
                Target.Directory.Create();
                Target.Directory.Refresh();
            }

            using (StreamWriter writer = new StreamWriter(Target.FullName, _append, Encoding.Default))
            {
                int buildFlags = 0;
                
                if (_debugBuild) {
                    buildFlags = 1;
                }

                if (_prereleaseBuild) {
                    buildFlags = buildFlags | 0x00000002;
                }

                if (_patchBuild) {
                    buildFlags = buildFlags | 0x00000004;
                }

                if (!String.IsNullOrEmpty(_privateBuild)) {
                    buildFlags = buildFlags | 0x00000008;
                }

                if (!String.IsNullOrEmpty(_specialBuild)) {
                    buildFlags = buildFlags | 0x00000020;
                }

                writer.WriteLine(String.Format("{0, -20} {1}", "LANGUAGE", "0, SUBLANG_NEUTRAL"));
                writer.WriteLine(String.Format("{0, -20} {1}", "1", "VERSIONINFO"));
                writer.WriteLine(String.Format("{0, -20} {1}", "FILEVERSION", _fileVersion.GetString(',')));
                writer.WriteLine(String.Format("{0, -20} {1}", "PRODUCTVERSION", _productVersion.GetString(',')));
                writer.WriteLine(String.Format("{0, -20} 0x{1:X2}L", "FILEFLAGSMASK", buildFlags));
                writer.WriteLine(String.Format("{0, -20} 0x{1:X2}L", "FILEFLAGS", buildFlags));
                writer.WriteLine("BEGIN");
                writer.WriteLine("    BLOCK \"StringFileInfo\"");
                writer.WriteLine("    BEGIN");
                writer.WriteLine("        BLOCK \"040904e4\"");
                writer.WriteLine("        BEGIN");
                WriteProperty(writer, "CompanyName", _companyName);
                WriteProperty(writer, "FileDescription", _fileDescription);
                WriteProperty(writer, "FileVersion", _fileVersion.GetStringOrText());
                WriteProperty(writer, "InternalName", _internalName);
                WriteProperty(writer, "LegalCopyright", _copyright);
                WriteProperty(writer, "LegalTrademarks", _trademarks);
                WriteProperty(writer, "OriginalFilename", _originalFilename);
                WriteProperty(writer, "ProductName", _productName);
                WriteProperty(writer, "ProductVersion", _productVersion.GetStringOrText());
                if (_specialBuild != string.Empty) {
                    WriteProperty(writer, "SpecialBuild", _specialBuild);
                }
                if (_privateBuild != string.Empty) {
                    WriteProperty(writer, "PrivateBuild", _privateBuild);
                }

                foreach (var prop in _properties)
                {
                    if (prop.IfDefined && !prop.UnlessDefined)
                    {
                        WriteProperty(writer, prop.PropertyName, prop.Value);
                    }
                }

                writer.WriteLine("        END");
                writer.WriteLine("    END");
                writer.WriteLine("    BLOCK \"VarFileInfo\"");
                writer.WriteLine("    BEGIN");
                writer.WriteLine("        VALUE \"Translation\", 0x409 0x04E4");
                writer.WriteLine("    END");
                writer.WriteLine("END");

                /*int tempfileflags = 0;

                tempfileflags = 0;*/



            }
        }
    }
}
