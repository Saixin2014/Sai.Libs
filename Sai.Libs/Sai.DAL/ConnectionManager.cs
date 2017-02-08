
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
//using System.Data.OracleClient;
using System.Data.Common;
using System.Data;
//using Utils;
using System.Configuration;


namespace DAL
{
    /// <summary>
    /// 数据库连接管理类
    /// </summary>
    internal sealed class ConnectionManager
    {
        private static ConnectionManager m_Instance = null;
        internal static ConnectionManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new ConnectionManager();
                }
                return m_Instance;
            }
        }

        internal Database Databaser
        {
            get;
            private set;
        }

        private static string ConnStr = ConfigurationManager.AppSettings["ConnStr"];

        private ConnectionManager()
        {
            //DataSourceElement dse = DataSourceConfig.DataSources[DataSourceConfig.Instance.DefaultDataSource];


            //Database db = DatabaseFactory.CreateDatabase();

            //string connStr = string.Format(@"user id={0};password={1};{2}", datacenter.OraUser, datacenter.OraPasswd, db.ConnectionStringWithoutCredentials);
            //var dbCtr = db.GetType().GetConstructor(new Type[] { typeof(string) });
            //this.Databaser = (Database)dbCtr.Invoke(new object[] { connStr });


            //StringBuilder sb = new StringBuilder(100);
            //sb.Append("Data Source=").Append(DataSource).Append(";User Id=").Append(User).Append(";password=").
            //    Append(PWD);

            //微软自带注册的
            //Database db = new GenericDatabase(sb.ToString(), DbProviderFactories.GetFactory(ProvideName));

            //扩展的Database
            //string str = ConfigurationManager.ConnectionStrings[ConnStr].ConnectionString;
            Database db = DatabaseFactory.CreateDatabase(ConnStr);
            this.Databaser = db;

           //Logger.Instance.Writer(LogLevel.Trace, "初始化数据库对象！");
        }
    }
}
