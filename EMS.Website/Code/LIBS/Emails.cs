using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Business;
using System.Configuration;

namespace EMS.Web.LIBS
{
    public class Emails
    {
        public static void SendEmailLeadUpdate(int LeadId, string UpdateUser, string Status = "updated")
        {
            try
            {
                string Subject = "Lead Update :: " + DateTime.Now.ToString("MMM, dd yyyy hh:mm tt");
                string BodyContent = String.Format("Lead ID#{0} has been {1} by {2}", LeadId, Status, UpdateUser);
                int pmuid = LIBS.Common.PMUid;
                Preference objpre = Preference.GetDataByPmuid(pmuid);
                FlexiMail objMail = new FlexiMail
                {
                    From = SiteKey.From,//"info@dsmanage.com",
                    //To = ConfigurationManager.AppSettings["EmailPM"],//"agarwal.nikhilds@gmail.com"
                    //To = ConfigurationManager.AppSettings["Report-Person"],
                    To = objpre != null ? objpre.EmailPM : "",
                    CC = "",
                    BCC = "",
                    Subject = Subject,
                    MailBodyManualSupply = true,
                    MailBody = BodyContent
                };

                objMail.Send();
            }
            catch { }
        }
        public static void SendEmailTask(string EmailTo, string EmailCC, string Comment, string Ename)
        {
            try
            {
                string Subject = " Urgent Task Comment :: " + DateTime.Now.ToString("MMM, dd yyyy hh:mm tt");
                // string BodyContent = String.Format("Email ID#{0}", EmailTo);
                StringBuilder BodyContent = new StringBuilder();
                BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
                BodyContent.Append("<tr><td>Dear&nbsp;<b>" + Ename + "</b>,<br/></td></tr>");
                BodyContent.Append("<tr><td><br/></td></tr>");
                BodyContent.Append("<tr><td>Please accelerate below task and complete in given time period.</td></tr>");
                BodyContent.Append("<tr><td><br/><br/></td></tr>");
                BodyContent.Append("<tr><td><b>Description: </b> " + Comment + "</td></tr><br/>");
                BodyContent.Append("<tr><td><br/></td></tr>");
                BodyContent.Append("<tr><td>Thanks & Regards<br/>" + SiteSession.SessionUser.Name + "</td></tr>");
                //BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Auto-generated email from internal system, Please do not reply to this email.]</span></td></tr>");
                BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
                BodyContent.Append("</table>");

                FlexiMail objMail = new FlexiMail
                {
                    From = SiteKey.From,
                    // From=EmailCC,
                    To = EmailTo,
                    CC = EmailCC,
                    BCC = "",
                    Subject = Subject,
                    MailBodyManualSupply = true,
                    MailBody = BodyContent.ToString()
                };

                objMail.Send();
            }
            catch { }
        }
        public static void SendEmailTaskCreate(string EmailTo, string EmailCC, string Comment, string Ename)
        {
            try
            {
                string Subject = "Urgent Task Assign :: " + DateTime.Now.ToString("MMM, dd yyyy hh:mm tt");
                // string BodyContent = String.Format("Email ID#{0}", EmailTo);
                StringBuilder BodyContent = new StringBuilder();
                BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
                BodyContent.Append("<tr><td>Dear&nbsp;<b>" + Ename + "</b>,<br/></td></tr>");
                BodyContent.Append("<tr><td><br/></td></tr>");
                BodyContent.Append("<tr><td>A new task has been assigned to you.</td></tr><br/>");
                BodyContent.Append("<tr><td><br/></td></tr>");
                BodyContent.Append("<tr><td><b>Description: </b>" + Comment + "</td></tr><br/>");
                BodyContent.Append("<tr><td><br/></td></tr>");
                BodyContent.Append("<tr><td>Thanks & Regards<br/>" + SiteSession.SessionUser.Name + "</td></tr>");
                //BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Auto-generated email from internal system, Please do not reply to this email.]</span></td></tr>");
                BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
                BodyContent.Append("</table>");

                FlexiMail objMail = new FlexiMail
                {
                    From = SiteKey.From,
                    // From=EmailCC,
                    To = EmailTo,
                    CC = EmailCC,
                    BCC = "",
                    Subject = Subject,
                    MailBodyManualSupply = true,
                    MailBody = BodyContent.ToString()

                };

                objMail.Send();
            }
            catch { }
        }
        public static void SendEmailLeave(string SenderName, string Email_To, string Email_From, string Reason, string HandoverPersonName, string EmailHandover, string WorkHandOver, string ContactNo, string FromDate, string ToDate, string AdjDate, bool IsRevised = false, string status = "", bool IsHalf = false, bool IsUrgent = false, string FilePath = null)
        {
            string Subject = (IsUrgent ? "Urgent " : "Normal ") + (IsHalf ? "Half Day Leave " : "Leave ") + "Request -" + SenderName;
            //string Subject1 = (IsNormal ? "Urgent " : "") + (IsHalf ? "Half Day Leave " : "Leave ") + "Request -" + SenderName;
            StringBuilder BodyContent = new StringBuilder();
            if (status == "")
            {
                BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
                BodyContent.Append("<tr><td>Dear Sir/Madam,</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Hope you are well.</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>This is to request you to kindly grant me " + (IsHalf ? " <b>half day</b>" : "") + (IsUrgent == false ? "Normal" : "Urgent") + " leave from <b>" + Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy") + " (" + Convert.ToString(Convert.ToDateTime(FromDate).DayOfWeek) + ") to " + Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy") + " (" + Convert.ToString(Convert.ToDateTime(ToDate).DayOfWeek) + ")</b>."); // due to " + Reason + ".</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");

                BodyContent.Append("<tr><td><b>Required :</b> " + (IsHalf ? "Half" : "Full") + " Day Leave</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><b>Reason :</b> " + Reason + "</td></tr>");

                if (!string.IsNullOrEmpty(HandoverPersonName))
                {
                    BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                    BodyContent.Append("<tr><td><b>Handover Person : </b> " + HandoverPersonName + "</td></tr>");
                    BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                    BodyContent.Append("<tr><td><b>Handover Description : </b> " + WorkHandOver + "</td></tr>");
                }

                if (!string.IsNullOrEmpty(AdjDate) && AdjDate.Contains('('))
                {
                    BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                    BodyContent.Append("<tr><td><b>Leave adjustment Date : </b> " + AdjDate.Split('(')[0] + "</td></tr>");
                }

                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>In case if you need to contact, please call me on " + ContactNo + ".</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Thanks and I’ll be grateful if you allow the leave.</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");

                BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
                BodyContent.Append("<tr><td><b>" + SenderName + "</b></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                //BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px;'>[Note : Auto-generated email from internal system, Please do not reply to this email.]</span></td></tr>");
                BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
                BodyContent.Append("</table>");
            }
            else if (status == "Cancelled")
            {
                BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
                BodyContent.Append("<tr><td>Dear  Sir/Madam,<br /></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>This is to inform you that <b>" + SenderName + "</b> has cancelled his/her leave request and no action is required.</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><p>Sorry for the trouble.</p></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
                BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                //BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Auto-generated email from internal system, Please do not reply to this email.]</span></td></tr>");
                BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
                BodyContent.Append("</table>");
                Subject = (IsUrgent ? "Urgent " : "Normal ") + "Leave Cancel Request From -" + SenderName;
            }
            //string EmailTo = !String.IsNullOrEmpty(SiteKey.To) ? SiteKey.To.Trim(';') : "";

            string EmailCC = string.Empty; //= !String.IsNullOrEmpty(SiteKey.CC) ? SiteKey.CC.Trim(';') : "";
            string EmailBCC = string.Empty; //!String.IsNullOrEmpty(SiteKey.BCC) ? SiteKey.BCC.Trim(';') : "";
            // Business.Preference ObjPreference = Business.Preference.GetData();
            int pmuid = LIBS.Common.PMUid;
            Business.Preference ObjPreference = Business.Preference.GetDataByPmuid(pmuid);
            if (ObjPreference != null)
            {
                // EmailTo = !String.IsNullOrEmpty(Email_To) ? Email_To : EmailTo;
                EmailCC = !String.IsNullOrEmpty(ObjPreference.EmailPM) ? (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailPM + ";" + ObjPreference.EmailHR : ObjPreference.EmailPM) : (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailHR : "");
                // EmailBCC = ObjPreference.LeaveEmailBCC != null ? ObjPreference.LeaveEmailBCC : EmailBCC;
            }

            FlexiMail objMail = new FlexiMail
            {
                From = Email_From,
                To = Email_To,
                CC = !String.IsNullOrEmpty(EmailCC) ? (!string.IsNullOrEmpty(HandoverPersonName) ? (status == "Cancelled") ? EmailHandover + ";" : string.Empty : string.Empty) + EmailCC.Trim(';') : "",
                BCC = EmailBCC,
                Subject = IsRevised ? "Revised : " + Subject : Subject,
                MailBodyManualSupply = true,
                AttachFile = FilePath != null ? new[] { FilePath } : null,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
        }




        public static void SendEmailLeaveRecieved(string RecipientName, string EmaiTo, string EmailLeadCC, string FromDate, string ToDate, string Status, string Reason, bool IsRevised = false, bool IsUrgent = false, string AdminName = "", bool IsHalf = false, string filepath = null)
        {
            StringBuilder BodyContent = new StringBuilder();
            string Subject = (IsUrgent ? "Urgent " : "Normal ") + "Leave Pending";

            bool isPending = false;

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear  " + RecipientName + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            if (Status == "Pending")
            {
                BodyContent.Append("<tr><td>Your leave request has been forwarded to your senior and is awaiting approval.<br /></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Once they take action on the above, you will receive an email accordingly.<br /></td></tr>");
                //  BodyContent.Append("<tr><td>Remark: " + Reason + "</p><br /><br /></td></tr>");
                isPending = true;
            }
            else if (Status == "Cancel")
            {
                BodyContent.Append("<tr><td>We regret to inform you that your leave request has been cancelled.<br /></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                if (!String.IsNullOrEmpty(Reason))
                {
                    BodyContent.Append("<tr><td>Because, the reason is: </td></tr>");
                    BodyContent.Append("<tr><td> " + Reason + " </td></tr>");
                    BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                }
                BodyContent.Append("<tr><td>Kindly contact your Team Lead for further information.</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><b>Cancellation Done by:</b> " + AdminName + "</td></tr>");
                Subject = (IsUrgent ? "Urgent " : "") + "Leave Cancelled";
            }
            else if (Status == "Approve")
            {
                BodyContent.Append("<tr><td>Congratulation, your leave has been Approved.<br /></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Please make sure you are available on either your normal phone number or the emergency number that you have provided.<br /></td></tr>");
                //BodyContent.Append("<tr><td>Remark: " + Reason + "</p><br /><br /></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><b>Required: </b> " + (IsHalf ? "Half" : "Full") + " Day Leave</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><b>Leave Period: </b> " + Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy") + " to " + Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy") + "</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><b>Approved By: </b> " + AdminName + "</td></tr>");
                if (!string.IsNullOrEmpty(Reason))
                {
                    BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                    BodyContent.Append("<tr><td><b>Remark: </b> " + Reason + "</td></tr>");
                }
                Subject = (IsUrgent ? "Urgent " : "Normal ") + "Leave Approved";
            }
            else if (Status == "Unapprove")
            {
                BodyContent.Append("<tr><td>We regret to inform you that your leave request has been disapproved.<br /></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Because, the reason is: </td></tr>");
                BodyContent.Append("<tr><td> " + (!String.IsNullOrEmpty(Reason) ? Reason : "N/A") + "</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Kindly contact your Team Lead for further information. </td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><b>Disapprovers Name:</b> " + AdminName + "</td></tr>");
                Subject = (IsUrgent ? "Urgent " : "Normal ") + "Leave UnApproved";
            }
            else
            {
                BodyContent.Append("<tr><td>Your leave request has been forwarded to your senior and is awaiting approval.<br /></td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>Once they take action on the above, you will receive an email accordingly.<br /></td></tr>");
                isPending = true;
            }

            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            //BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Auto-generated email from internal system, Please do not reply to this email.]</span></td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message, replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            string EmailFrom = !String.IsNullOrEmpty(SiteKey.From) ? SiteKey.From.Trim(';') : "info@dotsquares.com";
            string EmailCC = EmailLeadCC;
            int pmuid = LIBS.Common.PMUid;
            Business.Preference ObjPreference = Business.Preference.GetDataByPmuid(pmuid);
            //Business.Preference ObjPreference = Business.Preference.GetData();
            if (ObjPreference != null)
            {
                EmailFrom = !String.IsNullOrEmpty(ObjPreference.EmailFrom) ? ObjPreference.EmailFrom : EmailFrom;
                EmailCC = !String.IsNullOrEmpty(ObjPreference.EmailPM) ? (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailPM + ";" + ObjPreference.EmailHR : ObjPreference.EmailPM) : (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailHR : "");
            }
            EmailCC = !string.IsNullOrEmpty(EmailLeadCC) ? EmailLeadCC + (!String.IsNullOrEmpty(EmailCC) ? (";" + EmailCC.Trim(';')) : "") : !String.IsNullOrEmpty(EmailCC) ? EmailCC.Trim(';') : "";

            FlexiMail objMail = new FlexiMail
            {
                From = EmailFrom.Trim(';'),
                To = EmaiTo.Trim(';'),
                CC = isPending ? "" : EmailCC,
                BCC = "",
                Subject = IsRevised ? "Revised :" + Subject : Subject,
                MailBodyManualSupply = true,
                AttachFile = filepath != null ? new[] { filepath } : null,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
        }

        public static void SendEmailHandOver(string HandoverPersonName, string SenderName, string EmailLead, string EmailFrom, string EmailTo, string WorkDescription, string FromDate, string ToDate, bool IsRevised = false, bool IsUrgent = false, string filepath = null)
        {
            StringBuilder BodyContent = new StringBuilder();
            string Subject = (IsUrgent ? "Urgent " : "") + "Work handover details by " + SenderName;

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear  " + HandoverPersonName + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td><b>" + SenderName + "</b> is on leave from <b>" + Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy") + " (" + Convert.ToString(Convert.ToDateTime(FromDate).DayOfWeek) + ")</b> to <b>" + Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy") + " (" + Convert.ToString(Convert.ToDateTime(ToDate).DayOfWeek) + ")</b>. Handover person during the leave or absence is <b>" + HandoverPersonName + "</b>.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td><b>Handover Description :<b> " + WorkDescription + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            // BodyContent.Append("<tr><td>Please co-ordinate both of them and advise to TL/PM for approve this leave.</td></tr>");
            BodyContent.Append("<tr><td>Kindly co-ordinate with each other and then advice TL/PM to approve this leave.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            //BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Auto-generated email from internal system, Please do not reply to this email.]</span></td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            //string EmailFrom = string.Empty;
            string EmailCC = string.Empty;
            int pmuid = LIBS.Common.PMUid;
            Business.Preference ObjPreference = Business.Preference.GetDataByPmuid(pmuid);
            //Business.Preference ObjPreference = Business.Preference.GetData();
            if (ObjPreference != null)
            {
                // EmailFrom = !String.IsNullOrEmpty(ObjPreference.EmailFrom) ? ObjPreference.EmailFrom : EmailFrom;
                EmailCC = !String.IsNullOrEmpty(ObjPreference.EmailPM) ? (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailPM + ";" + ObjPreference.EmailHR : ObjPreference.EmailPM) : (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailHR : "");
            }
            EmailCC = !String.IsNullOrEmpty(EmailLead) ? EmailLead + ";" + EmailCC : EmailCC;

            FlexiMail objMail = new FlexiMail
            {
                From = EmailFrom,
                To = EmailTo,
                CC = !String.IsNullOrEmpty(EmailCC) ? EmailCC.Trim(';') : "",
                BCC = "",
                Subject = (IsRevised ? "Revised: " : "") + Subject,
                MailBodyManualSupply = true,
                AttachFile = filepath != null ? new[] { filepath } : null,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
        }

        public static void SendHandoverACK(string HandoverName, string HandoverEmail, string SenderName, string FromDate, string ToDate, string ApprovalName, string Status, bool IsUrgent = false, bool IsRevised = false)
        {
            StringBuilder BodyContent = new StringBuilder();
            string Subject = (IsUrgent ? "Urgent " : "") + "Leave Status For Handover Work From " + SenderName;

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear  " + HandoverName + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            if (Status == "Approve")
            {
                BodyContent.Append("<tr><td>This is to inform you that leave request for <b>" + SenderName + "</b> has been approved.</td></tr>");
                BodyContent.Append("<tr><td>Please make sure you have thorough understanding of tasks that you will be required to do in his/her absence during <b>" + Convert.ToDateTime(FromDate).ToString("dd/MM/yyyy") + "</b> to <b>" + Convert.ToDateTime(ToDate).ToString("dd/MM/yyyy") + "</b>.");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td> <b>" + SenderName + "</b> will be available on his/her normal phone number.</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td><b>Approved By: </b>" + ApprovalName + "</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            }
            else
            {
                BodyContent.Append("<tr><td>This is to inform you that leave request for <b>" + SenderName + "</b> has been " + (Status == "Unapprove" ? "disapproved" : "cancelled") + ".</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
                BodyContent.Append("<tr><td>No action is required from you now.</td></tr>");
                BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            }

            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            string EmailFrom = SiteKey.From; //"info@dotsquares.com";
            string EmailCC = string.Empty;
            //int pmuid = Convert.ToInt32(SiteSession.SessionUser.Uid);
            //Business.Preference ObjPreference = Business.Preference.GetData();
            //Business.Preference ObjPreference = Business.Preference.GetDataByPmid(pmuid);
            int pmuid = LIBS.Common.PMUid;
            Business.Preference ObjPreference = Business.Preference.GetDataByPmuid(pmuid);
            if (ObjPreference != null)
            {
                EmailFrom = !String.IsNullOrEmpty(ObjPreference.EmailFrom) ? ObjPreference.EmailFrom : EmailFrom;
                // EmailCC = !String.IsNullOrEmpty(ObjPreference.EmailPM) ? (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailPM + ";" + ObjPreference.EmailHR : ObjPreference.EmailPM) : (!String.IsNullOrEmpty(ObjPreference.EmailHR) ? ObjPreference.EmailHR : "");
            }

            FlexiMail objMail = new FlexiMail
            {
                From = EmailFrom,
                To = HandoverEmail,
                CC = "",
                BCC = "",
                Subject = (IsRevised ? "Revised: " : "") + Subject,
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
        }

        public static void SendEmailInduction(string RecipientName, string EmailTo, string UserName, string Password, bool IsSenior = false)
        {
            StringBuilder BodyContent = new StringBuilder();
            string Subject = "Welcome To DotSquares";

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear  " + RecipientName + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>Welcome aboard.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>We at Dotsquares use an employee management system, your login details for which are mentioned below:</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><b>URL:</b> " + SiteKey.DomainName + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><b>Username:</b> " + UserName + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><b>Password:</b> " + Password + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>If you face any issue with the above login details, please contact your Team Lead or Senior for more information.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Also, attached is an Induction Document for you. Kindly go through the same and try to understand all the details mentioned in the same. </td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>If you have any confusion or need further clarification for any point then please contact your Team Lead or Senior.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>We look forward to a long working relationship with you and wish you great success.</td></tr>");

            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            //BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Auto-generated email from internal system, Please do not reply to this email.]</span></td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            string EmailFrom = !String.IsNullOrEmpty(SiteKey.From) ? SiteKey.From.Trim(';') : "info@dotsquares.com";
            string DocName = string.Empty;
            //Business.Preference ObjPreference = Business.Preference.GetData();
            int pmuid = LIBS.Common.PMUid;
            Business.Preference ObjPreference = Business.Preference.GetDataByPmuid(pmuid);
            if (ObjPreference != null)
            {
                EmailFrom = !String.IsNullOrEmpty(ObjPreference.EmailFrom) ? ObjPreference.EmailFrom : EmailFrom;
                DocName = !String.IsNullOrEmpty(ObjPreference.InductionDoc) ? ObjPreference.InductionDoc : "";
            }

            DocName = !String.IsNullOrEmpty(DocName) ? IsSenior ? !String.IsNullOrEmpty(DocName.Split(',')[0]) ? DocName.Split(',')[0] : "" : !String.IsNullOrEmpty(DocName.Split(',')[1]) ? DocName.Split(',')[1] : "" : "";

            FlexiMail objMail = new FlexiMail
            {
                From = EmailFrom,
                To = EmailTo,
                CC = "",
                BCC = "",
                Subject = Subject,
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString(),
                AttachFile = (!String.IsNullOrEmpty(DocName) ? DocName.Split(',').Select(D => HttpContext.Current.Server.MapPath("~/Content/Induction/") + D).ToArray() : null)
            };

            objMail.Send();
        }


        public static void SendBugEmail(string SenderName, string EmailFrom, string SectionName, string SectionDescription, string PagePath, string AttachedFileName)
        {
            //string EmailPM = ConfigurationManager.AppSettings["EmailPM"];
            //Preference objPre = Preference.GetData();
            int pmuid = LIBS.Common.PMUid;
            Preference objPre = Preference.GetDataByPmuid(pmuid);
            string EmailDeveloper = objPre != null ? objPre.EmailDeveloper : "";
            string EmailPM = objPre != null ? objPre.EmailPM : "";
            //"agarwal.nikhilds@gmail.com";
            StringBuilder BodyContent = new StringBuilder();
            string Subject = "Report Bug/Suggestion - " + SenderName;

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear All,<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>" + SenderName + " has raised a bug/suggestion. Kindly have a look and action accordingly.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><b>Module/Section Name: </b>" + SectionName + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><b>Description: </b>" + SectionDescription + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            if (!string.IsNullOrEmpty(PagePath))
            {
                BodyContent.Append("<tr><td><b>Page Path: </b>" + PagePath + "</td></tr>");
            }
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            FlexiMail objMail = new FlexiMail
            {
                From = EmailFrom,
                To = EmailDeveloper,
                CC = EmailPM,
                BCC = "",
                Subject = Subject,
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString(),
                AttachFile = (!String.IsNullOrEmpty(AttachedFileName) ? AttachedFileName.Split(',').Select(D => HttpContext.Current.Server.MapPath("~/Content/BugSuggestions/") + D).ToArray() : null)
            };

            objMail.Send();
        }

        public static void SendBugACKEmail(string SenderName, string SenderEmail)
        {
            //Preference objPre = Preference.GetData();
            int pmuid = LIBS.Common.PMUid;
            Preference objPre = Preference.GetDataByPmuid(pmuid);
            string EmailFrom = objPre.EmailFrom ?? "info@dotsquares.com";

            StringBuilder BodyContent = new StringBuilder();
            string Subject = "Thank you for reporting bug/suggestion - " + SenderName;

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear " + SenderName + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>Thank you for providing your valuable input.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>We will action the same as required.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            FlexiMail objMail = new FlexiMail
            {
                From = EmailFrom,
                To = SenderEmail,
                CC = "",
                BCC = "",
                Subject = Subject,
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
        }

        public static void ForgetEmail(string Name, string EmailTo, string UserName, string Password)
        {
            string From = string.Empty;

            Preference objPreference = Preference.GetData();
            if (objPreference != null)
            {
                From = objPreference.EmailFrom != null ? objPreference.EmailFrom.ToString() : "";
            }
            StringBuilder BodyContent = new StringBuilder();

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear " + Name + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>Below are your credentials-</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><b>Username: </b>" + UserName + "</td></tr>");
            BodyContent.Append("<tr><td><b>Password: </b>" + Password + "</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Kindly change the password once you login.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            FlexiMail objMail = new FlexiMail
            {
                From = From,
                To = EmailTo,
                CC = "",
                BCC = "",
                Subject = "Forgot Passoword",
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();
        }



        public static void SendDeviceEmail(string SubmitedTo, string DeviceName, string SubmitedBy, string SubmitedToEmail)
        {

            //Preference objPre = Preference.GetData();
            int pmuid = LIBS.Common.PMUid;
            Preference objPre = Preference.GetDataByPmuid(pmuid);
            string EmailFrom = "info@dotsquares.com";

            StringBuilder BodyContent = new StringBuilder();
            string Subject = "Device received";

            BodyContent.Append("<table style='width:100%;font-family:Arial; font-size:13px;' cellpadding='0' cellspacing='0'>");
            BodyContent.Append("<tr><td>Dear " + SubmitedTo + ",<br /></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");

            BodyContent.Append("<tr><td>If you have received device <b>" + DeviceName + "</b> from " + SubmitedBy + ", Please update the receive status from your EMS Account. So another user can take this device.</td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");


            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td>Thanks & Regards</td></tr>");
            BodyContent.Append("<tr><td><b>System Administrator</b></td></tr>");
            BodyContent.Append("<tr><td>&nbsp;</td></tr>");
            BodyContent.Append("<tr><td><span style='font-weight:bold; font-family:Arial; font-size:11px; font-style:italic; color:gray;'>[Note : Please do not reply to this automated message; replies to this message are undeliverable. Contact your senior for any query.]</span></td></tr>");
            BodyContent.Append("</table>");

            FlexiMail objMail = new FlexiMail
            {
                From = EmailFrom,
                To = SubmitedToEmail,
                CC = "",
                BCC = "",
                Subject = Subject,
                MailBodyManualSupply = true,
                MailBody = BodyContent.ToString()
            };

            objMail.Send();

        }

        public static void ProjectclosureUpdateEmail(string EmailTo, string EmailCC, string body, string Subject)
        {
            int pmuid = LIBS.Common.PMUid;
            Preference objpre = Preference.GetDataByPmuid(pmuid);
            FlexiMail objMail = new FlexiMail
            {
                From = SiteKey.From,
                //To = objpre != null ? objpre.EmailPM : "",
                To = EmailTo,
                CC = EmailCC,
                BCC = "",
                Subject = Subject,
                MailBodyManualSupply = true,

                MailBody = body
            };

            objMail.Send();
        }



    }
}