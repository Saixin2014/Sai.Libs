using Sai.Develop.Logging;
using Sai.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sai.Crawler
{
    public class TaskRunner
    {
        public HttpHeader Header
        {
            get; set;
        }

        public CookieContainer LoginCookie
        {
            get; set;
        }

        private ConcurrentStack<object> m_TaskQue =
            new ConcurrentStack<object>();//任务队列

        //加载任务队列
        public void AddTasks(IList<object> datas)
        {
            TaskTotalCount = datas.Count;
            HandledNum = 0;
            ErrorNum = 0;
            foreach (var v in datas)
            {
                m_TaskQue.Push(v);
            }
            if (m_State == JobState.NotStart
                || m_State == JobState.End)
            {
                //任务未开始或者任务结束 开启任务线程
                m_OneFinished = false;
                m_State = JobState.Running;
                RunTask();
            }
        }


        private static int DefaultThreadNum = 5;
        private JobState m_State = JobState.NotStart;//任务的状态
        /// <summary>
        /// 任务的状态
        /// </summary>
        public JobState State
        {
            get { return m_State; }
        }

        /// <summary>
        /// 设置线程数
        /// </summary>
        /// <param name="threadNum"></param>
        public void SetThreadNum(int threadNum)
        {
            DefaultThreadNum = threadNum;
        }

        private bool m_IsCompleted = false;

        private static int HandledNum = 0;
        private object m_HandledNumLock = new object();

        private static int ErrorNum = 0;
        private object m_ErrorNumLock = new object();

        private static int TaskTotalCount = 0;
        private bool m_OneFinished = false;
        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                if (TaskTotalCount == 0 ||
                    HandledNum + ErrorNum == TaskTotalCount)
                {
                    m_IsCompleted = true;
                }
                else
                {
                    m_IsCompleted = false;
                }
                return m_IsCompleted;
            }
        }

        private void RunTask()
        {
            int num = DefaultThreadNum < m_TaskQue.Count ? DefaultThreadNum : m_TaskQue.Count;
            for (int i = 0; i < num; i++)
            {
                object t;
                if (m_TaskQue.TryPop(out t))
                {
                    Task ts = new Task(() => Run(t));
                    ts.Start();
                }
            }
        }

        //Go to Next
        private void RunOneTask()
        {
            if (m_TaskQue.Count == 0)
                return;
            object t;
            if (m_TaskQue.TryPop(out t))
            {

                Task ts = new Task(() => Run(t));
                ts.Start();
            }
        }
        //one task

        private void Run(object objData)
        {
            try
            {

                //2.线程 跑

                string url = "http://223.223.176.56:8090/foreground/Book/detail.action?tsid=";
                url += objData;

                string html = "";//HTMLHelper.GetHtml(url, LoginCookie, Header);

                if (string.IsNullOrEmpty(html))
                {
                    //未成功获取 重新加入队列中
                    //等待下次从堆栈取出处理
                    m_TaskQue.Push(objData);
                    //System.Threading.Thread.Sleep(5 * 1000);//休息5s        
                    return;
                }


                //todo

                //to save data

                lock (m_HandledNumLock)
                {
                    HandledNum++;
                    //to report
                    ReportMsg();
                }

            }
            catch (Exception ex)
            {
                lock (m_ErrorNumLock)
                {
                    ErrorNum++;
                    ReportMsg();
                }
                StringBuilder msg = new StringBuilder();
                msg.AppendLine("当前时间:" + DateTime.Now);
                msg.AppendLine(ex.Message + ex.StackTrace);
                SLogger.Instance.Writer(SLogLevel.Error, msg.ToString());
            }
            finally
            {
                RunOneTask();
                lock (m_FinishLock)
                {
                    string info = string.Format("hanlerNum:{0}|erorrNum:{1}|totalNum:{2}", HandledNum, ErrorNum, TaskTotalCount);
                    if (IsCompleted && !m_OneFinished)
                    {
                        m_OneFinished = true;
                        ObjectArgs arg = new ObjectArgs() { ObjData = info };
                        OnRaiseFinished(arg);
                        m_State = JobState.End;
                    }
                }
            }
        }

        private void ReportMsg()
        {
            string msg = string.Format("成功处理个数：{0}|出现错误：{1}", HandledNum, ErrorNum);
            ObjectArgs arg = new ObjectArgs() { ObjData = msg };
            OnRaiseMsgReporting(arg);
        }


        private object m_FinishLock = new object();


        public delegate void MsgReportEventHandler(object sender, ObjectArgs e);

        public event MsgReportEventHandler MsgReporting;
        /// <summary>
        /// 执行的消息告知
        /// </summary>
        /// <param name="args"></param>
        private void OnRaiseMsgReporting(ObjectArgs args)
        {
            MsgReportEventHandler handler = MsgReporting;
            if (handler != null)
            {
                handler.Invoke(this, args);
            }
        }


        //public delegate void ReportEventHandler(object sender, ObjectArgs e);

        ///// <summary>
        ///// 报告事件
        ///// </summary>
        //public event ReportEventHandler OneFinished;

        //private void OnRaiseReporting(ObjectArgs e)
        //{
        //    ReportEventHandler handler = OneFinished;
        //    if (handler != null)
        //    {
        //        handler(this, e);
        //    }
        //}

        public delegate void FinishedEventHandler(object sender, ObjectArgs e);
        /// <summary>
        /// 全部加载完毕事件
        /// </summary>
        public event FinishedEventHandler AllFinished;

        private void OnRaiseFinished(ObjectArgs e)
        {
            FinishedEventHandler handler = AllFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
