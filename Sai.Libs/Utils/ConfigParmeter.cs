using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    /// <summary>
    /// 参考读取ini文件
    /// </summary>
    public class ConfigParmeter
    {
        private static string IniFilePath = @"config.ini";
        private ConfigParmeter()
        {
            _Parser = new FileIniDataParser();
            _IniData = _Parser.ReadFile(IniFilePath);
        }

        private static ConfigParmeter m_Instance = null;
        private FileIniDataParser _Parser = null;
        private IniData _IniData = null;
        public static ConfigParmeter Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new ConfigParmeter();
                return m_Instance;
            }
        }

        public string GetIpUrl1
        {
            get
            {
                return _IniData["config"]["GetIpUrl1"];
            }
        }

        public string GetIpUrl2
        {
            get
            {
                return _IniData["config"]["GetIpUrl2"];
            }
        }
        public string GetIpUrl3
        {
            get
            {
                return _IniData["config"]["GetIpUrl3"];
            }
        }

        public string ValidateUrl
        {
            get
            {
                return _IniData["config"]["ValidateUrl"];
            }
        }

        public string GetIpLoc1
        {
            get
            {
                return _IniData["config"]["GetIpLoc1"];
            }
        }

        public string GetIpLoc2
        {
            get
            {
                return _IniData["config"]["GetIpLoc2"];
            }
        }

        public string ProxyName
        {
            get
            {
                return _IniData["config"]["ProxyName"];
            }
        }

        public string ProxyPwd
        {
            get
            {
                return _IniData["config"]["ProxyPwd"];
            }
        }

        public string QqwryPath
        {
            get
            {
                return _IniData["config"]["QqwryPath"];
            }
        }

        public int MinSecond
        {
            get
            {
                return int.Parse(_IniData["config"]["MinSecond"]);
            }
        }

        public int MaxSecond
        {
            get
            {
                return int.Parse(_IniData["config"]["MaxSecond"]);
            }
        }
    }
}
