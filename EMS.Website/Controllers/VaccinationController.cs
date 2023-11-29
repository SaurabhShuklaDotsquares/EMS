using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataTables.AspNet.Core;
using EMS.Core;
using EMS.Data;
using EMS.Dto;
using EMS.Service;
using EMS.Web.Code.Attributes;
using EMS.Web.Code.LIBS;
using EMS.Web.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Website.Controllers
{
    [CustomAuthorization()]
    public class VaccinationController : BaseController
    {
        private readonly IVaccinationService _iVccinationService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public VaccinationController(IVaccinationService iVccinationService, IHostingEnvironment hostingEnvironment)
        {
            _iVccinationService = iVccinationService;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new VaccinationDto
            {
                DoseTypes = new List<DoseType>
               {
                new DoseType {Id = 1, DoseName = "Partially"},
                new DoseType {Id = 2, DoseName = "Fully"},
               }
            };

            vm.VaccineTypes = _iVccinationService.GetAllVaccineTypes();
            vm.DeclartionTextOne = "I";
            vm.DeclartionTextTwo = CurrentUser.Name;
            vm.DeclartionTextThree = "hereby, acknowledge that the above information is true and valid to the best of my knowledge.";
            vm.DeclartionText = vm.DeclartionTextOne + " " + vm.DeclartionTextTwo + " " + vm.DeclartionTextThree;
            return View(vm);
        }
        [HttpPost]
        public IActionResult VaccinationFiles(IDataTablesRequest request)
        {
            var pagingServices = new PagingService<VaccinationStatus>(request.Start, request.Length);
            var expr = PredicateBuilder.True<VaccinationStatus>();
            expr = expr.And(e => e.Uid == CurrentUser.Uid);
            pagingServices.Filter = expr;
            pagingServices.Sort = (o) =>
            {
                foreach (var item in request.SortedColumns())
                {
                    switch (item.Name)
                    {
                        case "title":
                            return o.OrderByColumn(item, c => c.VaccinationType.VaccineName);

                        default:
                            return o.OrderByColumn(item, c => c.AddedDate);
                    }
                }
                return o.OrderByDescending(c => c.AddedDate);
            };
            
            int totalCount = 0;
            var response = _iVccinationService.GetVaccinationDocByPaging(out totalCount, pagingServices);
         
            return DataTablesJsonResult(totalCount, request, response.Select((r, Index) => new
            {
                r.Id,
                rowIndex = (Index + 1) + (request.Start),
                r.VaccinationType.VaccineName,
                doseType = r.VaccinationDose1Date.HasValue && r.VaccinationDose2Date.HasValue ? "Fully Vaccinated" : "Partially Vaccinated",
                r.AddedDate,
                r.UpdatedCertificate
            }));
        }

        [HttpPost]
        public IActionResult Index(VaccinationDto vModel)
        {
            bool isSuccess = false;
            if (vModel.file.Length == 0)
                return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = false, ErrorMessage = "file not selected" });

            if (vModel.file.Length > 0)
            {
                vModel.FileName = SaveFile(vModel.file, CurrentUser.Uid, "Upload/Vaccination_Files/", "Certificate");
            }

            vModel.UserLoginId = CurrentUser.Uid;
            var obj = _iVccinationService.Save(vModel);
            if (obj.Id > 0)
            {
                isSuccess = true;
            }

            return NewtonSoftJsonResult(new RequestOutcome<string> { IsSuccess = isSuccess, Message = isSuccess ? "Record has been saved successfully" : "Facing issues while updating the record !!" });
        }


        private  string SaveFile(IFormFile FileUpload, int userId, string Folder, string prefix)
        {
            if (FileUpload != null && FileUpload.Length > 0)
            {
                string fileName = FileUpload.FileName;
                string ext = Path.GetExtension(fileName.ToLower());
                // fileName = Path.GetFileNameWithoutExtension(fileName);
                //fileName = fileName.Replace(' ', '-');
                //var createDirectory = Path.Combine(_hostingEnvironment.WebRootPath, $"Upload\\Vaccination_Files");
                //if (!Directory.Exists(createDirectory))
                //{
                //    Directory.CreateDirectory(createDirectory);
                //}

                fileName = prefix + "-" + userId + ext;
                string FilePath = Path.Combine(ContextProvider.HostEnvironment.WebRootPath + "/" + Folder, fileName);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);
                }

                using (var stream = new FileStream(FilePath, FileMode.Create))
                {
                    FileUpload.CopyTo(stream);
                }
              
                return fileName;
            }

            return String.Empty;
        }
    }
}