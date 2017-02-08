using Sai.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai.BLL
{
    public class BusinessManage
    {
        private BusinessManage()
        {

        }
        private static BusinessManage _Instance;
        public static BusinessManage Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new BusinessManage();
                }
                return _Instance;
            }
        }
        private PaperDao m_UserPaperDao = new PaperDao();

        //todo

    }
}
