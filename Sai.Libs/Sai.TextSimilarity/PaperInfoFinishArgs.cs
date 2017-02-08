using Sai.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sai.TextSimilarity
{
    public class PaperDtoArgs:EventArgs
    {
        public PaperDto Paper { get; set; }
    }

    public class PaperInfoFinishArgs : EventArgs
    {
        public ErrorDto Error { get; set; }
        public UserPaperDto UPaper { get; set; }
    }
}
