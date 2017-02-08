using NPOI.HSSF.UserModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sai.Excel
{
    public interface IExcel
    {
        /// <summary> 打开文件 </summary>  
        bool Open();
        /// <summary> 文件版本 </summary>  
        ExcelVersion Version { get; }
        /// <summary> 文件路径 </summary>  
        string FilePath { get; set; }
        /// <summary> 文件是否已经打开 </summary>  
        bool IfOpen { get; }
        /// <summary> 文件包含工作表的数量 </summary>  
        int SheetCount { get; }
        /// <summary> 当前工作表序号 </summary>  
        int CurrentSheetIndex { get; set; }
        /// <summary> 获取当前工作表中行数 </summary>  
        int GetRowCount();
        /// <summary> 获取当前工作表中列数 </summary>  
        int GetColumnCount();
        /// <summary> 获取当前工作表中某一行中单元格的数量 </summary>  
        /// <param name="Row">行序号</param>  
        int GetCellCountInRow(int Row);
        /// <summary> 获取当前工作表中某一单元格的值（按字符串返回） </summary>  
        /// <param name="Row">行序号</param>  
        /// <param name="Col">列序号</param>  
        string GetCellValue(int Row, int Col);
        /// <summary> 关闭文件 </summary>  
        void Close();

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="headerDic"></param>
        /// <param name="dt"></param>
        void ExportToFile(string[] headerDic, DataTable dt, string sheetName);
    }

    public enum ExcelVersion
    {
        /// <summary> Excel2003之前版本 ,xls </summary>  
        Excel03,
        /// <summary> Excel2007版本 ,xlsx  </summary>  
        Excel07
    }

    public class ExcelUtils
    {

    }

    public class ExcelLib
    {
        /// <summary> 获取Excel对象 </summary>  
        /// <param name="filePath">Excel文件路径</param>  
        /// <returns></returns>  
        public static IExcel GetExcel(string filePath)
        {
            if (filePath.Trim() == "")
                throw new Exception("文件名不能为空");

            if (!filePath.Trim().EndsWith("xls") && !filePath.Trim().EndsWith("xlsx"))
                throw new Exception("不支持该文件类型");

            if (filePath.Trim().EndsWith("xls"))
            {
                IExcel res = new Excel03(filePath.Trim());
                return res;
            }
            else if (filePath.Trim().EndsWith("xlsx"))
            {
                IExcel res = new Excel07(filePath.Trim());
                return res;
            }
            else return null;
        }
    }

    public class Excel03 : IExcel
    {
        public Excel03()
        { }

        public Excel03(string path)
        { filePath = path; }

        private FileStream file = null;
        private string filePath = "";
        private HSSFWorkbook book = null;
        private int sheetCount = 0;
        private bool ifOpen = false;
        private int currentSheetIndex = 0;
        private HSSFSheet currentSheet = null;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public bool Open()
        {
            try
            {
                file = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                book = new HSSFWorkbook(file);

                if (book == null) return false;
                sheetCount = book.NumberOfSheets;
                currentSheetIndex = 0;
                currentSheet = (HSSFSheet)book.GetSheetAt(0);
                ifOpen = true;
            }
            catch (Exception ex)
            {
                throw new Exception("打开文件失败，详细信息：" + ex.Message);
            }
            return true;
        }

        public void Close()
        {
            if (!ifOpen) return;
            file.Close();
        }

        public ExcelVersion Version
        { get { return ExcelVersion.Excel03; } }

        public bool IfOpen
        { get { return ifOpen; } }

        public int SheetCount
        { get { return sheetCount; } }

        public int CurrentSheetIndex
        {
            get { return currentSheetIndex; }
            set
            {
                if (value != currentSheetIndex)
                {
                    if (value >= sheetCount)
                        throw new Exception("工作表序号超出范围");
                    currentSheetIndex = value;
                    currentSheet = (HSSFSheet)book.GetSheetAt(currentSheetIndex);
                }
            }
        }

        public int GetRowCount()
        {
            if (currentSheet == null) return 0;
            return currentSheet.LastRowNum + 1;
        }

        public int GetColumnCount()
        {
            if (currentSheet == null) return 0;
            int colCount = 0;
            for (int i = 0; i <= currentSheet.LastRowNum; i++)
            {
                if (currentSheet.GetRow(i) != null && currentSheet.GetRow(i).LastCellNum + 1 > colCount)
                    colCount = currentSheet.GetRow(i).LastCellNum + 1;
            }
            return colCount;
        }

        public int GetCellCountInRow(int Row)
        {
            if (currentSheet == null) return 0;
            if (Row > currentSheet.LastRowNum) return 0;
            if (currentSheet.GetRow(Row) == null) return 0;

            return currentSheet.GetRow(Row).LastCellNum + 1;
        }

        public string GetCellValue(int Row, int Col)
        {
            if (Row > currentSheet.LastRowNum) return "";
            if (currentSheet.GetRow(Row) == null) return "";
            HSSFRow r = (HSSFRow)currentSheet.GetRow(Row);

            if (Col > r.LastCellNum) return "";
            if (r.GetCell(Col) == null) return "";
            return r.GetCell(Col).ToString();
        }

        public void ExportToFile(string[] headerDic, DataTable dt, string sheetName)
        { }
    }

    public class Excel07 : IExcel
    {
        public Excel07()
        { }

        public Excel07(string path)
        { filePath = path; }

        private string filePath = "";
        private ExcelWorkbook book = null;
        private int sheetCount = 0;
        private bool ifOpen = false;
        private int currentSheetIndex = 0;
        private ExcelWorksheet currentSheet = null;
        private ExcelPackage ep = null;

        public bool Open()
        {
            try
            {

                ep = new ExcelPackage(new FileInfo(filePath));

                if (ep == null) return false;

                book = ep.Workbook;
                sheetCount = book.Worksheets.Count;
                currentSheetIndex = 0;
                currentSheet = book.Worksheets[1];
                ifOpen = true;
            }
            catch (Exception ex)
            {
                throw new Exception("打开文件失败，详细信息：" + ex.Message);
            }
            return true;
        }

        public void Close()
        {
            if (!ifOpen || ep == null) return;
            ep.Dispose();
        }

        public ExcelVersion Version
        { get { return ExcelVersion.Excel07; } }

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        public bool IfOpen
        { get { return ifOpen; } }

        public int SheetCount
        { get { return sheetCount; } }

        public int CurrentSheetIndex
        {
            get { return currentSheetIndex; }
            set
            {
                if (value != currentSheetIndex)
                {
                    if (value >= sheetCount)
                        throw new Exception("工作表序号超出范围");
                    currentSheetIndex = value;
                    currentSheet = book.Worksheets[currentSheetIndex + 1];
                }
            }
        }

        public int GetRowCount()
        {
            if (currentSheet == null) return 0;
            return currentSheet.Dimension.End.Row;
        }

        public int GetColumnCount()
        {
            if (currentSheet == null) return 0;
            return currentSheet.Dimension.End.Column;
        }

        public int GetCellCountInRow(int Row)
        {
            if (currentSheet == null) return 0;
            if (Row >= currentSheet.Dimension.End.Row) return 0;
            return currentSheet.Dimension.End.Column;
        }

        public string GetCellValue(int Row, int Col)
        {
            if (currentSheet == null) return "";
            if (Row >= currentSheet.Dimension.End.Row || Col >= currentSheet.Dimension.End.Column) return "";
            object tmpO = currentSheet.GetValue(Row + 1, Col + 1);
            if (tmpO == null) return "";
            return tmpO.ToString();
        }

        public void ExportToFile(string[] headerDic, DataTable dt, string sheetName)
        {
            FileInfo newFile = new FileInfo(FilePath);
            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                //worksheet.Cells[1, 1].Value = "名称";
                //worksheet.Cells[1, 2].Value = "价格";
                //worksheet.Cells[1, 3].Value = "销量";
                for (int i = 0; i < headerDic.Length; i++)
                {
                    //1开始
                    worksheet.Cells[1, i + 1].Value = headerDic[i];
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //int rDx = 1 + i + 1;//从2开始
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        //1开始
                        worksheet.Cells[2+i, j+1].Value = dt.Rows[i][j].ToString();
                    }
                }
                package.Save();
            }
        }
    }
}
