using Spire.Doc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sai.Word
{
    public class WordHelper
    {
        /// <summary>
        /// 获得Word Text文本
        /// </summary>
        /// <param name="docFileName"></param>
        /// <returns></returns>
        public static string DocToText(string docFileName)
        {
            if (!File.Exists(docFileName))
            {
                throw new FileNotFoundException(docFileName+" Not exist!");
            }
            using (Document doc = new Document())
            {
                doc.LoadFromFile(@docFileName);
                string content = doc.GetText();
                doc.Close();
                return content;
            }
        }

        public static string DocToText(Stream sm)
        {
            using (Document doc = new Document(sm,FileFormat.Docx))
            {
                string content = doc.GetText();
                doc.Close();
                return content;
            }
        }
        public static string DocToText(Stream sm,string suffix)
        {
            FileFormat ff = FileFormat.Docx;
            if (suffix.EndsWith("doc"))
            {
                ff = FileFormat.Doc;
            }
            using (Document doc = new Document(sm, ff))
            {
                string content = doc.GetText();
                doc.Close();
                return content;
            }
        }
    }
}
