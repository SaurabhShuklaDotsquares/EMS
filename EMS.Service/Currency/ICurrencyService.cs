using EMS.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Service
{
    public interface ICurrencyService : IDisposable
    {
        Currency GetCurrencyByName(string Currency);
        List<Currency> GetCurrency();
        Currency GetCurrencyByEstimateCountry(int countryId);
    }
}
