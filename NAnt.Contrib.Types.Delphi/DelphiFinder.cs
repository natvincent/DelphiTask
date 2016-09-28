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

namespace Nant.Contrib.Tasks.Delphi
{
    public class DelphiFinder 
    {
        private const string BORLAND_BDS_REG_PATH = @"SOFTWARE\Borland\BDS\";
        private const string CODEGEAR_BDS_REG_PATH = @"SOFTWARE\CodeGear\BDS\";
        private const string BORLAND_DELPHI_REG_PATH = @"SOFTWARE\Borland\Delphi\";
        private const string EMBARCADERO_BDS_REG_PATH = @"SOFTWARE\Embarcadero\BDS\";

        private string _versionString = String.Empty;
        private string _toolPath = String.Empty;

        private Dictionary<string, string> _versionRegistryPaths = new Dictionary<string, string>();

        public DelphiFinder(string versionString, string toolPath)
        {
            _versionString = versionString;
            _toolPath = toolPath;

            _versionRegistryPaths["2"] = BORLAND_DELPHI_REG_PATH + @"2.0\";
            _versionRegistryPaths["3"] = BORLAND_DELPHI_REG_PATH + @"3.0\";
            _versionRegistryPaths["4"] = BORLAND_DELPHI_REG_PATH + @"4.0\";
            _versionRegistryPaths["5"] = BORLAND_DELPHI_REG_PATH + @"5.0\";
            _versionRegistryPaths["6"] = BORLAND_DELPHI_REG_PATH + @"6.0\";
            _versionRegistryPaths["7"] = BORLAND_DELPHI_REG_PATH + @"7.0\";
            //_versionRegistryPaths["8"] = "NOT SUPPORTED!!!";
            _versionRegistryPaths["2005"] = BORLAND_BDS_REG_PATH + @"3.0\";
            _versionRegistryPaths["2006"] = BORLAND_BDS_REG_PATH + @"4.0\";
            _versionRegistryPaths["2007"] = BORLAND_BDS_REG_PATH + "5.0\\";
            _versionRegistryPaths["2009"] = CODEGEAR_BDS_REG_PATH + "6.0\\";
            _versionRegistryPaths["2010"] = CODEGEAR_BDS_REG_PATH + "7.0\\";
            _versionRegistryPaths["XE"] = EMBARCADERO_BDS_REG_PATH + "8.0\\";
            _versionRegistryPaths["XE2"] = EMBARCADERO_BDS_REG_PATH + "9.0\\";
            _versionRegistryPaths["XE3"] = EMBARCADERO_BDS_REG_PATH + "10.0\\";
            _versionRegistryPaths["XE4"] = EMBARCADERO_BDS_REG_PATH + "11.0\\";
            _versionRegistryPaths["XE5"] = EMBARCADERO_BDS_REG_PATH + "12.0\\";
            _versionRegistryPaths["XE6"] = EMBARCADERO_BDS_REG_PATH + "14.0\\";
            _versionRegistryPaths["XE7"] = EMBARCADERO_BDS_REG_PATH + "15.0\\";

        }

        private string AbsoluteToRelativePath(string mainDirPath, string absoluteFilePath)
        {
            string[] firstPathParts = mainDirPath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] secondPathParts = absoluteFilePath.Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            int sameCounter = 0;

            for (int i = 0; i < Math.Min(firstPathParts.Length, secondPathParts.Length); i++)
            {
                if (!firstPathParts[i].ToLower().Equals(secondPathParts[i].ToLower()))
                {
                    break;
                }
                sameCounter++;
            }

            if (sameCounter == 0)
            {
                return absoluteFilePath;
            }

            string newPath = String.Empty;

            for (int i = sameCounter; i < firstPathParts.Length; i++)
            {
                if (i > sameCounter)
                {
                    newPath += Path.DirectorySeparatorChar;
                }
                newPath += "..";
            }
            if (newPath.Length == 0)
            {
                newPath = ".";
            }

            for (int i = sameCounter; i < secondPathParts.Length; i++)
            {
                newPath += Path.DirectorySeparatorChar;
                newPath += secondPathParts[i];
            }

            Log(Level.Verbose, string.Format("AbsoluteToRelative: mainDirPath {0} absoluteFilePath {1} result {2}", mainDirPath, absoluteFilePath, newPath));

            return newPath;
        }

        private string searchPath()
        {
            string originalPathEnv = Environment.GetEnvironmentVariable("PATH");
            string[] paths = originalPathEnv.Split(new char[1] { Path.PathSeparator });
            foreach (string s in paths)
            {
                string pathEnv = Environment.ExpandEnvironmentVariables(s) + @"\" + ToolName;
                Log(Level.Info, string.Format("searching in {0}", pathEnv));
                if (pathEnv.Length > 0 && File.Exists(pathEnv))
                {
                    Log(Level.Verbose, "found");
                    return pathEnv;
                }
            }
            return string.Empty;
        }

        private string findExecutable()
        {
            if (Version != "1")
            {
                string toolDir = this.DelphToolDir;
                if (toolDir != null)
                {
                    if (!toolDir.EndsWith(@"\"))
                    {
                        toolDir += @"\";
                    }
                    toolDir = toolDir + @"bin\" + this.ToolName;
                    if (File.Exists(toolDir))
                    {
                        return toolDir;
                    }
                }
                throw new BuildException(string.Format("Unable to locate Delphi tool \"{0}\" (looking in {1})", this.ToolName, toolDir), this.Location);
            }
            else
            {
                return searchPath();
            }
        }

        private string getDelphiRootPath(RegistryKey key)
        {
            Object value = key.GetValue("RootDir");
            if (value == null)
            {
                const string formatString = "The RootDir value in registry key {0} doesn't exist.\r\n" +
                                      "Have your run Delphi after installing it?\r\n" +
                                      "(for Delphi 2 you may need to add this value manually)";
                throw new BuildException(string.Format(formatString, key.Name), this.Location);
            }
            return key.GetValue("RootDir").ToString();
        }

        private string highestVersionPathIn(string lookIn, int higherThan)
        {
            RegistryKey parentkey = Registry.LocalMachine.OpenSubKey(lookIn, false);
            if (parentkey == null)
            {
                parentkey = Registry.CurrentUser.OpenSubKey(lookIn, false);
            }
            if (parentkey != null)
            {
                string[] subKeyNames = parentkey.GetSubKeyNames();
                Array.Sort(subKeyNames);
                Array.Reverse(subKeyNames);
                int index = 0;
                while (!IsNumeric(subKeyNames[index]) && (index < (subKeyNames.Length - 1)))
                {
                    index++;
                }
                this.Log(Level.Debug, "Highest sub key = '{0}'.", new object[] { subKeyNames[index] });
                if ((subKeyNames.Length > 0) && (Convert.ToDouble(subKeyNames[index]) >= higherThan))
                {
                    RegistryKey key = parentkey.OpenSubKey(subKeyNames[index]);
                    if (key != null)
                    {
                        return this.getDelphiRootPath(key);
                    }
                    return null;
                }
                return null;
            }
            return null;
        }

        public static bool IsNumeric(object Expression)
        {
            double subKeyNames;
            return double.TryParse(Convert.ToString(Expression), NumberStyles.Any, (IFormatProvider)NumberFormatInfo.InvariantInfo, out subKeyNames);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetShortPathName(

                 [MarshalAs(UnmanagedType.LPTStr)]

                   string path,

                 [MarshalAs(UnmanagedType.LPTStr)]

                   StringBuilder shortPath,

                 int shortPathLength

                 );


        private string GetShortFilename(string filename)
        {
            StringBuilder result = new StringBuilder(255);
            GetShortPathName(filename, result, result.Capacity);
            if (result.ToString() == string.Empty)
            {
                return filename;
            }
            else
            {
                return result.ToString();
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetEnvironmentVariable(
                 [MarshalAs(UnmanagedType.LPTStr)]
                 string lpName,
                 [MarshalAs(UnmanagedType.LPTStr)]
                 string lpValue);

        protected string GetFilename(string AFilename)
        {
            if (Version == "1")
            {
                string relPath = AbsoluteToRelativePath(BaseDirectory.FullName, AFilename);
                StringBuilder result = new StringBuilder(255);
                GetShortPathName(relPath, result, result.Capacity);
                Log(Level.Debug, string.Format("GetFilename AFilename: {0} relPath: {1} result: {2}", AFilename, relPath, result.ToString()));
                if (result.ToString() == string.Empty)
                {
                    return relPath;
                }
                else
                {
                    return result.ToString();
                }
            }
            else
            {
                string newFilename = AFilename.TrimEnd(Path.DirectorySeparatorChar);
                return '"' + newFilename + '"';
            }
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
                    string directory = null;
                    if (this._versionString == String.Empty)
                    {
                        directory = this.highestVersionPathIn(BORLAND_BDS_REG_PATH, 3);
                        if (directory == null)
                        {
                            directory = this.highestVersionPathIn(BORLAND_DELPHI_REG_PATH, 3);
                            if (directory == null)
                            {
                                directory = this.highestVersionPathIn(CODEGEAR_BDS_REG_PATH, 3);
                                if (directory == null)
                                {
                                    directory = this.highestVersionPathIn(EMBARCADERO_BDS_REG_PATH, 3);
                                }
                            }
                        }
                    }
                    else if (this._versionString == @"1")
                    {
                        directory = string.Empty;
                    }
                    else
                    {
                        string rootKey;

                        if (!_versionRegistryPaths.TryGetValue(_versionString, out rootKey))
                        {
                            throw new BuildException(string.Format("Delphi version {0} not supported", this._versionString), this.Location);
                        }
                        else
                        {
                            RegistryKey key = Registry.CurrentUser.OpenSubKey(rootKey, false);
                            if (key == null)
                            {
                                throw new BuildException(string.Format("Delphi version {0} not found in registry", this._versionString), this.Location);
                            }
                            directory = this.getDelphiRootPath(key);
                        }
                    }
                    this.Log(Level.Debug, "Using '{0}'.", new object[] { directory });
                    return directory;
                }
            }
        }

    }

    private string ShortenPathVariable(string oldPath)
    {
        string[] paths = oldPath.Split(Path.PathSeparator);
        StringBuilder builder = new StringBuilder();

        foreach (string path in paths)
        {
            builder.Append(GetShortFilename(path));
            builder.Append(Path.PathSeparator);
        }

        return builder.ToString();
    }

}
