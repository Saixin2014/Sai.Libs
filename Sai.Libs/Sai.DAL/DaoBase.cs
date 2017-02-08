using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Data;

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Sai.Dto;

namespace DAL
{
    public abstract class DaoBase<TModel> where TModel : class
    {
        //使用static 有两个原因：
        //1. 可以缓存database，使其在整个网站可用。但是entlib文档说不需要缓存也行，因为已经使用了连接池。这个还需要测试。
        //2. 可以与生成的代码进行匹配，因为生成代码方法都是static
        protected static Database db = null;//DatabaseFactory.CreateDatabase();

        #region Properties
        /// <summary>
        /// 基本的查询语句
        /// </summary>
        protected virtual string BaseQuerySql
        {
            get
            {
                throw new NotSupportedException("BaseQuerySql is not supported.");
            }
        }

        /// <summary>
        /// 获取数据记录的SQL语句
        /// </summary>
        protected virtual string GetCountSql
        {
            get
            {
                throw new NotSupportedException("GetCountSql is not supported.");
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// 使用受保护的构造函数，不允许在外部进行该类的实例化
        /// </summary>
        protected DaoBase()
        {
            db = ConnectionManager.Instance.Databaser;
        }
        #endregion

        #region CRUD
        /// <summary>
        /// 添加一条记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(TModel model)
        {
            DbCommand cmd = PrepareAddCommand(model);
            return db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 更新一条记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(TModel model)
        {
            DbCommand cmd = PrepareAddCommand(model);
            return db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public int Delete(TModel criteria)
        {
            DbCommand cmd = PrepareDeleteCommand(criteria);
            return db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// 判断记录是否存在
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public bool Exists(TModel criteria)
        {
            DbCommand cmd = PrepareExistCommand(criteria);
            object obj = db.ExecuteScalar(cmd);
            int result = 0;
            if (null != obj)
            {
                int.TryParse(obj.ToString(), out result);
            }
            return (result > 0); //如果记录数大于0，表示存在
        }

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public virtual TModel GetModel(TModel criteria)
        {
            TModel model = default(TModel);
            DbCommand cmd = PrepareGetModelCommand(criteria);
            using (SafeDataReader dr = new SafeDataReader(db.ExecuteReader(cmd)))
            {
                if (dr.Read())
                {
                    model = this.GetModel(dr);
                }
            }
            return model;
        }

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <returns></returns>
        public virtual int GetCount()
        {
            DbCommand cmd = db.GetSqlStringCommand(this.GetCountSql);
            object obj = db.ExecuteScalar(cmd);
            int result = 0;
            if (null != obj)
            {
                int.TryParse(obj.ToString(), out result);
            }
            return result;
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <returns></returns>
        public virtual List<TModel> GetList()
        {
            return this.GetList(string.Empty, string.Empty);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual List<TModel> GetList(string where, string order)
        {
            DbCommand cmd = db.GetSqlStringCommand(this.GetQuerySql(where, order));
            using (SafeDataReader dr = new SafeDataReader(db.ExecuteReader(cmd)))
            {
                List<TModel> lst = this.GetList(dr);
                return lst;
            }
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public abstract List<TModel> GetList(TModel criteria);

        /// <summary>
        /// 获取分页的实体列表
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        public virtual PageList<TModel> GetPageList(PageInfo pi)
        {
            return this.GetPageList(pi, string.Empty, string.Empty);
        }

        /// <summary>
        /// 获取分页的实体列表
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public virtual PageList<TModel> GetPageList(PageInfo pi, string where, string order)
        {
            pi.RecordCount = this.GetCount();
            pi.Compute();

            PageList<TModel> pl = new PageList<TModel>(pi);

            DbCommand cmd = db.GetSqlStringCommand(this.GetQuerySql(where, order));
            using (SafeDataReader dr = new SafeDataReader(db.ExecuteReader(cmd)))
            {
                pl.List = this.GetList(dr);
            }
            return pl;
        }

        #endregion

        #region Abstract Methods
        /// <summary>
        /// 为新增一条数据准备参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract DbCommand PrepareAddCommand(TModel model);
        /// <summary>
        /// 为更新一条数据准备参数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected abstract DbCommand PrepareUpdateCommand(TModel model);
        /// <summary>
        /// 为删除一条数据准备参数
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected abstract DbCommand PrepareDeleteCommand(TModel criteria);
        /// <summary>
        /// 为查询是否存在一条数据准备参数
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected abstract DbCommand PrepareExistCommand(TModel criteria);
        /// <summary>
        /// 为获取一条数据准备参数
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected abstract DbCommand PrepareGetModelCommand(TModel criteria);
        /// <summary>
        /// 由一行数据得到一个实体
        /// </summary>
        protected abstract TModel GetModel(SafeDataReader dr);

        #endregion

        #region Internal Mothods

        /// <summary>
        /// 获取查询语句，含where子句和order子句
        /// </summary>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        protected string GetQuerySql(string where, string order)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(this.BaseQuerySql);
            strSql.Append(this.ConcatWhereAndOrderStr(where, order));

            return strSql.ToString();
        }

        /// <summary>
        /// 连接WHERE子句和ORDER子句
        /// </summary>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        protected string ConcatWhereAndOrderStr(string where, string order)
        {
            StringBuilder strSql = new StringBuilder();
            if (!string.IsNullOrEmpty(where))
            {
                strSql.Append(" WHERE ");
                string whereTemp = Regex.Replace(where, @"\s*where\s*", string.Empty, RegexOptions.IgnoreCase);
                //whereTemp = Regex.Replace(whereTemp, @"and\s?$", string.Empty, RegexOptions.IgnoreCase);
                strSql.Append(whereTemp);
            }
            if (!string.IsNullOrEmpty(order))
            {
                strSql.Append(" ORDER BY ");
                string orderTemp = Regex.Replace(order, @"\s*order by\s*", string.Empty, RegexOptions.IgnoreCase);
                strSql.Append(orderTemp);
            }

            return strSql.ToString();
        }

        /// <summary>
        /// 由DbDataReader得到泛型数据列表
        /// </summary>
        protected List<TModel> GetList(SafeDataReader dr)
        {
            List<TModel> lst = new List<TModel>();
            while (dr.Read())
            {
                lst.Add(GetModel(dr));
            }
            return lst;
        }

        /// <summary>
        /// 由DbDataReader得到分页泛型数据列表
        /// </summary>
        private List<TModel> GetPageList(SafeDataReader dr, int first, int count)
        {
            List<TModel> lst = new List<TModel>();

            for (int i = 0; i < first; i++)
            {
                if (!dr.Read())
                {
                    return lst;
                }
            }

            int resultsFetched = 0;
            while (resultsFetched < count && dr.Read())
            {
                lst.Add(GetModel(dr));
                resultsFetched++;
            }

            return lst;
        }

        #endregion
    }
}
