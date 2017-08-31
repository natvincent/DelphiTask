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
        private const string SOFTWARE_ROOT_PATH = @"SOFTWARE\";
        private const string SOFTWARE_32_BIT_ON_64_BIT_PATH = @"Wow6432Node\";
        private const string BORLAND_BDS_REG_PATH = @"Borland\BDS\";
        private const string CODEGEAR_BDS_REG_PATH = @"CodeGear\BDS\";
        private const string BORLAND_DELPHI_REG_PATH = @"Borland\Delphi\";
        private const string EMBARCADERO_BDS_REG_PATH = @"Embarcadero\BDS\";

        private string _versionString = String.Empty;
        private string _toolPath = String.Empty;
        private DirectoryInfo _basePath;
        private Project _project = null;

        private Dictionary<string, string> _versionRegistryPaths = new Dictionary<string, string>();

        public DelphiFinder(string versionString, Project project = null) : this(versionString, "", null, project) {}

        public DelphiFinder(string versionString, string toolPath, DirectoryInfo basePath, Project project = null)
        {
            _versionString = versionString;
            _toolPath = toolPath;
            _basePath = basePath;
            _project = project;

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
            _versionRegistryPaths["XE8"] = EMBARCADERO_BDS_REG_PATH + "16.0\\";
            _versionRegistryPaths["Seattle"] = EMBARCADERO_BDS_REG_PATH + "17.0\\";
            _versionRegistryPaths["Berlin"] = EMBARCADERO_BDS_REG_PATH + "18.0\\";
        }

        public void Log(Level messageLevel, string format)
        {
            if (_project != null)
            {
                _project.Log(messageLevel, format);
            }
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

            return newPath;
        }

        private string searchPath(string toolName)
        {
            string originalPathEnv = Environment.GetEnvironmentVariable("PATH");
            string[] paths = originalPathEnv.Split(new char[1] { Path.PathSeparator });
            foreach (string s in paths)
            {
                string pathEnv = Environment.ExpandEnvironmentVariables(s) + @"\" + toolName;
                if (pathEnv.Length > 0 && File.Exists(pathEnv))
                {
                    return pathEnv;
                }
            }
            return string.Empty;
        }

        public bool findExecutable(string toolName, out string path)
        {
            path = this.DelphToolDir;
            if (path != null)
            {
                if (!path.EndsWith(@"\"))
                {
                    path += @"\";
                }
                path = path + @"bin\" + toolName;
                return File.Exists(path);
            }
            else
            {
                path = searchPath(toolName);
                return !string.IsNullOrEmpty(path);
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
                throw new BuildException(string.Format(formatString, key.Name));
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

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetEnvironmentVariable(
                 [MarshalAs(UnmanagedType.LPTStr)]
                 string lpName,
                 [MarshalAs(UnmanagedType.LPTStr)]
                 string lpValue);

        private bool PathFromRegistry(RegistryKey rootKey, string delphiKeyName, ref RegistryKey delphiKey)
        {
            string key = SOFTWARE_ROOT_PATH + delphiKeyName;
            delphiKey = rootKey.OpenSubKey(key, false);
            if (delphiKey == null)
            {
                Log(Level.Debug, string.Format(@"Delphi key not found at {0}\{1}, trying in 32bit key in 64 bit environment", rootKey, key));
                key = SOFTWARE_ROOT_PATH + SOFTWARE_32_BIT_ON_64_BIT_PATH + delphiKeyName;
                delphiKey = rootKey.OpenSubKey(key, false);
                if (delphiKey == null)
                {
                    Log(Level.Debug, string.Format(@"Delphi key not found at {0}\{1}", rootKey, key));
                    return false;
                }
            }
            return true;
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
                    else
                    {
                        string delphiKey;

                        if (!_versionRegistryPaths.TryGetValue(_versionString, out delphiKey))
                        {
                            throw new BuildException(string.Format("Delphi version {0} not supported", this._versionString));
                        }
                        else
                        {
                            RegistryKey key = null;
                            if (!PathFromRegistry(Registry.CurrentUser, delphiKey, ref key) && !PathFromRegistry(Registry.LocalMachine, delphiKey, ref key))
                            {
                                throw new BuildException(string.Format("Delphi version {0} not found in registry", this._versionString));
                            }
                            directory = this.getDelphiRootPath(key);
                        }
                    }
                    return directory;
                }
            }
        }

    }

}
