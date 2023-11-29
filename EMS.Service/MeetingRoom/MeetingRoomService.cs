using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;
using EMS.Repo;
using System.Linq.Expressions;
using EMS.Core;
using System.Globalization;

namespace EMS.Service
{
    public class MeetingRoomService : IMeetingRoomService
    {
        #region "Fields"
        private IRepository<CompanyOffice> repoCompanyOffice;
        private IRepository<ConferenceRoom> repoConferenceRoom;
        private IRepository<ConferenceRoomBooking> repoConferenceRoomBooking;
        #endregion

        #region "Cosntructor"
        public MeetingRoomService(IRepository<CompanyOffice> _repoCompanyOffice, IRepository<EMS.Data.ConferenceRoom> _repoConferenceRoom, IRepository<ConferenceRoomBooking> _repoConferenceRoomBooking)
        {
            this.repoCompanyOffice = _repoCompanyOffice;
            this.repoConferenceRoom = _repoConferenceRoom;
            this.repoConferenceRoomBooking = _repoConferenceRoomBooking;
        }
        #endregion

        public List<ConferenceRoomBooking> GetDateFilterConferenceRoom(int id, DateTime startDate, DateTime endDate, string country)
        {
            return repoConferenceRoomBooking.Query()
                .Filter(x => (id == 0 || x.ConferenceRoomId == id) &&
                            x.ConferenceRoom.CompanyOffice.Country == country &&
                            x.StartTime >= startDate && x.EndTime <= endDate)
                .Get()
                .ToList();
        }
        public List<CompanyOffice> GetGetOfficeList()
        {
            return repoCompanyOffice.Query().Filter(R => R.IsActive == true).Get().OrderBy(R => R.Name).ToList();
        }
        //public List<CompanyOfficeFloor> GetOfficeFloorList()
        //{
        //    return repoCompanyOfficeFloor.Query().Filter(R => R.IsActive == true).Get().OrderBy(R => R.CompanyOffice.Name).ToList();
        //}
        public List<ConferenceRoom> GetConferenceRoomList()
        {
            return repoConferenceRoom.Query().Filter(R => R.IsActive == true).Get().OrderBy(R => R.CompanyOffice.Name).ToList();
        }
        public List<ConferenceRoomBooking> GetConferenceRoomBookingList(int CRoomId)
        {
            if (CRoomId > 0)
                return repoConferenceRoomBooking.Query().Filter(R => R.ConferenceRoomId == CRoomId).Get().ToList();
            return repoConferenceRoomBooking.Query().Get().ToList();
        }
        public void Save(ConferenceRoomBooking entity)
        {
            if (entity.Id == 0)
            {
                repoConferenceRoomBooking.Insert(entity);
            }
            else
            {
                repoConferenceRoomBooking.ChangeEntityState(entity, ObjectState.Modified);
                repoConferenceRoomBooking.SaveChanges();
            }
        }

        public void SaveRoom(ConferenceRoom entity)
        {
            if (entity.Id == 0)
            {
                repoConferenceRoom.ChangeEntityState<ConferenceRoom>(entity, ObjectState.Added);
                repoConferenceRoom.InsertGraph(entity);
            }
            else
            {
                repoConferenceRoom.ChangeEntityState<ConferenceRoom>(entity, ObjectState.Modified);
                repoConferenceRoom.SaveChanges();
            }
        }
        public bool CheckedConferenceRoomBooked(int ConferenceRoomId, DateTime StartTime, DateTime EndTime, int exceptBookingId)
        {
            //StartTime = StartTime.ToUniversalTime();
            //EndTime = EndTime.ToUniversalTime();
            var result = repoConferenceRoomBooking.Query().Filter(r => r.ConferenceRoomId == ConferenceRoomId && r.Id != exceptBookingId &&
                 //((StartTime > r.StartTime && StartTime < r.EndTime) ||
                 //(EndTime > r.StartTime && EndTime < r.EndTime) ||
                 //(StartTime == r.StartTime && EndTime == r.EndTime))
                 //(StartTime >= r.StartTime && StartTime < r.EndTime) && (EndTime > r.StartTime && EndTime <= r.EndTime))
                 (StartTime >= r.StartTime && EndTime <= r.EndTime) && (StartTime < r.EndTime && EndTime > r.StartTime))
                .GetQuerable().Any();

            return result;
        }

        public void Delete(long CRoomId)
        {
            repoConferenceRoomBooking.Delete(CRoomId);
        }
        public ConferenceRoomBooking GetConferenceRoomBookingById(long CRoomId)
        {
            return repoConferenceRoomBooking.FindById(CRoomId);
        }

        public ConferenceRoom GetConferenceRoom(int Id)
        {
            return repoConferenceRoom.FindById(Id);
        }

        public List<ConferenceRoom> GetRooms(out int total, PagingService<ConferenceRoom> pagingService)
        {
            return repoConferenceRoom.Query()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();
        }

        public double GetTotalRooms(PagingService<ConferenceRoom> pagingService)
        {
            return (repoConferenceRoom.Query()
                .Filter(pagingService.Filter).GetQuerable().Count());
        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoCompanyOffice != null)
            {
                repoCompanyOffice.Dispose();
                repoCompanyOffice = null;
            }
            //if (repoCompanyOfficeFloor != null)
            //{
            //    repoCompanyOfficeFloor.Dispose();
            //    repoCompanyOfficeFloor = null;
            //}
            if (repoConferenceRoom != null)
            {
                repoConferenceRoom.Dispose();
                repoConferenceRoom = null;
            }
            if (repoConferenceRoomBooking != null)
            {
                repoConferenceRoomBooking.Dispose();
                repoConferenceRoomBooking = null;
            }
        }
        #endregion

    }
}
