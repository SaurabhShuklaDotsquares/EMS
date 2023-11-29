using EMS.Data;
using EMS.Repo;
using EMS.Service.LibraryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class SerialNumberService : ISerialNumberService
    {
        private IRepository<SerialNumber> repoSerialNumber;
        public SerialNumberService(IRepository<SerialNumber> _repoSerialNumber)
        {
            repoSerialNumber = _repoSerialNumber;
        }
        public void Dispose()
        {
            if (repoSerialNumber != null)
            {
                repoSerialNumber.Dispose();
                repoSerialNumber = null;
            }
        }

        public int GetNumber()
        {
            return repoSerialNumber.Query().Get().FirstOrDefault().Id;
        }

        public void Save(SerialNumber entity)
        {
            var serialNumber = repoSerialNumber.Query().Get().FirstOrDefault();
            repoSerialNumber.Delete(serialNumber);
            repoSerialNumber.InsertGraph(entity);
            //serialNumber.Id = entity.Id;
            //repoSerialNumber.SaveChanges();
        }
    }
}
