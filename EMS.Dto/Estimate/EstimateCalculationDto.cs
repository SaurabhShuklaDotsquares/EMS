using System;
using System.Collections.Generic;
using System.Text;

namespace EMS.Dto
{
    public class EstimateCalculationDto
    {

        public EstimateCalculationDto()
        {
            EstimateForm = new List<EstimateFormDto>();
            CalculationResult = new List<CalculationResultDto>();
            EstimateGraph = new List<EstimateGraphDto>();
            EstimateModel = new List<EstimateModelDto>();
            //EstimateHostingPackage = new List<EstimateHostingPackageDto>();
        }
        public List<EstimateFormDto> EstimateForm { get; set; }

        public List<CalculationResultDto> CalculationResult { get; set; }
        public List<EstimateGraphDto> EstimateGraph { get; set; }

        public List<EstimateModelDto> EstimateModel { get; set; }
        //public List<EstimateHostingPackageDto> EstimateHostingPackage { get; set; }

        public int Id { get; set; }
        public string Command { get; set; }
        public string SearchCRMLeadId { get; set; }
        public string CRMLeadId { get; set; }
        public string HiddenCRMLeadId { get; set; }
        public bool IsOverWrite { get; set; }

        public decimal TotalDays { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string TotalDurationOfProject { get; set; }

        public string SearchStrCRMLeadId { get; set; }
        public bool IsSearchCRM { get; set; }

        public int? CountryId { get; set; }
        public int UKCount { get; set; }
        public int USCount { get; set; }
        public int AUSCount { get; set; }
        public int UAECount { get; set; }
        public int? EstimateModelId { get; set; }
        public string EstimateName { get; set; }
    }
}
