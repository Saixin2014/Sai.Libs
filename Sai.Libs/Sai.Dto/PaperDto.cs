using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sai.Dto
{
    public class PaperDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string ReportId { get; set; }

        public string FileName { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public int Score { get; set; }

        public string UploadTime { get; set; }

        public string Category { get; set; }

        public string Level { get; set; }

        public string SavePath { get; set; }
    }
}
