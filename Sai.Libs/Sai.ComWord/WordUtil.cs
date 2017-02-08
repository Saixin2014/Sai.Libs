using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai.ComWord
{
    public class WordUtil
    {
        public static string DocToText(string docFileName)
        {
            //实例化COM        
            Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
            StringBuilder outTextSb = new StringBuilder();
            object fileobj = docFileName;
            object nullobj = System.Reflection.Missing.Value;//打开指定文件（不同版本的COM参数个数有差异，
                                                             //一般而言除第一个外都用nullobj就行了）
            try
            {
                Microsoft.Office.Interop.Word.Document wd = WordApp.Documents.Open(ref fileobj, ref nullobj, ref nullobj, ref nullobj,
                                                        ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                                        ref nullobj, ref nullobj, ref nullobj, ref nullobj, ref nullobj,
                                                        ref nullobj, ref nullobj);
                //取得doc文件中的文本内容
                outTextSb.Append(wd.Content.Text);
                //wd.Footnotes[0].Range.Text;
                //关闭文件
                wd.Close(ref nullobj, ref nullobj, ref nullobj);

                //返回文本内容
            }
            catch (Exception ex)
            {
                string info = string.Format("time：{0}---fileName:{1}---{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), docFileName, ex.Message + " " + ex.StackTrace);
                Console.WriteLine(info);
                //return "";
            }
            finally
            {
                if (WordApp != null)
                {
                    //关闭COM
                    WordApp.Quit(ref nullobj, ref nullobj, ref nullobj);
                }
            }
            return outTextSb.ToString();
        }
    }
}
