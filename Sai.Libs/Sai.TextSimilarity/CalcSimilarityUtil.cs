using JiebaNet.Analyser;
using JiebaNet.Segmenter;
using Sai.Dto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utils;

namespace Sai.TextSimilarity
{
    /// <summary>
    /// 计算两篇论文的相似度
    /// </summary>
    public class CalcSimilarityUtil
    {
        //    private CalcSimilarityUtil() { }

        //    private static CalcSimilarityUtil m_Instance;

        //    public static CalcSimilarityUtil Instance
        //    {
        //        get
        //        {
        //            if (m_Instance == null)
        //                m_Instance = new CalcSimilarityUtil();
        //            return m_Instance;
        //        }
        //    }


        public double CalsSimilarityByFile(string filePath1, string filePath2)
        {
            string txt1 = TextUtil.GetText(filePath1);
            string txt2 = TextUtil.GetText(filePath2);
            double similar = CalcSimilarity(txt1, txt2);
            return similar;
        }
        public double CalsTfidfSimilarityByFile(string filePath1, string filePath2)
        {
            string txt1 = TextUtil.GetText(filePath1);
            string txt2 = TextUtil.GetText(filePath2);
            double similar = CalcTFIDFSimilarity(txt1, txt2);
            return similar;
        }
        /// <summary>
        /// 去掉分隔符 返回分词结果
        /// </summary>
        /// <param name="txtStr"></param>
        /// <returns></returns>
        public IEnumerable<string> SegText(string txtStr)
        {
            //去掉分隔符，。...
            string txt = Regex.Replace(txtStr, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");
            var segmenter = new JiebaSegmenter();
            IEnumerable<string> seg = segmenter.Cut(txt);
            return seg;
        }

        private string GetText(IEnumerable<string> seg)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var s in seg)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        private static int TopNum = 500;
        public double CalcTFIDFSimilarity(string paperText1, string paperText2)
        {
            ObjectArgs arg = new ObjectArgs();
            //去掉分隔符，。...
            string txt1 = paperText1;
            txt1 = Regex.Replace(txt1, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");

            string txt2 = paperText2;
            txt2 = Regex.Replace(txt2, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");

            var segmenter = new JiebaSegmenter();
            JiebaNet.Analyser.TfidfExtractor tfd = new TfidfExtractor(segmenter);
            arg.ObjData = "开始分词";
            OnRaiseReporting(arg);
            Console.WriteLine("开始分词");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IEnumerable<string> segment1 = segmenter.Cut(txt1);

            IEnumerable<string> segment2 = segmenter.Cut(txt2);

            int num1 = segment1.Count();
            int num2 = segment2.Count();

            Console.WriteLine("文档1分词数:" + num1);

            Console.WriteLine("文档2分词数:" + num2);

            arg.ObjData = "文档1分词数:" + num1 + "\r\n文档2分词数: " + num2;
            OnRaiseReporting(arg);

            IEnumerable<string> seg1 = null;//多的
            IEnumerable<string> seg2 = null;//少的

            if (num1 > num2)
            {
                seg1 = segment1;
                seg2 = segment2;
            }
            else
            {
                seg2 = segment1;
                seg1 = segment2;

            }

            int maxLength = seg1.Count();
            int minLength = seg2.Count();
            double similar = 0;

            //Dictionary<string, double> seg2Dic = CalcTF(seg2);
            string str2 = GetText(seg2);
            for (int i = 0; i + minLength <= maxLength; i++)
            {
                //0-interval
                //1-interval+1
                //2-interval+2 ...
                IEnumerable<string> seg = seg1.Where((item, index) => index > i && index < i + minLength);//取i 到 i+minLength
                //从seg1 中截取与seg2相同数量的词集合seg
                //分别计算词频 seg2的词频只需要计算一次
                //计算cos ===相似度
                //Dictionary<string, double> dic = CalcTF(seg);

                //double s = CalcSimilar(dic, seg2Dic);
                string str = GetText(seg);
                int topNum = 500;
                IEnumerable<WordWeightPair> tf_a = tfd.ExtractTagsWithWeight(str2, topNum);
                IEnumerable<WordWeightPair> tf_b = tfd.ExtractTagsWithWeight(str, topNum);

                double molecular = 0;// 分子
                double denominator_a = 0;// 分母
                double denominator_b = 0;

                Dictionary<string, WordWeightPair> dic_a = new Dictionary<string, WordWeightPair>();
                Dictionary<string, WordWeightPair> dic_b = new Dictionary<string, WordWeightPair>();
                foreach (var a in tf_a)
                {
                    dic_a.Add(a.Word, a);
                }

                foreach (var b in tf_b)
                {
                    dic_b.Add(b.Word, b);
                }

                //Console.WriteLine("两篇文档相似的词有：");

                foreach (var k in dic_a.Keys)
                {
                    WordWeightPair a = dic_a[k];
                    WordWeightPair b;
                    dic_b.TryGetValue(k, out b);
                    denominator_a += a.Weight * a.Weight;

                    molecular += a.Weight * (null == b ? 0 : b.Weight);
                    //if (a != null && b != null)
                    //{

                    //    Console.WriteLine(a.Word + "  TF-IDF词频统计 文档一：" + a.Weight + "|文档二："
                    //            + b.Weight);
                    //}
                }
                foreach (var k in dic_b.Keys)
                {
                    WordWeightPair b = dic_b[k];
                    denominator_b += b.Weight * b.Weight;
                }
                double s = 0;
                if (denominator_a != 0 && denominator_b != 0)
                {
                    s = (molecular / (Math.Sqrt(denominator_a) * Math.Sqrt(denominator_b)));
                }

                //Console.WriteLine("两篇文档相似度：" + s * 100 + "%");
                if ((i + 1) % 50 == 0)
                {
                    Console.WriteLine(string.Format("第{0}次计算出的相似度：{1}", i + 1, s));

                    arg.ObjData = string.Format("第{0}次计算出的相似度：{1}", i + 1, s);
                    OnRaiseReporting(arg);
                }
                if (s > similar)
                {
                    similar = s;
                }
                if (s >= 0.99)
                {
                    //极高相似度
                    Console.WriteLine(string.Format("第{0}次计算出的相似度：{1}", i + 1, s));

                    arg.ObjData = string.Format("第{0}次计算出的相似度：{1}", i + 1, s);
                    OnRaiseReporting(arg);
                }

                //Console.WriteLine("第"+i+"次花费时间：" + sw.ElapsedMilliseconds / 1000 + "秒");
            }
            sw.Stop();
            Console.WriteLine("两篇文章的相似度：" + similar);

            Console.WriteLine("花费时间：" + sw.ElapsedMilliseconds + "ms");
            arg.ObjData = string.Format("两篇文章的相似度：" + similar + "\r\n花费时间：" + sw.ElapsedMilliseconds + "ms");
            OnRaiseReporting(arg);
            return similar;
        }



        /// <summary>
        /// 计算文章1和文章2的相似度
        /// </summary>
        /// <param name="paperText1">文章1的文本</param>
        /// <param name="paperText2">文章2的文本</param>
        /// <returns></returns>
        public double CalcSimilarity(string paperText1, string paperText2)
        {
            ObjectArgs arg = new ObjectArgs();
            //去掉分隔符，。...
            string txt1 = paperText1;
            txt1 = Regex.Replace(txt1, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");

            string txt2 = paperText2;
            txt2 = Regex.Replace(txt2, @"[^a-zA-Z0-9\u4e00-\u9fa5\s]", "");

            var segmenter = new JiebaSegmenter();
            arg.ObjData = "开始分词";
            OnRaiseReporting(arg);
            Console.WriteLine("开始分词");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            IEnumerable<string> segment1 = segmenter.Cut(txt1);

            IEnumerable<string> segment2 = segmenter.Cut(txt2);

            int num1 = segment1.Count();
            int num2 = segment2.Count();

            Console.WriteLine("文档1分词数:" + num1);

            Console.WriteLine("文档2分词数:" + num2);

            arg.ObjData = "文档1分词数:" + num1 + "\r\n文档2分词数: " + num2;
            OnRaiseReporting(arg);

            IEnumerable<string> seg1 = null;//多的
            IEnumerable<string> seg2 = null;//少的

            if (num1 > num2)
            {
                seg1 = segment1;
                seg2 = segment2;
            }
            else
            {
                seg2 = segment1;
                seg1 = segment2;

            }

            int maxLength = seg1.Count();
            int minLength = seg2.Count();
            double similar = 0;
            Dictionary<string, double> seg2Dic = CalcTF(seg2);
            for (int i = 0; i + minLength <= maxLength; i++)
            {
                //0-interval
                //1-interval+1
                //2-interval+2 ...
                IEnumerable<string> seg = seg1.Where((item, index) => index > i && index < i + minLength);//取i 到 i+minLength
                //从seg1 中截取与seg2相同数量的词集合seg
                //分别计算词频 seg2的词频只需要计算一次
                //计算cos ===相似度
                Dictionary<string, double> dic = CalcTF(seg);

                double s = CalcSimilar(dic, seg2Dic);
                if ((i + 1) % 50 == 0)
                {
                    Console.WriteLine(string.Format("第{0}次计算出的相似度：{1}", i + 1, s));

                    arg.ObjData = string.Format("第{0}次计算出的相似度：{1}", i + 1, s);
                    OnRaiseReporting(arg);
                }
                if (s > similar)
                {
                    similar = s;
                }
                if (s >= 0.99)
                {
                    //极高相似度
                    Console.WriteLine(string.Format("第{0}次计算出的相似度：{1}", i + 1, s));

                    arg.ObjData = string.Format("第{0}次计算出的相似度：{1}", i + 1, s);
                    OnRaiseReporting(arg);
                }
                //Console.WriteLine("第"+i+"次花费时间：" + sw.ElapsedMilliseconds / 1000 + "秒");
            }
            sw.Stop();
            Console.WriteLine("两篇文章的相似度：" + similar);

            Console.WriteLine("花费时间：" + sw.ElapsedMilliseconds + "ms");
            arg.ObjData = string.Format("两篇文章的相似度：" + similar + "\r\n花费时间：" + sw.ElapsedMilliseconds + "ms");
            OnRaiseReporting(arg);
            return similar;
        }

        public double CalcSimilar(Dictionary<string, double> dic1, Dictionary<string, double> dic2)
        {
            double similar = 0;
            double molecular = 0;// 分子
            double denominator_a = 0;// 分母
            double denominator_b = 0;
            foreach (var k in dic1.Keys)
            {
                double a = dic1[k];
                double b;
                dic2.TryGetValue(k, out b);
                denominator_a += a * a;

                molecular += a * (0 == b ? 0 : b);
                if (a != 0 && b != 0)
                {
                    //相同词

                }
            }
            foreach (var k in dic2.Keys)
            {
                double b = dic2[k];
                denominator_b += b * b;
            }
            double result = 0;
            if (denominator_a != 0 && denominator_b != 0)
            {
                result = (molecular / (Math.Sqrt(denominator_a) * Math.Sqrt(denominator_b)));
            }
            similar = result;
            return similar;
        }


        public Dictionary<string, double> CalcTF(IEnumerable<string> seg)
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            foreach (string w in seg)
            {
                double tf = 0;
                if (dic.TryGetValue(w, out tf))
                {
                    dic[w] += 1;//计数+1
                }
                else
                {
                    dic.Add(w, 1);
                }
            }
            //int num = dic.Count;
            //foreach (var k in dic.Keys.ToArray<string>())
            //{
            //    dic[k] = dic[k] / num;
            //}

            return dic;
        }


        public delegate void ReportMsgEventHandler(object sender, ObjectArgs e);

        /// <summary>
        /// 报告事件
        /// </summary>
        public event ReportMsgEventHandler Reporting;

        private void OnRaiseReporting(ObjectArgs e)
        {
            ReportMsgEventHandler handler = Reporting;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
