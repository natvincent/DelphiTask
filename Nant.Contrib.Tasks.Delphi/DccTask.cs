namespace Nant.Contrib.Tasks.Delphi
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NAnt.Core.Types;
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    [TaskName("dcc")]
    public class DccTask : DelphiTool
    {
        private bool _build = false;
        private CompilerOptionCollection _compileroptions = new CompilerOptionCollection();
        private DirectoryInfo _dcuOutput;
        private DefineCollection _defines = new DefineCollection();
        private NameSpaceCollection _nameSpaces = new NameSpaceCollection();
        private DirectoryInfo _exeOutput;
        private DirectoryInfo _bplOutput;
        private DirectoryInfo _dcpOutput;
        private FileInfo _source;
        private UnitPath _unitpath = new UnitPath();
        private bool _console = false;
        private bool _quiet = true;
        private bool _writeCfg = false;
        private string _mapFile;
        private WarningCollection _warnings = new WarningCollection();
        private string _debugInfo;
        private bool _debugExe = false;
        private bool _debugNameSpace = false;
        private bool _debugRemote = false;
        private bool _debugTds = false;
        private PackageCollection _packages = new PackageCollection();

        private static string[] CValidWarnings = new string[]
            {
                "ASG_TO_TYPED_CONST",
                "BAD_GLOBAL_SYMBOL",
                "BOUNDS_ERROR",
                "CASE_LABEL_RANGE",
                "COMBINING_SIGNED_UNSIGNED",
                "COMPARING_SIGNED_UNSIGNED",
                "COMPARISON_FALSE",
                "COMPARISON_TRUE",
                "CONSTRUCTING_ABSTRACT",
                "CVT_ACHAR_TO_WCHAR",
                "CVT_NARROWING_STRING_LOST",
                "CVT_WCHAR_TO_ACHAR",
                "CVT_WIDENING_STRING_LOST",
                "DUPLICATE_CTOR_DTOR",
                "DUPLICATES_IGNORED",
                "EXPLICIT_STRING_CAST",
                "EXPLICIT_STRING_CAST_LOSS",
                "FILE_OPEN",
                "FILE_OPEN_UNITSRC",
                "FOR_LOOP_VAR_UNDEF",
                "FOR_LOOP_VAR_VARPAR",
                "FOR_VARIABLE",
                "GARBAGE",
                "HIDDEN_VIRTUAL",
                "HIDING_MEMBER",
                "HPPEMIT_IGNORED",
                "HRESULT_COMPAT",
                "IMAGEBASE_MULTIPLE",
                "IMPLICIT_IMPORT",
                "IMPLICIT_STRING_CAST",
                "IMPLICIT_STRING_CAST_LOSS",
                "IMPLICIT_VARIANTS",
                "INVALID_DIRECTIVE",
                "LOCAL_PINVOKE",
                "LOCALE_TO_UNICODE",
                "MESSAGE_DIRECTIVE",
                "NO_CFG_FILE_FOUND",
                "NO_RETVAL",
                "OPTION_TRUNCATED",
                "PACKAGE_NO_LINK",
                "PACKAGED_THREADVAR",
                "PRIVATE_PROPACCESSOR",
                "RLINK_WARNING",
                "STRING_CONST_TRUNCED",
                "SUSPICIOUS_TYPECAST",
                "SYMBOL_DEPRECATED",
                "SYMBOL_EXPERIMENTAL",
                "SYMBOL_LIBRARY",
                "SYMBOL_PLATFORM",
                "TYPED_CONST_VARPAR",
                "TYPEINFO_IMPLICITLY_ADDED",
                "UNICODE_TO_LOCALE",
                "UNIT_DEPRECATED",
                "UNIT_EXPERIMENTAL",
                "UNIT_INIT_SEQ",
                "UNIT_LIBRARY",
                "UNIT_NAME_MISMATCH",
                "UNIT_PLATFORM",
                "UNSAFE_CAST",
                "UNSAFE_CODE",
                "UNSAFE_TYPE",
                "UNSUPPORTED_CONSTRUCT",
                "USE_BEFORE_DEF",
                "WIDECHAR_REDUCED",
                "XML_CREF_NO_RESOLVE",
                "XML_EXPECTED_CHARACTER",
                "XML_INVALID_NAME",
                "XML_INVALID_NAME_START",
                "XML_NO_MATCHING_PARM",
                "XML_NO_PARM",
                "XML_UNKNOWN_ENTITY",
                "XML_WHITESPACE_NOT_ALLOWED",
                "ZERO_NIL_COMPAT"
            };

        [TaskAttribute("build")]
        public bool Build
        {
            get
            {
                return this._build;
            }
            set
            {
                this._build = value;
            }
        }

        [TaskAttribute("dcuoutput")]
        public DirectoryInfo DcuOutput
        {
            get
            {
                return this._dcuOutput;
            }
            set
            {
                this._dcuOutput = value;
            }
        }

        [BuildElementCollection("defines", "define")]
        public DefineCollection Defines
        {
            get
            {
                return this._defines;
            }
        }

        [BuildElementCollection("namespaces", "namespace")]
        public NameSpaceCollection NameSpaces
        {
            get
            {
                return this._nameSpaces;
            }
        }

        [TaskAttribute("exeoutput")]
        public DirectoryInfo ExeOutput
        {
            get
            {
                return this._exeOutput;
            }
            set
            {
                this._exeOutput = value;
            }
        }

        [TaskAttribute("bploutput")]
        public DirectoryInfo BplOutput
        {
            get
            {
                return this._bplOutput;
            }
            set
            {
                this._bplOutput = value;
            }
        }

        [TaskAttribute("dcpoutput")]
        public DirectoryInfo DcpOutput
        {
            get
            {
                return this._dcpOutput;
            }
            set
            {
                this._dcpOutput = value;
            }
        }

        [TaskAttribute("console", Required = false)]
        [BooleanValidator()]
        public Boolean Console
        {
            get
            {
                return _console;
            }
            set
            {
                _console = value;
            }
        }

        [TaskAttribute("mapfile", Required = false)]
        [StringValidator(
            AllowEmpty = true,
            Expression = "off|detailed|publics|segments",
            ExpressionErrorMessage = "Must be an empty string or one of 'detailed', 'public', 'segments' or 'off'")]
        public string MapFile
        {
            get
            {
                return _mapFile;
            }
            set
            {
                _mapFile = value;
            }
        }

        [TaskAttribute("writecfg", Required = false)]
        [BooleanValidator()]
        public Boolean WriteCfg
        {
            get
            {
                return _writeCfg;
            }
            set
            {
                _writeCfg = value;
            }
        }

        [TaskAttribute("quiet", Required = false)]
        [BooleanValidator()]
        public Boolean Quiet
        {
            get
            {
                return _quiet;
            }
            set
            {
                _quiet = value;
            }
        }

        [TaskAttribute("debuginfo", Required = false)]
        [StringValidator(AllowEmpty = false)] 
        public string DebugInfo
        {
            get
            {
                return _debugInfo;
            }
            set
            {
                string[] array = value.Split(',');
                foreach (string option in array)
                {
                    if (String.Compare("exe", option, true) == 0)
                    {
                        _debugExe = true;
                    }
                    else if (String.Compare("namespace", option, true) == 0)
                    {
                        _debugNameSpace = true;
                    }
                    else if (String.Compare("remote", option, true) == 0)
                    {
                        _debugRemote = true;
                    }
                    else if (String.Compare("exe", option, true) == 0)
                    {
                        _debugTds = true;
                    }
                    else 
                    {
                        throw new ValidationException(string.Format("{0} is not a valid option for debuginfo", option));
                    }
                }
                _debugInfo = value;
            }
        }
        
        [BuildElementCollection("compileroptions", "option")]
        public CompilerOptionCollection Options
        {
            get
            {
                return this._compileroptions;
            }
        }

        [BuildElementCollection("runtimepackages", "package")]
        public PackageCollection Packages
        {
            get
            {
                return this._packages;
            }
        }

        private StringCollection BuildParameters()
        {
            StringCollection parameters = new StringCollection();

            foreach (CompilerOption option in this._compileroptions)
            {
                if ((("ABDEFGIKLMNOPQRSTUVWXYZ".IndexOf(option.OptionName) <= -1)) && ("BCDGHIJLMOPQRTUVWXY".IndexOf(option.OptionName) <= -1))
                {
                    throw new BuildException(string.Format("Compiler option {0} not supported", option.OptionName), this.Location);
                }

                parameters.Add(String.Format("-${0}{1}", option.OptionName, option.State));
            }

            if (this._exeOutput != null)
            {
                parameters.Add(String.Format("-E{0}", GetFilename(_exeOutput.ToString())));
            }

            if (this._dcuOutput != null)
            {
                string paramFormat = "-N0{0}";

                if ("2 3 4 5 6 7".IndexOf(Version) != -1)
                {
                    paramFormat = "-N{0}";
                }

                parameters.Add(String.Format(paramFormat, GetFilename(this._dcuOutput.ToString())));
            }

            if (this._bplOutput != null)
            {
                parameters.Add(String.Format("-LE{0}", GetFilename(this._bplOutput.ToString())));
            }

            if (this._dcpOutput != null)
            {
                parameters.Add(String.Format("-LN{0}", GetFilename(this._dcpOutput.ToString())));
            }

            if (this._build)
            {
                parameters.Add("-B");
            }

            if (_console)
            {
                parameters.Add("-CC");
            }

            if (_quiet)
            {
                parameters.Add("-Q");
            }

            if (_mapFile == @"detailed")
            {
                parameters.Add(@"-GD");
            }
            else if (_mapFile == @"publics")
            {
                parameters.Add(@"-GP");
            }
            else if (_mapFile == @"segments")
            {
                parameters.Add(@"-GS");
            }

            if (_debugExe)
            {
                parameters.Add(@"-V");
            }
            
            if (_debugNameSpace)
            {
                parameters.Add(@"-VN");
            }
            
            if (_debugRemote)
            {
                parameters.Add(@"-VR");
            }
            
            if (_debugTds)
            {
                parameters.Add(@"-VT");
            }

            foreach (Package pkg in _packages)
            {
                parameters.Add(String.Format("-LU{0}", pkg.PackageName));
            }

            foreach (Warning warning in _warnings)
            {
                //foreach (string validWarning in CValidWarnings)
                //{
                //    if (String.Compare(validWarning, warning.WarningName, true) != 0)
                //    {
                //        throw new BuildException(String.Format("{0} is not a valid warning", warning.WarningName));
                //    }
                //}
                parameters.Add(String.Format("-W{0}{1}", warning.State, warning.WarningName));
            }

            foreach (Define define in this._defines)
            {
                parameters.Add(String.Format("-D{0}", define.DefineName));
            }

            if (_nameSpaces.Count > 0)
            {
                StringBuilder nameSpacePath = new StringBuilder();
                foreach (NameSpace nameSpace in this._nameSpaces)
                {
                    nameSpacePath.AppendFormat("{0};", nameSpace.NameSpaceName);
                }
                parameters.Add(String.Format("-NS{0}", nameSpacePath.ToString()));
            }

            StringBuilder searchPath = new StringBuilder();
            this.Log(Level.Debug, string.Format("There are {0} lib paths", this._unitpath.DirectoryNames.Count));

            if (_unitpath.IncludeDelphiLib) 
            {
                string libPath = Path.Combine(DelphToolDir, @"lib");
                string platformLibPath = Path.Combine(libPath, @"win32");
                if (Directory.Exists(platformLibPath)) 
                {
                    libPath = platformLibPath;
                }
                string releaseLibPath = Path.Combine(libPath, @"release");
                if (Directory.Exists(releaseLibPath)) 
                {
                    libPath = releaseLibPath;
                }
                searchPath.AppendFormat("{0};", libPath);
            }

            foreach (string file in this._unitpath.DirectoryNames)
            {
                this.Log(Level.Debug, "Adding lib path: " + file);
                searchPath.AppendFormat("{0};", file);
            }

            if (searchPath.Length > 0)
            {
                parameters.Add(String.Format("-U\"{0}\"", searchPath.ToString()));
                parameters.Add(String.Format("-I\"{0}\"", searchPath.ToString()));
                if (Version != "1")
                {
                    parameters.Add(String.Format("-O\"{0}\"", searchPath.ToString()));
                    parameters.Add(String.Format("-R\"{0}\"", searchPath.ToString()));
                }

            }

            return parameters;
        }

        public override string ProgramArguments
        {
            get
            {
                this.Log(Level.Info, "Building project '{0}'.", new object[] { this.Source.FullName });
                if (this._unitpath.BaseDirectory == null)
                {
                    this._unitpath.BaseDirectory = new DirectoryInfo(this.Project.BaseDirectory);
                }

                StringBuilder commandLine = new StringBuilder();

                commandLine.Append(" " + GetFilename(this.Source.FullName) + " ");

                if (_writeCfg)
                {
                    StreamWriter writer = new StreamWriter(Path.ChangeExtension(Source.FullName, ".cfg"), false);

                    this.Log(Level.Debug, "CFG file contains:");
                    foreach (string parameter in BuildParameters())
                    {
                        writer.WriteLine(String.Format("{0}", parameter));
                        this.Log(Level.Debug, parameter);
                    }
                    writer.Close();
                }
                else
                {
                    foreach (string parameter in BuildParameters())
                    {
                        commandLine.AppendFormat(" {0}", parameter);
                    }
                }
                this.Log(Level.Debug, "DCC Command line: {0}", new object[] { commandLine.ToString() });
                return commandLine.ToString();
            }
        }

        [BuildElementCollection("warnings", "warning")]
        public WarningCollection Warnings
        {
            get
            {
                return _warnings;
            }
            set
            {
                _warnings = value;
            }
        }

        [TaskAttribute("source", Required=true)]
        public FileInfo Source
        {
            get
            {
                return this._source;
            }
            set
            {
                this._source = value;
            }
        }

        protected override string ToolName
        {
            get
            {
                if (Version == "1")
                { return "dcc.exe"; }
                else
                { return "dcc32.exe"; };
            }
        }

        [BuildElement("unitpath")]
        public UnitPath UnitPath
        {
            get
            {
                return this._unitpath;
            }
            set
            {
                this._unitpath = value;
            }
        }
    }
}

