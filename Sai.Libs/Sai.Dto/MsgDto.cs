using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sai.Dto
{
    /// <summary>
    /// 通用的异常消息数据类
    /// </summary>
    public class ErrorDto
    {
        public ErrorDto()
        {
            Code = "0";
            Msg = "";
        }
        /// <summary>
        /// 0表示无消息 其他异常信息
        /// </summary>
        public string Code { get; set; }

        public string Msg { get; set; }
    }
}
