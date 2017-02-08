using Sai.Develop.Logging;
using Sai.Dto;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai.TextSimilarity
{
    public class UserPaperHandler
    {
        public UserPaperHandler()
        {
        }


        private ConcurrentQueue<UserPaperDto> m_TaskQueue =
            new ConcurrentQueue<UserPaperDto>();//任务队列

        //加载任务队列
        public void AddTasks(IList<UserPaperDto> datas)
        {
            TaskTotalCount = datas.Count;
            HandledNum = 0;
            ErrorNum = 0;
            foreach (var v in datas)
            {
                m_TaskQueue.Enqueue(v);
            }

            //任务未开始或者任务结束 开启任务线程
            m_OneFinished = false;
            RunTask();
            m_State = JobState.Running;
        }

        private void RunTask()
        {
            int num = DefaultThreadNum < m_TaskQueue.Count ? DefaultThreadNum : m_TaskQueue.Count;
            for (int i = 0; i < num; i++)
            {
                UserPaperDto t;
                if (m_TaskQueue.TryDequeue(out t))
                {
                    Task ts = new Task(() => Run(t));
                    ts.Start();
                }
            }
        }

        private static int DefaultThreadNum = 20;//

        private object m_StateLock = new object();//任务状态锁
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

        private static int HandledNum = 0;
        private object m_HandledNumLock = new object();

        private static int ErrorNum = 0;
        private object m_ErrorNumLock = new object();

        private static int TaskTotalCount = 0;//带执行的任务总数

        private bool m_OneFinished = false;
        private bool m_IsCompleted = false;
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
        //Go to Next
        private void RunOneTask()
        {
            if (m_TaskQueue.Count == 0)
                return;
            UserPaperDto t;
            if (m_TaskQueue.TryDequeue(out t))
            {

                Task ts = new Task(() => Run(t));
                ts.Start();
            }
        }

        Stopwatch sw = new Stopwatch();
        Dictionary<string, PaperHandler> m_HandlerDic = new Dictionary<string, PaperHandler>();
        private void Run(UserPaperDto d)
        {
            try
            {
                //todo report event
                PaperHandler paperHandler = new PaperHandler();
                paperHandler.Reporting += PaperHandler_Reporting;
                paperHandler.ReportFinished += PaperHandler_ReportFinished;
                paperHandler.UserPaper = d;
                paperHandler.MonitorUserName = m_CurUserName;
                paperHandler.AddTasks(d.PaperInfo);
                m_HandlerDic.Add(d.UserName, paperHandler);
            }
            catch (Exception ex)
            {
                SLogger.Instance.Writer(SLogLevel.Error, ex.Message);
            }
            finally
            {
                
            }
        }

        private void PaperHandler_ReportFinished(object sender, PaperInfoFinishArgs e)
        {
            try
            {
                //one finished
                if (e.Error != null)
                {
                    //异常出错
                    lock (m_ErrorNumLock) { ErrorNum++; }
                    SLogger.Instance.Writer(SLogLevel.Error, e.Error.Code + e.Error.Msg);
                    return;
                }
                UserPaperDto u = e.UPaper;
                string uName = u.UserName;

                lock (m_HandledNumLock)
                {
                    //执行的成功次数
                    HandledNum++;
                }
                PaperInfoFinishArgs arg = new PaperInfoFinishArgs() { UPaper = u };
                OnRaiseReporting(arg);

                //to remove
                if (m_HandlerDic.ContainsKey(uName))
                {
                    m_HandlerDic.Remove(uName);
                }
            }
            catch (Exception ex)
            {
                SLogger.Instance.Writer(SLogLevel.Error, e.Error.Code + e.Error.Msg);
            }
            finally
            {
                //todo next 轮询
                RunOneTask();
                lock (m_FinishLock)
                {
                    if (IsCompleted && !m_OneFinished)
                    {
                        m_OneFinished = true;
                        m_State = JobState.End;
                        //finished
                        Console.WriteLine("All Finished!");
                        OnRaiseFinished(new EventArgs());
                    }
                }
            }
        }

        private string m_CurUserName = "";
        /// <summary>
        /// 动态设置推送的UserName 执行消息
        /// </summary>
        /// <param name="uName"></param>
        public void SetMonitorUserName(string uName)
        {
            m_CurUserName = uName;
            foreach (var k in m_HandlerDic.Keys)
            {
                m_HandlerDic[k].MonitorUserName = uName;
            }
        }

        private void PaperHandler_Reporting(object sender, ObjectArgs e)
        {
            //通告 一个用户下的执行情况
            //if (string.IsNullOrEmpty(m_CurUserName))
            //    return;
            OnRaiseMsgReporting(e);
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


        public delegate void ReportEventHandler(object sender, PaperInfoFinishArgs e);

        /// <summary>
        /// 报告事件
        /// </summary>
        public event ReportEventHandler OneFinished;

        private void OnRaiseReporting(PaperInfoFinishArgs e)
        {
            ReportEventHandler handler = OneFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public delegate void FinishedEventHandler(object sender, EventArgs e);
        /// <summary>
        /// 全部加载完毕事件
        /// </summary>
        public event FinishedEventHandler AllFinished;

        private void OnRaiseFinished(EventArgs e)
        {
            FinishedEventHandler handler = AllFinished;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
