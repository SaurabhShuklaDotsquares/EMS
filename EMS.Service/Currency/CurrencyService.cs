using EMS.Data;
using EMS.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
     public class CurrencyService:ICurrencyService
    {
        #region "Fields"
        private IRepository<Currency> repoCurrency;
        #endregion
        #region "Cosntructor"
        public CurrencyService(IRepository<Currency> repoCurrency)
        {
            this.repoCurrency = repoCurrency;
        }
        #endregion "Cosntructor"
        public Currency GetCurrencyByName(string Currency)
        {
            return repoCurrency.Query().Filter(C => C.CurrName == Currency).Get().FirstOrDefault();
        }

        public List<Currency> GetCurrency()
        {
            return repoCurrency.Query().Get().ToList();
        }

        public Currency GetCurrencyByEstimateCountry(int countryId)
        {
            return repoCurrency.Query().Filter(C => C.EstimateCountry.Any(a => a.Id == countryId)).Get().FirstOrDefault();
        }

        public void Dispose()
        {
            if (repoCurrency != null)
            {
                repoCurrency.Dispose();
                repoCurrency = null;
            }
        }
    }
}
