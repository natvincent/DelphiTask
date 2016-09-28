namespace Nant.Contrib.Tasks.Delphi
{
    using Microsoft.Win32;
    using NAnt.Core;
    using NAnt.Core.Attributes;

    using NAnt.Core.Tasks;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;

    using Nant.Contrib.Tasks.Delphi;

    public class DelphiTool : ExternalProgramBase
    {

        private string _versionString = String.Empty;
        private string _toolPath = String.Empty;

        public override string ProgramArguments
        {
            get
            {
                return string.Empty;
            }
        }

        public override string ProgramFileName
        {
            get
            {
                return this.findExecutable();
            }
        }

        protected virtual string ToolName
        {
            get
            {
                return string.Empty;
            }
        }

        protected override void PrepareProcess(Process process)
        {
            base.PrepareProcess(process);
            Log(Level.Debug, string.Format(@"Path: {0}", process.StartInfo.EnvironmentVariables[@"PATH"]));
            Log(Level.Debug, string.Format(@"Working Directory: {0}", process.StartInfo.WorkingDirectory));
            Log(Level.Debug, string.Format(@"Filename: {0}", process.StartInfo.FileName));
        }

        [TaskAttribute("version")]
        [StringValidator(
            AllowEmpty = true,
            Expression = "2|3|4|5|6|7|2005|2006|2007|2009|2010|XE|XE2|XE3|XE4|XE5|XE6|XE7|XE8|Seattle|Berlin",
            ExpressionErrorMessage = "Must be an empty string or one of 'detailed', 'public', 'segments' or 'off'")]
        public string Version
        {
            get
            {
                return _versionString;
            }
            set
            {
                this._versionString = value;

            }
        }

        [TaskAttribute("toolpath", Required = false)]
        public string ToolPath
        {
            get
            {
                return _toolPath;
            }
            set
            {
                _toolPath = value;
            }
        }

        private string findExecutable()
        {
            DelphiFinder finder = new DelphiFinder(_versionString, _toolPath, BaseDirectory);

            string toolPath;

            if (!finder.findExecutable(this.ToolName, out toolPath))
            {
                throw new BuildException(string.Format("Unable to locate Delphi tool \"{0}\" (looking in {1})", ToolName, toolPath));
            }

            this.Log(Level.Debug, "Using '{0}'.", new object[] { toolPath });

            return toolPath;
        }


        protected string GetFilename(string AFilename)
        {
            string newFilename = AFilename.TrimEnd(Path.DirectorySeparatorChar);
            return '"' + newFilename + '"';
        }

        public string DelphToolDir
        {
            get
            {
                if (_toolPath != String.Empty)
                {
                    return _toolPath;
                }
                else
                {
                    DelphiFinder finder = new DelphiFinder(_versionString, _toolPath, BaseDirectory);

                    return finder.DelphToolDir;
                }
            }
        }

    }
}