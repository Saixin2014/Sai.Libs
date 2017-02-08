using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Utils
{
    public class AccountHelper
    {
        private static string m_UserFileName = @"datas.db";

        private AccountHelper()
        {
        }

        private static AccountHelper _Instance;

        public static AccountHelper Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new AccountHelper();
                return _Instance;
            }
        }

        /// <summary>
        /// 获得历史登录用户信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, UserDto> GetUserInfos()
        {
            //读取文件流对象
            Dictionary<string, UserDto> users = new Dictionary<string, UserDto>();
            using (FileStream fs = new FileStream(m_UserFileName, FileMode.OpenOrCreate))
            {
                if (fs.Length > 0)
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    //读出存在Data.bin 里的用户信息
                    users = bf.Deserialize(fs) as Dictionary<string, UserDto>;
                    foreach (var v in users)
                    {
                        //if (v.Value.IsRemember)
                        //{
                        //    v.Value.Password = MyAES.AESDecrypt(v.Value.Password);
                        //}
                    }
                }
            }
            return users;
        }

        public void SaveUserInfos(Dictionary<string, UserDto> userDic)
        {
            Dictionary<string, UserDto> dic = new Dictionary<string, UserDto>();
            
            //先加密
            foreach (var key in userDic.Keys)
            {
                var user = userDic[key].Clone() as UserDto;
                dic.Add(key, user);
            }
            foreach (var key in dic.Keys)
            {
                var user = dic[key];
                if (user.IsRemember)
                {
                    user.Password = MyAES.AESEncrypt(user.Password);
                }
                else
                {
                    user.Password = "";
                }
            }
            // 登录时 如果没有data.db文件就创建、有就打开
            Task ts = new Task(() =>
            {
                using (FileStream fs = new FileStream(m_UserFileName, FileMode.OpenOrCreate))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    //写入文件
                    bf.Serialize(fs, dic);
                    //关闭
                    fs.Close();
                }
                //System.Threading.Thread.Sleep(1000*10);
            });
            
            ts.Start();
        }
    }
}
