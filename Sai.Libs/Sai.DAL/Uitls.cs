using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Sai.DAL
{
    public class Uitls
    {
        public static void AddParm(DbCommand cmd, string ParameterName, DbType dtype, object value)
        {
            DbParameter idParm = cmd.CreateParameter();
            idParm.ParameterName = ParameterName;
            idParm.DbType = dtype;
            idParm.Value = value;
            cmd.Parameters.Add(idParm);
        }
    }
}
