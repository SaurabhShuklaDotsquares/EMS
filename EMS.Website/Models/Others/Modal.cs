using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static EMS.Core.Enums;

namespace EMS.Web.Models.Others
{
    public class Modal
    {
        public string ID { get; set; }
        public string AreaLabeledId { get; set; }
        public ModalSize Size { get; set; }
        public string ModelHeading { get; set; }
        public string Message { get; set; }

        public string ElementId { get; set; }
        public string ModalSizeClass
        {
            get
            {
                switch (this.Size)
                {
                    case ModalSize.Small:
                        return "modal-sm";
                    case ModalSize.Large:
                        return " modal-lg";
                    case ModalSize.XLarge:
                        return " modal-xl";
                    case ModalSize.Medium:
                    default:
                        return "";
                }
            }
        }

        public ModalHeader Header { get; set; }
        public ModalFooter Footer { get; set; }
    }

    public class ModalHeader
    {
        public ModalHeader()
        {
            DisplayCloseButton = true;
        }
        public bool DisplayCloseButton { get; set; }
        public string Heading { get; set; }
    }

    public class ModalFooter
    {
        public ModalFooter()
        {
            SubmitButtonText = "Submit";
            CancelButtonText = "Cancel";
            SubmitButtonID = "btn-submit";
            CancelButtonID = "btn-cancel";
            CloseButtonText = "Close";
            DefaultButtonCss = false;
        }

        public string SubmitButtonText { get; set; }
        public string CancelButtonText { get; set; }
        public string SubmitButtonID { get; set; }
        public string CancelButtonID { get; set; }
        public bool OnlyCancelButton { get; set; }
        public bool DefaultButtonCss { get; set; }
        public string CloseButtonText { get; set; }
    }
}