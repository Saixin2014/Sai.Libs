using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sai.TextSimilarity
{
    public class UserPaperDto : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyname)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        public int SeqNum { get; set; }

        public string UserName { get; set; }

        public int PaperNum { get; set; }

        private int m_GroupNum = 0;
        public int GroupNum
        {
            get { return m_GroupNum; }
            set
            {
                m_GroupNum = value;
                OnPropertyChanged("GroupNum");
            }
        }

        private string m_State;
        public string State
        {
            get { return m_State; }
            set { m_State = value; OnPropertyChanged("State"); }
        }
        private int m_SpendTime;
        public int SpendTime
        { get { return m_SpendTime; } set { m_SpendTime = value;OnPropertyChanged("SpendTime"); } }

        public List<FileDto> PaperInfo { get; set; }

        public List<FileGroupDto> GroupData { get; set; }
    }
}
