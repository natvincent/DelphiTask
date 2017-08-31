namespace Nant.Contrib.Tasks.Delphi
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using System;
    using System.IO;

    [TaskName("brcc")]
    public class BrccTask : DelphiTool
    {
        private FileInfo _file;
        private FileInfo _output;

        [TaskAttribute("file", Required=true)]
        public FileInfo File
        {
            get
            {
                return this._file;
            }
            set
            {
                this._file = value;
            }
        }

        [TaskAttribute("output", Required =false)]
        public FileInfo OutputFile
        {
            get { return _output; }
            set { _output = value; }
        }

        public override string ProgramArguments
        {
            get
            {
                this.Log(Level.Info, "Compiling resource '{0}'.", new object[] { this.File.FullName });

                string outputOption = "";
                if (_output != null && _output.FullName != "")
                {
                    outputOption = string.Format("-fo\"{0}\"", _output.FullName);
                }

                return string.Format("{0} {1}", GetFilename(this.File.FullName), outputOption);
            }
        }

        protected override string ToolName
        {
            get
            {
                if (Version == "1")
                {
                    return "brcc.exe";
                }
                else
                {
                    return "brcc32.exe";
                }
            }
        }
    }
}

