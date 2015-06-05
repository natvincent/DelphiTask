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

        public override string ProgramArguments
        {
            get
            {
                this.Log(Level.Info, "Compiling resource '{0}'.", new object[] { this.File.FullName });
                return GetFilename(this.File.FullName);
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

