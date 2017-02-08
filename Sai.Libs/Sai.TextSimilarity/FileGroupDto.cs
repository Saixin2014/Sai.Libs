using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sai.TextSimilarity
{
    public class FileGroupDto
    {
        public FileGroupDto()
        {
            Files = new List<FileDto>();
            Similar = new Dictionary<string, double>();
            PaperSimilarity = new List<PaperSimilarDto>();
        }
        public List<FileDto> Files
        { get; set; }

        public Dictionary<string,double> Similar { get; set; }

        public List<PaperSimilarDto> PaperSimilarity { get; set; }
    }

    public class PaperSimilarDto
    {
        public FileDto Paper1 { get; set; }

        public FileDto Paper2 { get; set; }

        public double Similar { get; set; }
    }
}
