using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sai.Dto
{
    /// <summary>
    /// 组件加载的状态
    /// </summary>
    public enum LoadStat
    {
        None,//未初始化，或clear后
        Loading,//正在加载中
        Loaded,//加载完毕
        UnLoading,//卸载中
        UnLoaded//卸载完毕
    }
}
