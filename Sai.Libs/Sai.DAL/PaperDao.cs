using DAL;
using Sai.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace Sai.DAL
{
    public class PaperDao : DaoBase<PaperDto>
    {
        public override List<PaperDto> GetList(PaperDto criteria)
        {
            throw new NotImplementedException();
        }

        protected override PaperDto GetModel(SafeDataReader dr)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand PrepareAddCommand(PaperDto model)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand PrepareDeleteCommand(PaperDto criteria)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand PrepareExistCommand(PaperDto criteria)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand PrepareGetModelCommand(PaperDto criteria)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand PrepareUpdateCommand(PaperDto model)
        {
            throw new NotImplementedException();
        }
    }
}
