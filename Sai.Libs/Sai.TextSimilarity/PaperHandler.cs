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

    public class PaperHandler
    {
        public PaperHandler()
        {
            m_Calc = new CalcSimilarityUtil();
            m_Calc.Reporting += M_Calc_Reporting;
        }

        private void M_Calc_Reporting(object sender, ObjectArgs e)
        {
            if (m_CurUserName.Equals(UserPaper.UserName))
            {
                OnRaiseReporting(e.ObjData);
            }
        }

        private ConcurrentQueue<FileDto> m_TaskQueue =
            new ConcurrentQueue<FileDto>();//任务队列
        private CalcSimilarityUtil m_Calc = null;

        public UserPaperDto UserPaper { get; set; }

        private string m_CurUserName = "";
        public string MonitorUserName
        {
            get { return m_CurUserName; }
            set { m_CurUserName = value; }
        }

        //加载任务队列
        public void AddTasks(IList<FileDto> datas)
        {
            //if (!System.IO.Directory.Exists(SaveDic))
            //{
            //    Directory.CreateDirectory(SaveDic);
            //}
            //TaskTotalCount = datas.Count;
            //HandledNum = 0;
            //ErrorNum = 0;
            foreach (var v in datas)
            {
                m_TaskQueue.Enqueue(v);
            }

            //任务未开始或者任务结束 开启任务线程
            m_OneFinished = false;
            RunTask();

        }

        private void RunTask()
        {
            //int num = DefaultThreadNum < m_TaskStack.Count ? DefaultThreadNum : m_TaskStack.Count;
            int num = m_TaskQueue.Count;

            if (num == 0)
            {
                sw.Stop();
                //finished report
                //m_Groups
                UserPaper.SpendTime = (int)sw.ElapsedMilliseconds / 1000;
                UserPaper.GroupData = m_Groups;
                UserPaper.GroupNum = m_Groups.Count;
                PaperInfoFinishArgs args = new PaperInfoFinishArgs() { UPaper = UserPaper };
                OnRaiseFinished(this, args);
                Console.WriteLine(string.Format("共分{0}组", m_Groups.Count));
                return;
            }

            List<FileDto> datas = new List<FileDto>();
            for (int i = 0; i < num; i++)
            {
                FileDto t;
                if (m_TaskQueue.TryDequeue(out t))
                {
                    datas.Add(t);
                }
            }
            if (datas.Count >= 1)
            {
                Task ts = new Task(() => Run(datas));
                ts.Start();
            }
        }

        //private static int DefaultThreadNum = 1;//单线程跑

        private object m_StateLock = new object();//任务状态锁
        //private JobState m_State = JobState.NotStart;//任务的状态
        ///// <summary>
        ///// 任务的状态
        ///// </summary>
        //public JobState State
        //{
        //    get { return m_State; }
        //}

        /// <summary>
        /// 设置线程数
        /// </summary>
        /// <param name="threadNum"></param>
        //public void SetThreadNum(int threadNum)
        //{
        //    DefaultThreadNum = threadNum;
        //}

        private static int HandledNum = 0;
        private object m_HandledNumLock = new object();

        private static int TaskTotalCount = 0;//带执行的任务总数

        private bool m_OneFinished = false;


        //Go to Next

        Stopwatch sw = new Stopwatch();
        //List<ManualResetEvent> m_Events = new List<ManualResetEvent>();//任务信号量 标识每个任务是否完成


        private List<FileGroupDto> m_Groups = new List<FileGroupDto>();//按照相似度进行分组

        private static double GROUP_SIMILARITY = 0.75;//相似度>=0.8 分为同一组

        /// <summary>
        /// 设置分组的相似度阈值
        /// </summary>
        /// <param name="similarity"></param>
        public void SetGroupSimilarity(double similarity)
        {
            GROUP_SIMILARITY = similarity;
        }

        private void Run(List<FileDto> datas)
        {
            try
            {
                sw.Start();
                //List<FileDto> groupLs = new List<FileDto>();
                FileGroupDto fg = new FileGroupDto();
                if (datas.Count == 1)
                {
                    //最后一个 不用计算相似度
                    //当前所有计算任务完成
                    //groupLs.AddRange(datas);

                    fg.Files.AddRange(datas);

                    PaperSimilarDto psd = new PaperSimilarDto() { Paper1 = datas[0], Paper2 = datas[0], Similar = 1 };
                    fg.PaperSimilarity.Add(psd);
                    m_Groups.Add(fg);
                    return;
                }

                //从第一个轮询 一一比较
                //>=0.8 进入分组
                //<0.8 放回队列
                FileDto first = datas[0];
                //groupLs.Add(first);
                List<PaperSimilarDto> groupLs = new List<PaperSimilarDto>();
                for (int i = 1; i < datas.Count; i++)
                {
                    double similar = m_Calc.CalsSimilarityByFile(first.FilePath, datas[i].FilePath);
                    //todo report event
                    string msg = string.Format("{0}和{1}的相似度：{2}", first.FileName, datas[i].FileName, similar);
                    Console.WriteLine(msg);

                    fg.Similar.Add(string.Format("{0}和{1}的相似度", first.FileName, datas[i].FileName), similar);

                    if (m_CurUserName.Equals(UserPaper.UserName))
                    {
                        OnRaiseReporting(msg);
                    }


                    //1-2:
                    //1-3:
                    //1-4:
                    if (similar >= GROUP_SIMILARITY)
                    {
                        //groupLs.Add(datas[i]);
                        fg.Files.Add(datas[i]);
                        PaperSimilarDto psd = new PaperSimilarDto() { Paper1 = first, Paper2 = datas[i], Similar = similar };
                        groupLs.Add(psd);
                        //fg.PaperSimilarity.Add(psd);
                    }
                    else
                    {
                        //重新入队列
                        m_TaskQueue.Enqueue(datas[i]);
                    }
                }
                //与论文1相似的找到分组
                if (groupLs.Count >= 1)
                {
                    fg.PaperSimilarity.AddRange(groupLs);
                }
                else
                {
                    //与论文1相似的未找到分组
                    PaperSimilarDto psd = new PaperSimilarDto() { Paper1 = first, Paper2 = first, Similar = 1 };
                    fg.PaperSimilarity.Add(psd);
                }
                m_Groups.Add(fg);
                //todo report event

            }
            catch (Exception ex)
            {
                SLogger.Instance.Writer(SLogLevel.Error, ex.Message);
            }
            finally
            {
                //todo next 轮询
                RunTask();
            }
        }


        private object m_FinishLock = new object();

        public delegate void ReportEventHandler(object sender, ObjectArgs e);

        /// <summary>
        /// 报告事件
        /// </summary>
        public event ReportEventHandler Reporting;

        private void OnRaiseReporting(object msg)
        {
            ObjectArgs e = new ObjectArgs() { ObjData = msg };
            ReportEventHandler handler = Reporting;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public delegate void FinishedEventHandler(object sender, PaperInfoFinishArgs e);
        /// <summary>
        /// 全部加载完毕事件
        /// </summary>
        public event FinishedEventHandler ReportFinished;

        private void OnRaiseFinished(object sender, PaperInfoFinishArgs e)
        {
            FinishedEventHandler handler = ReportFinished;
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
