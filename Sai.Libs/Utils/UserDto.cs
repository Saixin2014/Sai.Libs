using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    /// <summary>
    /// 登录用户信息
    /// </summary>
    [Serializable]
    public class UserDto : ICloneable
    {

        public int UserId
        {
            get; set;
        }

        public string UserName
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        private bool m_IsRemember = false;
        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool IsRemember
        {
            get { return m_IsRemember; }
            set { m_IsRemember = value; }
        }

        private bool m_AutoLogin = false;

        /// <summary>
        /// 是否自动登录
        /// </summary>
        public bool AutoLogin
        {
            get { return m_AutoLogin; }
            set { m_AutoLogin = value; }
        }

        private DateTime m_LastLogin = DateTime.Now;

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public DateTime LastLogin
        {
            get { return m_LastLogin; }
            set { m_LastLogin = value; }
        }


        public string Token
        {
            get;
            set;
        }

        public Object Clone()  //实现ICloneable接口，达到浅表复制。
        {
            return this.MemberwiseClone();
        }
    }
}
