using EMS.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMS.Dto
{
    public class VaccinationDto
    {
        public VaccinationDto()
        {
            VaccineTypes = new List<Vaccine>();
        }
        public int Id { set; get; }
        public int UserLoginId { set; get; }
        public int AddedById { set; get; }
        public IEnumerable<DoseType> DoseTypes { set; get; }
        [Required(ErrorMessage = "*required")]
        public byte SelectedDose { set; get; }
        [Required(ErrorMessage = "*required")]
        public int VaccinationTypeId { set; get; }
        [Required(ErrorMessage = "*required")]
        public string Dose1Date { set; get; }
        [Required(ErrorMessage = "*required")]
        public string Dose2Date { set; get; }
        public string FileName { set; get; }

        public string DeclartionTextOne { set; get; }
        public string DeclartionTextTwo { set; get; }
        public string DeclartionTextThree { set; get; }
        public string DeclartionText { set; get; }
        [Required(ErrorMessage = "*required")]
        public IFormFile file { set; get; }

        [Required(ErrorMessage = "*required")]
        public string DeclarationName {set;get;}

        public List<Vaccine> VaccineTypes { get; set; }
    }

    public class DoseType
    {
        public int Id { set; get; }
        public string DoseName { set; get; }
    }

    public class VaccinationReportDto
    {
        public string EmployeeName { set; get; }
        public string ManagerName { set; get; }
        public string PhoneNumber { set; get; }
        public string Email { set; get; }
        public string VaccinationStatus { set; get; }
        //public string Certificate { set; get; }
    }
}
