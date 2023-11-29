using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class Library
    {
        public Library()
        {
            LibraryFile = new HashSet<LibraryFile>();
            LibraryIndustry = new HashSet<LibraryIndustry>();
            LibraryLayoutTypeMapping = new HashSet<LibraryLayoutTypeMapping>();
            LibraryTechnology = new HashSet<LibraryTechnology>();
            LibraryComponent = new HashSet<LibraryComponent>();
            LibraryDownloadHistory= new HashSet<LibraryDownloadHistory>();
            LibrarySearch = new HashSet<LibrarySearch>();
            LibraryTemplate = new HashSet<LibraryTemplate>();
            LibraryComponentFile = new HashSet<LibraryComponentFile>();
        }

        public long Id { get; set; }
        public int? CRMUserId { get; set; }
        public string Title { get; set; }
        public byte LibraryTypeId { get; set; }

        public int? SalesKitId { get; set; }
        public int? CvsId { get; set; }

        public string SearchKeyword { get; set; }
        public string Description { get; set; }
        public DateTime? AddDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsArchive { get; set; }
        public int AddedBy { get; set; }
        public int ModifyByUid { get; set; }
        public bool? IsNda { get; set; }
        public bool? IsLive { get; set; }
        public bool IsFeatured { get; set; }
        public bool? IsInternal { get; set; }
        public bool IsApproved { get; set; }
        public byte? DesignTypeId { get; set; }
        public string Ip { get; set; }
        public Guid KeyId { get; set; }
        public DateTime? LibraryCreatedDate { get; set; }
        public string BannerImage { get; set; }
        public virtual UserLogin AddedByNavigation { get; set; }
        public virtual UserLogin ModifyByU { get; set; }
        public string LiveUrl { get; set; }
        public string OtherIndustry { get; set; }
        public string OtherTechnologyParent { get; set; }
        public string OtherTechnology { get; set; }
        public int? AuthorUid { get; set; }
        public virtual UserLogin AuthorU { get; set; }
        public int? UidBa { get; set; }
        public int? UidTl { get; set; }
        public bool? IsGoodToShow { get; set; }
        public string Version { get; set; }
        public bool IsReadyToUse { get; set; }
        public decimal? IntegrationHours { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? EstimatedHours { get; set; }
        public decimal? ReDevelopmentHours { get; set; }
        public virtual ICollection<LibraryFile> LibraryFile { get; set; }
        public virtual ICollection<LibraryIndustry> LibraryIndustry { get; set; }
        public virtual ICollection<LibraryLayoutTypeMapping> LibraryLayoutTypeMapping { get; set; }
        public virtual ICollection<LibraryTechnology> LibraryTechnology { get; set; }
        public virtual ICollection<LibraryComponent> LibraryComponent { get; set; }
        public virtual ICollection<LibraryDownloadHistory> LibraryDownloadHistory { get; set; }
        public virtual ICollection<LibrarySearch> LibrarySearch { get; set; }
        public virtual UserLogin UidBaNavigation { get; set; }
        public virtual UserLogin UidTlNavigation { get; set; }
        public virtual CvsType Cvs { get; set; }
        public virtual SalesKitType SalesKit { get; set; }
        public virtual ICollection<LibraryTemplate> LibraryTemplate { get; set; }
        public virtual ICollection<LibraryComponentFile> LibraryComponentFile { get; set; }
    }
}
