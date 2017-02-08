
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai.Develop.Logging
{
    public enum SLogLevel
    {
        Normal,
        Trace,
        Warn,
        Error,
        Fatal,
    }

    /// <summary>
    /// 日志类
    /// </summary>
    public class SLogger
    {
        private static readonly Lazy<SLogger> current = new Lazy<SLogger>((Func<SLogger>)(() => new SLogger()), true);
        protected static NLog.Logger m_Logger = LogManager.GetCurrentClassLogger();
        private static SLogger m_Instance;

        public static SLogger Instance
        {
            get
            {
                return SLogger.current.Value;
            }
        }

        static SLogger()
        {
        }

        protected SLogger()
        {
        }

        public void Writer(string msg)
        {
            this.Writer(Sai.Develop.Logging.SLogLevel.Normal, msg);
        }

        public void Writer(Sai.Develop.Logging.SLogLevel level, string msg)
        {
            switch (level)
            {
                case Sai.Develop.Logging.SLogLevel.Normal:
                    SLogger.m_Logger.Info(msg);
                    break;
                case Sai.Develop.Logging.SLogLevel.Trace:
                    SLogger.m_Logger.Trace(msg);
                    break;
                case Sai.Develop.Logging.SLogLevel.Warn:
                    SLogger.m_Logger.Warn(msg);
                    break;
                case Sai.Develop.Logging.SLogLevel.Error:
                    SLogger.m_Logger.Error(msg);
                    break;
                case Sai.Develop.Logging.SLogLevel.Fatal:
                    SLogger.m_Logger.Fatal(msg);
                    break;
            }
        }
    }
}
