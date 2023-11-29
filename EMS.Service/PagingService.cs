using System;
using System.Linq;
using System.Linq.Expressions;

namespace EMS.Service
{
    public class PagingService<T> where T : class
    {
        public PagingService()
        {

        }

        public PagingService(int start, int length)
        {
            this.Start = start == 0 ? 1 : (Convert.ToInt32(Decimal.Floor(Convert.ToDecimal(start) / length)) + 1);
            this.Length = length;
        }
        
        public int Start { get; set; }
        public int Length { get; set; }
        public Expression<Func<T, bool>> Filter { get; set; }
        public Func<IQueryable<T>, IOrderedQueryable<T>> Sort { get; set; }

        public void SortBy(Func<IQueryable<T>, IOrderedQueryable<T>> orderBy)
        {
            Sort = orderBy;
        }
    }
}
