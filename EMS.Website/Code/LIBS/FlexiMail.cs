using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Net.Mail;

namespace EMS.Web.Code.LIBS
{
    public class FlexiMail
    {
        #region Constructors-Destructors

        public FlexiMail()
        {
            //set defaults

            myEmail = new System.Net.Mail.MailMessage();
            _MailBodyManualSupply = false;
        }

        //public void Dispose()
        //{
        //    base.Finalize();
        //    myEmail.Dispose();
        //}
        #endregion

        #region  Class Data
        private string _From;

        // private string _FromList;
        private string _FromName;

        private string _To;
        private string _ToList;
        private string _Subject;
        private string _CC;
        private string _CCList;
        private string _BCC;
        private string _TemplateDoc;
        private string[] _ArrValues;
        private string _BCCList;
        private bool _MailBodyManualSupply;
        private string _MailBody;
        private string[] _Attachment;
        private Attachment[] _MailAttachment;
        private MailMessage myEmail;

        #endregion

        #region Propertie

        public string From
        {
            set { _From = value; }
        }

        public string FromName
        {
            set { _FromName = value; }
        }

        public string To
        {
            set { _To = value; }
        }

        public string Subject
        {
            set { _Subject = value; }
        }

        public string CC
        {
            set { _CC = value; }
        }

        public string BCC
        {
            set { _BCC = value; }
        }

        public bool MailBodyManualSupply
        {
            set { _MailBodyManualSupply = value; }
        }

        public string MailBody
        {
            set { _MailBody = value; }
        }

        public string EmailTemplateFileName
        {
            //FILE NAME OF TEMPLATE ( MUST RESIDE IN ../EMAILTEMPLAES/ FOLDER )
            set { _TemplateDoc = value; }
        }

        public string[] ValueArray
        {
            //ARRAY OF VALUES TO REPLACE VARS IN TEMPLATE
            set { _ArrValues = value; }
        }

        public string[] AttachFile
        {
            set { _Attachment = value; }
        }

        public Attachment[] MailAttachments
        {
            set { _MailAttachment = value; }
        }

        #endregion

        #region SEND EMAIL

        public void Send()
        {
            SetMailProperties();

            SmtpClient client = new SmtpClient();
            client.Host = SiteKey.SMTPHost;
            client.Credentials = new System.Net.NetworkCredential(SiteKey.SMTPUserName, SiteKey.SMTPPassword);
            client.Port = Convert.ToInt32(SiteKey.SMTPPort);

            client.Send(myEmail);
        }

        public void SendCalendarEvent(string eventString)
        {
            var htmlContentType = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Text.Html);
            var avHtmlBody = AlternateView.CreateAlternateViewFromString(_MailBody, htmlContentType);
            myEmail.AlternateViews.Add(avHtmlBody);

            System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType("text/calendar");
            ct.Parameters.Add("method", "REQUEST");
            ct.Parameters.Add("name", "meeting.ics");
            AlternateView avCalendar = AlternateView.CreateAlternateViewFromString(eventString, ct);
            myEmail.AlternateViews.Add(avCalendar);

            //set mandatory properties
            if (_FromName == "")
                _FromName = _From;
            myEmail.From = new MailAddress(_From, _FromName);
            myEmail.Subject = _Subject;

            //---Set recipients in To List
            _ToList = _To.Replace(";", ",");
            if (_ToList != "")
            {
                string[] arr = _ToList.Split(',');
                myEmail.To.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        myEmail.To.Add(new MailAddress(address));
                    }
                }
                else
                {
                    myEmail.To.Add(new MailAddress(_ToList));
                }
            }

            //---Set recipients in CC List
            _CCList = _CC.Replace(";", ",");
            if (_CCList != "")
            {
                string[] arr = _CCList.Split(',');
                myEmail.CC.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        myEmail.CC.Add(new MailAddress(address));
                    }
                }
                else
                {
                    myEmail.CC.Add(new MailAddress(_CCList));
                }
            }

            //---Set recipients in BCC List
            _BCCList = _BCC.Replace(";", ",");
            if (_BCCList != "")
            {
                string[] arr = _BCCList.Split(',');
                myEmail.Bcc.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        myEmail.Bcc.Add(new MailAddress(address));
                    }
                }
                else
                {
                    myEmail.Bcc.Add(new MailAddress(_BCCList));
                }
            }

            // set attachment
            if (_Attachment != null)
            {
                for (int i = 0; i < _Attachment.Length; i++)
                {
                    if (_Attachment[i] != null)
                        myEmail.Attachments.Add(new Attachment(_Attachment[i]));
                }
            }

            SmtpClient client = new SmtpClient();
            client.Host = SiteKey.SMTPHost;
            client.Credentials = new System.Net.NetworkCredential(SiteKey.SMTPUserName, SiteKey.SMTPPassword);
            client.Port = Convert.ToInt32(SiteKey.SMTPPort);

            client.Send(myEmail);
        }

        #endregion

        #region Helpers

        public string GetHtml(string argTemplateDocument)
        {
            int i;
            StreamReader filePtr;
            string fileData = argTemplateDocument;
            var templatePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/EmailTemplate/");
            filePtr = File.OpenText(templatePath + argTemplateDocument);
            //filePtr = File.OpenText(ConfigurationSettings.AppSettings["EMLPath"] + argTemplateDocument);
            fileData = filePtr.ReadToEnd();

            filePtr.Close();
            filePtr = null;
            if ((_ArrValues == null))
            {
                return fileData;
            }
            else
            {
                //fileData = fileData.Replace("##user##", _ArrValues[0].ToString());
                //fileData = fileData.Replace("##question##", _ArrValues[1].ToString());

                for (i = _ArrValues.GetLowerBound(0); i <= _ArrValues.GetUpperBound(0); i++)
                {
                    fileData = fileData.Replace("@v" + i.ToString() + "@", (string)_ArrValues[i]);
                }
                return fileData;
            }
        }

        public void SetMailProperties()
        {
            myEmail.IsBodyHtml = true;

            // Set mandatory properties
            if (_FromName == "")
                _FromName = _From;
            myEmail.From = new MailAddress(_From, _FromName);
            myEmail.Subject = _Subject;

            // Set recipients in To List
            _ToList = _To.Replace(";", ",");
            if (_ToList != "")
            {
                string[] arr = _ToList.Trim().TrimEnd(',').Split(',');
                myEmail.To.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        myEmail.To.Add(new MailAddress(address));
                    }
                }
                else
                {
                    myEmail.To.Add(new MailAddress(_ToList));
                }
            }

            // Set recipients in CC List
            _CCList = (_CC ?? "").Replace(";", ",");
            if (_CCList != "")
            {
                string[] arr = _CCList.Trim().TrimEnd(',').Split(',');
                myEmail.CC.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        myEmail.CC.Add(new MailAddress(address));
                    }
                }
                else
                {
                    myEmail.CC.Add(new MailAddress(_CCList));
                }
            }

            // Set recipients in BCC List
            _BCCList = (_BCC ?? "").Replace(";", ",");
            if (_BCCList != "")
            {
                string[] arr = _BCCList.Trim().TrimEnd(',').Split(',');
                myEmail.Bcc.Clear();
                if (arr.Length > 0)
                {
                    foreach (string address in arr)
                    {
                        myEmail.Bcc.Add(new MailAddress(address));
                    }
                }
                else
                {
                    myEmail.Bcc.Add(new MailAddress(_BCCList));
                }
            }

            // Set mail body
            if (_MailBodyManualSupply)
            {
                myEmail.Body = _MailBody;
            }
            else
            {
                myEmail.Body = GetHtml(_TemplateDoc);
            }

            // Set Attachments
            if (_Attachment != null)
            {
                for (int i = 0; i < _Attachment.Length; i++)
                {
                    if (_Attachment[i] != null)
                        myEmail.Attachments.Add(new Attachment(_Attachment[i]));
                }
            }

            // Set Mail Attachments
            if (_MailAttachment != null && _MailAttachment.Length > 0)
            {
                for (int i = 0; i < _MailAttachment.Length; i++)
                {
                    if (_MailAttachment[i] != null)
                    {
                        myEmail.Attachments.Add(_MailAttachment[i]);
                    }
                }
            }
        }

        #endregion
    }
}