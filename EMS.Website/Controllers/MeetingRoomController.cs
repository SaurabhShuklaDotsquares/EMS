using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.LIBS;
using EMS.Web.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using static EMS.Core.Enums;


namespace EMS.Web.Controllers
{
    [CustomAuthorization()]
    public class MeetingRoomController : BaseController
    {
        #region "Fields"

        private IMeetingRoomService meetingRoomService;
        #endregion

        #region "Constructor"
        public MeetingRoomController(IMeetingRoomService _meetingRoomService)
        {
            this.meetingRoomService = _meetingRoomService;
        }

        #endregion

        #region Meeting calender
        // GET: MeetingRoom
        [CustomActionAuthorization]
        public ActionResult Index()
        {
            ViewBag.OfficeList = GetOffices();
            ViewBag.MeetingRoomList = GetMeetingRooms();
            ViewBag.ColorList = GetColorsRoomList();
            return View();
        }

        public ActionResult GetEvents(int id, string start, string end)
        {
            string country = Enums.Country.India.ToString();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                country = Enums.Country.UK.ToString();
            }

            List<ConferenceRoomBooking> conferenceRoomBookingList = null;

            DateTime? startDate = start.ToDateTime("yyyy-MM-dd") ?? DateTime.Today;
            DateTime? endDate = end.ToDateTime("yyyy-MM-dd") ?? DateTime.Today;

            conferenceRoomBookingList = meetingRoomService.GetDateFilterConferenceRoom(id, startDate.Value, endDate.Value, country);

            var events = conferenceRoomBookingList
                .Select(x => new
                {
                    EventID = x.Id.ToString(),
                    MeetingSubject = x.Title.ToString(),
                    SubjectTitle = id == 0 ? $"{x.StartTime.ToFormatDateString("hh:mm tt")} - {x.EndTime.ToFormatDateString("hh:mm tt")} {x.Title} [ {x.ConferenceRoom.Name} - {x.ConferenceRoom.CompanyOffice.Name} ]" : $"{x.StartTime.ToFormatDateString("hh:mm tt")} - {x.EndTime.ToFormatDateString("hh:mm tt")} {x.Title.ToString()}",
                    Subject = id == 0 ? $"{x.Title} [ {x.ConferenceRoom.Name } - { x.ConferenceRoom.CompanyOffice.Name } ]" : x.Title,
                    MeetingTime = x.StartTime.ToFormatDateString("hh:mm tt") + " - " + x.EndTime.ToFormatDateString("hh:mm tt"),
                    Description = x.Description,
                    Start = x.StartTime,
                    End = x.EndTime,
                    CreatedBy = x.UserLogin.Name,
                    Location = x.ConferenceRoom.Name +"<br/><span style='font-size:11px;padding-left:20px;'> " + x.ConferenceRoom.CompanyOffice.OfficeAddress+"</span>",
                    ConferenceRoomId = x.ConferenceRoomId ?? 0,
                    Attendee = x.AttendeeName.ToString(),
                    ThemeColor = x.ConferenceRoom.ThemeColor,
                    IsDeletedAllow = x.BookedByUid == CurrentUser.Uid,
                    TimeDifference = GetTimeDifference(x.StartTime, x.EndTime),
                }).ToList();

            return NewtonSoftJsonResult(events);
        }

        [HttpPost]
        public JsonResult SaveEvent(BooKMeetingRoomDto model)
        {
            try
            {
                var booking = new ConferenceRoomBooking();
                string format = "dd/MM/yyyy hh:mmtt";

                DateTime startTime = $"{model.Date.Trim()} {model.StartTime.Trim()}".ToDateTime(format).Value;
                DateTime endTime = $"{model.Date.Trim()} {model.EndTime.Trim()}".ToDateTime(format).Value;

                //check if already book
                bool isRoomBooked = meetingRoomService.CheckedConferenceRoomBooked(model.ConferenceRoomId, startTime, endTime, model.EventID);

                if (isRoomBooked)
                {
                    return new JsonResult(new { status = false });
                }

                if (model.EventID > 0)
                {
                    booking = meetingRoomService.GetConferenceRoomBookingById(model.EventID);
                }
                else
                {
                    booking.BookedByUid = CurrentUser.Uid;
                    booking.CreatedDate = DateTime.Now;
                }

                booking.ConferenceRoomId = model.ConferenceRoomId;
                booking.Description = model.Description;
                booking.Title = model.Title;
                booking.StartTime = startTime;
                booking.EndTime = endTime;
                booking.AttendeeName = model.AttendeeName;
                booking.ModifiedDate = DateTime.Now;

                meetingRoomService.Save(booking);
                return new JsonResult(new { status = true });
            }
            catch(Exception ex)
            {
                var error = ex.Message;
                return new JsonResult(new { status = false });
            }
        }

        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            try
            {
                meetingRoomService.Delete(eventID);
                return new JsonResult(new { status = true });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { status = false });
            }
        }

        [HttpGet]
        public ActionResult AddTimeSlot(int? eventID)
        {
            BooKMeetingRoomDto response = new BooKMeetingRoomDto();
            response.OfficeList = GetOffices();
            response.ConferenceRoomList = GetMeetingRooms();
            response.TimeSlot = GetTimeSlot();
            return PartialView("_AddTimeSlot", response);
        }

        [HttpGet]
        public ActionResult GetDisabledTimeSlot(int roomId, string date, int bookingId)
        {
            string slotids = string.Empty;
            try
            {
                var timeslot = GetTimeSlot();
                foreach (var row in timeslot)
                {
                    var firstPartDate = date.Split("/")[0];
                    var secondPartDate = date.Split("/")[1];
                    var thirdPartDate = date.Split("/")[2];

                    if(Convert.ToInt16(firstPartDate)>12)
                    {
                        date = secondPartDate + "/" + firstPartDate + "/" + thirdPartDate;
                    }
                   

                    var startTimeOld = Convert.ToDateTime(date.Trim() + " " + row.Text.Split('-')[0].Trim());
                    var endTimeOld = Convert.ToDateTime(date.Trim() + " " + row.Text.Split('-')[1].Trim());

                    var startTimeString = startTimeOld.ToString().ToDateTime();
                    var endTimeString = endTimeOld.ToString().ToDateTime();

                    DateTime startTime = Convert.ToDateTime(startTimeString);
                    DateTime endTime = Convert.ToDateTime(endTimeString);

                    bool isBooked = meetingRoomService.CheckedConferenceRoomBooked(roomId, startTime, endTime, bookingId);
                    if (isBooked)
                    {
                        slotids = slotids + row.Slot_Id.ToString() + "#";
                    }
                }
            }
            catch(Exception ex)
            {
                var erroeMsg = ex.Message;
            }

            return Json(slotids);
        }
        #endregion Meeting calender

        #region Manage rooms
        [HttpGet]
        [CustomActionAuthorization]
        public ActionResult RoomList()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetRooms(IDataTablesRequest request)
        {
            //string country = Enums.Country.India.ToString();
            //if (CurrentUser.RoleId == 7 || CurrentUser.RoleId == 14 || CurrentUser.RoleId == 20)
            //{
            //     country = Enums.Country.UK.ToString();
            //}
            var pagingService = new PagingService<ConferenceRoom>(request.Start, request.Length);
            //var expr = PredicateBuilder.True<ConferenceRoom>();
            pagingService.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "roomName":
                            return o.OrderByColumn(item, c => c.Name);//room name
                        case "officeName":
                            return o.OrderByColumn(item, c => c.CompanyOffice.Name);//company office name
                        case "themeColor":
                            return o.OrderByColumn(item, c => c.ThemeColor);//office floor name
                    }
                }

                return o.OrderByDescending(c => c.CreatedDate);
            };

            //expr = expr.And(x => x.CompanyOffice.Country == country);
            //pagingService.Filter = expr;

            int totalCount = 0;
            double totalRooms = 0;
            var response = meetingRoomService.GetRooms(out totalCount, pagingService);
            IDictionary<string, object> additionalParameters = new Dictionary<string, object>();
            totalRooms = meetingRoomService.GetTotalRooms(pagingService);
            additionalParameters.Add(new KeyValuePair<string, object>("totalLeave", totalRooms));

            return DataTablesJsonResult(totalCount, request, response.Select((x, index) => new
            {
                rowId = request.Start + index + 1,
                roomId = x.Id,
                roomName = x.Name,
                officeName = x.CompanyOffice.Name,
                themeColor = FirstCharToUpper(x.ThemeColor.Split('-')[1]),
                colorClass = x.ThemeColor,
            }), additionalParameters);
        }

        [HttpGet]
        public ActionResult ManageRoom(int id = 0)
        {
            AddMeetingRoomDto objAddMeetingRoomDto = new AddMeetingRoomDto();
            ViewBag.OfficeList = GetOfficesForManageRoom();
            ViewBag.ColorList = GetColorList();
            if (id > 0)
            {
                var conferenceRoomDB = meetingRoomService.GetConferenceRoom(id);
                objAddMeetingRoomDto.ConferenceRoomId = conferenceRoomDB.Id;
                objAddMeetingRoomDto.CompanyOfficeId = conferenceRoomDB.CompanyOfficeId.ToString();
                objAddMeetingRoomDto.Name = conferenceRoomDB.Name;
                objAddMeetingRoomDto.ThemeColor = conferenceRoomDB.ThemeColor;
                //ViewBag.OfficeFloorList = GetFloorsRooms(Convert.ToInt32(objAddMeetingRoomDto.CompanyOfficeId));
            }
            //else
            //{
            //    ViewBag.OfficeFloorList = GetFloorsRooms();
            //}

            return View(objAddMeetingRoomDto);
        }

        [HttpPost]
        public ActionResult ManageRoom(AddMeetingRoomDto objAddMeetingRoomDto)
        {
            try
            {
                bool IsEdit = (Convert.ToInt64(objAddMeetingRoomDto.ConferenceRoomId) > 0) ? true : false;
                ConferenceRoom conferenceRoomDB = new ConferenceRoom();
                if (IsEdit)
                {
                    conferenceRoomDB = meetingRoomService.GetConferenceRoom(objAddMeetingRoomDto.ConferenceRoomId);
                    conferenceRoomDB.CompanyOfficeId = Convert.ToInt32(objAddMeetingRoomDto.CompanyOfficeId);
                    conferenceRoomDB.ThemeColor = objAddMeetingRoomDto.ThemeColor;
                    conferenceRoomDB.Name = objAddMeetingRoomDto.Name;
                    conferenceRoomDB.ModifiedDate = DateTime.Now;
                    ShowSuccessMessage("Success!", "Record updated successfully", false);
                }
                else
                {
                    conferenceRoomDB.CompanyOfficeId = Convert.ToInt32(objAddMeetingRoomDto.CompanyOfficeId);
                    conferenceRoomDB.ThemeColor = objAddMeetingRoomDto.ThemeColor;
                    conferenceRoomDB.Name = objAddMeetingRoomDto.Name;
                    conferenceRoomDB.ModifiedDate = DateTime.Now;
                    conferenceRoomDB.CreatedDate = DateTime.Now;
                    conferenceRoomDB.IsActive = true;
                    ShowSuccessMessage("Success!", "Record saved successfully", false);
                }
                meetingRoomService.SaveRoom(conferenceRoomDB);
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("RoomList");

        }

        //[HttpGet]
        //public ActionResult GetOfficeFloorsById(int OfficeId)
        //{
        //    var output = meetingRoomService.GetOfficeFloorList().Where(x => x.CompanyOfficeId == OfficeId).ToList();
        //    var response = output.Select(x => new SelectListItem
        //    {
        //        Text = x.Name.ToString(),
        //        Value = x.Id.ToString(),
        //        Selected = false
        //    }).ToList();
        //    response.Insert(0, new SelectListItem() { Text = "--Select--", Value = "" });
        //    return Json(response, JsonRequestBehavior.AllowGet);
        //}

        [HttpGet]
        public ActionResult GetColor()
        {
            var colorList = new List<KeyValuePair<string, string>>();
            colorList.Add(new KeyValuePair<string, string>("Darkpink", "color-darkpink"));
            colorList.Add(new KeyValuePair<string, string>("Green", "color-green"));
            colorList.Add(new KeyValuePair<string, string>("Yellow", "color-yellow"));
            colorList.Add(new KeyValuePair<string, string>("Pink", "color-pink"));
            colorList.Add(new KeyValuePair<string, string>("Orange", "color-orange"));
            colorList.Add(new KeyValuePair<string, string>("Blue", "color-blue"));
            colorList.Add(new KeyValuePair<string, string>("Red", "color-red"));
            //23/01/2019
            colorList.Add(new KeyValuePair<string, string>("Brown", "color-brown"));
            colorList.Add(new KeyValuePair<string, string>("Purple", "color-purple"));
            colorList.Add(new KeyValuePair<string, string>("Indigo", "color-indigo"));
            colorList.Add(new KeyValuePair<string, string>("Gold", "color-gold"));
            colorList.Add(new KeyValuePair<string, string>("DarkSlateBlue", "color-darkslateblue"));
            colorList.Add(new KeyValuePair<string, string>("Chocolate", "color-chocolate"));
            colorList.Add(new KeyValuePair<string, string>("DarkSlateGrey", "color-darkslategrey"));
            colorList.Add(new KeyValuePair<string, string>("Grey", "color-grey"));
            colorList.Add(new KeyValuePair<string, string>("Violet", "color-violet"));
            colorList.Add(new KeyValuePair<string, string>("CadetBlue", "color-cadetblue"));

            var returnTypeList = new List<CommonListType>();
            var list = colorList.Select(x => new SelectListItem
            {
                Text = x.Key.ToString(),
                Value = x.Value.ToString(),
            }).ToList();
            list.Insert(0, new SelectListItem() { Text = "--Select--", Value = "" });
            return Json(list);
        }
        #endregion Manage rooms


        #region Helper
        /// <summary>
        /// Get conference list
        /// </summary>
        /// <param name="selectDefault"></param>
        /// <returns></returns>
        private List<SelectListItem> GetMeetingRooms()
        {
            var conferenceRoomList = meetingRoomService.GetConferenceRoomList();
            var selectRoomsList = conferenceRoomList.Select(x => new SelectListItem
            {
                Text = x.Name.ToString(),
                Value = x.Id.ToString(),
                Group = new SelectListGroup { Name = x.CompanyOffice.Name },
                Selected = false
            }).ToList();
            return selectRoomsList;
        }

        //private List<SelectListItem> GetFloorsRooms(int officeId = 0)
        //{
        //    List<SelectListItem> objSelectListItem = new List<SelectListItem>();
        //    if (officeId > 0)
        //    {
        //        var output = meetingRoomService.GetOfficeFloorList().Where(x => x.CompanyOfficeId == officeId).ToList();
        //        objSelectListItem = output.Select(x => new SelectListItem
        //        {
        //            Text = x.Name.ToString(),
        //            Value = x.Id.ToString(),
        //            Selected = false
        //        }).ToList();
        //        objSelectListItem.Insert(0, new SelectListItem() { Text = "--Select--", Value = "" });
        //    }
        //    else
        //    {
        //        objSelectListItem.Add(new SelectListItem { Text = "--Select--", Value = "0" });
        //    }
        //    return objSelectListItem;
        //}

        /// <summary>
        /// get office list
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetOffices()
        {
            string country = Enums.Country.India.ToString();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                country = Enums.Country.UK.ToString();
            }
            var officeList = meetingRoomService.GetGetOfficeList();
            var selectofficesList = officeList.Where(x => x.Country == country).Select(x => new SelectListItem
            {
                Text = x.Name.ToString(),
                Value = x.Id.ToString(),
            }).ToList();
            return selectofficesList;
        }

        private List<SelectListItem> GetOfficesForManageRoom()
        {
            var officeList = meetingRoomService.GetGetOfficeList();
            var selectofficesList = officeList.Select(x => new SelectListItem
            {
                Text = x.Name.ToString(),
                Value = x.Id.ToString(),
            }).ToList();
            return selectofficesList;
        }

        /// <summary>
        /// get time slot
        /// </summary>
        /// <returns></returns>
        private List<CommonListType> GetTimeSlot()
        {
            var returnTypeList = new List<CommonListType>();
            DateTime start = DateTime.Parse("7:00 AM");
            DateTime end = DateTime.Parse("9:00 PM");
            int count = 1;
            while (end > start)
            {
                returnTypeList.Add(new CommonListType
                {
                    Slot_Id = count,
                    Text = start.ToString("hh:mmtt", CultureInfo.InvariantCulture) + " - " + start.AddMinutes(30).ToString("hh:mmtt", CultureInfo.InvariantCulture),
                    IsSelect = false
                });
                count++;
                start = start.AddMinutes(30);
            }
            return returnTypeList;
        }

        private List<ColorRoomsDto> GetColorsRoomList()
        {
            string country = Enums.Country.India.ToString();
            if (CurrentUser.RoleId == (int)Enums.UserRoles.PMO || CurrentUser.RoleId == (int)Enums.UserRoles.UKPM || CurrentUser.RoleId == (int)Enums.UserRoles.UKBDM)
            {
                country = Enums.Country.UK.ToString();
            }
            var roomColorList = meetingRoomService.GetConferenceRoomList();

            var list = roomColorList.Where(x => x.CompanyOffice.Country == country).Select(x => new ColorRoomsDto
            {
                MeetingRoom = x.Name + " - " + x.CompanyOffice.Name,
                Class = x.ThemeColor.ToString(),
            }).ToList();
            return list;
        }

        private List<SelectListItem> GetColorList()
        {
            var colorList = new List<KeyValuePair<string, string>>();
            colorList.Add(new KeyValuePair<string, string>("Darkpink", "color-darkpink"));
            colorList.Add(new KeyValuePair<string, string>("Green", "color-green"));
            colorList.Add(new KeyValuePair<string, string>("Yellow", "color-yellow"));
            colorList.Add(new KeyValuePair<string, string>("Pink", "color-pink"));
            colorList.Add(new KeyValuePair<string, string>("Orange", "color-orange"));
            colorList.Add(new KeyValuePair<string, string>("Blue", "color-blue"));
            colorList.Add(new KeyValuePair<string, string>("Red", "color-red"));
            //23/01/2019
            colorList.Add(new KeyValuePair<string, string>("Brown", "color-brown"));
            colorList.Add(new KeyValuePair<string, string>("Purple", "color-purple"));
            colorList.Add(new KeyValuePair<string, string>("Indigo", "color-indigo"));
            colorList.Add(new KeyValuePair<string, string>("Gold", "color-gold"));
            colorList.Add(new KeyValuePair<string, string>("DarkSlateBlue", "color-darkslateblue"));
            colorList.Add(new KeyValuePair<string, string>("Chocolate", "color-chocolate"));
            colorList.Add(new KeyValuePair<string, string>("DarkSlateGrey", "color-darkslategrey"));
            colorList.Add(new KeyValuePair<string, string>("Grey", "color-grey"));
            colorList.Add(new KeyValuePair<string, string>("Violet", "color-violet"));
            colorList.Add(new KeyValuePair<string, string>("CadetBlue", "color-cadetblue"));
            var returnTypeList = new List<CommonListType>();
            var list = colorList.Select(x => new SelectListItem
            {
                Text = x.Key.ToString(),
                Value = x.Value.ToString(),
            }).ToList();
            return list;
        }

        private string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                return "";
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        private string GetTimeDifference(DateTime startTime, DateTime endTime)
        {
            string diff = string.Empty;
            TimeSpan span = endTime.Subtract(startTime);
            if (span.Hours > 0)
            {
                if (span.Minutes > 0)
                    diff = span.Hours.ToString() + " hr " + span.Minutes + " min";
                else
                    diff = span.Hours.ToString() + " hr";
            }
            else
            {
                diff = span.Minutes + " min";
            }
            return diff;
        }

        #endregion Helper
    }
}