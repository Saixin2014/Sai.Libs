using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sai.Dto
{
    #region EnumMessageBox

    /// <summary>
    /// EnumMessageBox的信息类型
    /// </summary>
    public enum EnumMessageBox
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info,

        /// <summary>
        /// 错误
        /// </summary>
        Error,

        /// <summary>
        /// 询问
        /// </summary>
        Question,

        /// <summary>
        /// 警告
        /// </summary>
        Warning,
    }

    #endregion


    public enum JobState
    {
        NotStart,
        Running,
        End
    }
}
