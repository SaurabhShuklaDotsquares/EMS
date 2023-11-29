using System;

namespace EMS.Dto
{
    public class AgeDto
    {
        public int Years;
        public int Months;
        public int Days;

        public AgeDto(DateTime Bday)
        {
            this.Count(Bday);
        }

        public AgeDto(DateTime Bday, DateTime Cday)
        {
            this.Count(Bday, Cday);
        }

        public AgeDto Count(DateTime Bday)
        {
            return this.Count(Bday, DateTime.Today);
        }

        public AgeDto Count(DateTime Bday, DateTime Cday)
        {
            int Years = Cday > Bday ? new DateTime(Cday.Subtract(Bday).Ticks).Year - 1 : 0;
            DateTime PastYearDate = Bday.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Cday)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Cday)
                {
                    Months = i - 1;
                    break;
                }
            }
           
           // int Days = Cday.Day - Bday.Day;
            this.Years = Years;
            this.Months = Months;
            int Days = Cday.Subtract(PastYearDate.AddMonths(this.Months)).Days;
            this.Days = Days;
            return this;
        }
    }
}
