using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMS.Data;

namespace EMS.Service
{
    public interface IMeetingRoomService : IDisposable
    {
        List<CompanyOffice> GetGetOfficeList();
        List<ConferenceRoom> GetConferenceRoomList();
        //List<CompanyOfficeFloor> GetOfficeFloorList();
        List<ConferenceRoomBooking> GetConferenceRoomBookingList(int CRoomId);
        void Save(ConferenceRoomBooking entity);
        ConferenceRoomBooking GetConferenceRoomBookingById(long CRoomId);
        void Delete(long CRoomId);
        bool CheckedConferenceRoomBooked(int ConferenceRoomId,DateTime StartTime, DateTime EndTime, int exceptBookingId);
        void SaveRoom(ConferenceRoom conferenceRoomDB);
        List<ConferenceRoom> GetRooms(out int total, PagingService<ConferenceRoom> pagingService);
        double GetTotalRooms(PagingService<ConferenceRoom> pagingService);
        ConferenceRoom GetConferenceRoom(int Id);
        List<ConferenceRoomBooking> GetDateFilterConferenceRoom(int id, DateTime startDate, DateTime endDate, string country);
    }
}
