using EMS.Data;
using EMS.Dto;
using EMS.Repo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EMS.Service
{
    public class KRAServices : IKRAServices
    {


        #region "Fields"
        private IRepository<Kra> repoKRA;        
        #endregion

        #region "Cosntructor"
        public KRAServices(IRepository<Kra> repoKRA)
        {
            this.repoKRA = repoKRA;

        }
        #endregion

        public KraDto DeleteList(KraDto kraDto)
        {
             var data = repoKRA.Query()
                .Filter(x => x.DesignationId == kraDto.DesignationId)
                .Get()
                .ToList();

            repoKRA.DeleteBulk(data);

            //foreach (var item in kraDto.dataList)
            //{
            //    Kra obj = new Kra();
            //    obj.DesignationId = kraDto.DesignationId;
            //    obj.CreatedOn = DateTime.Now;
            //    obj.IsActive = true;
            //    obj.Title = kraDto.Title;
            //    obj.DisplayOrder = kraDto.KRAOrderno;
            //    obj.CreatedBy = kraDto.CreatedBy; ;
            //    repoKRA.Delete(obj);
            //}
            return kraDto;          
        }

        public KraDto SaveList(KraDto kraDto)
        {
            //List<Kra> entities = kraDto.dataList.Select(x => new Kra {Title=kraDto.Title, Kradesc=kraDto.Title,CreatedOn=DateTime.Now,IsActive=true, DesignationId = kraDto.DesignationId  }).ToList();                           
            foreach (var item in kraDto.dataList)
            {
                Kra obj = new Kra();
                obj.DesignationId = kraDto.DesignationId;
                obj.CreatedOn = DateTime.Now;
                obj.IsActive = true;
                obj.Title = item.Title;
                obj.DisplayOrder = item.KRAOrderno;
                obj.CreatedBy = kraDto.CreatedBy;
                repoKRA.InsertCallback(obj);
            }
            return kraDto;
        }


        public Kra GetDesignationById(int DesignationId)
        {
            //return repoUserRole.FindById(RoleId);          
            return repoKRA.Query()
                .Include(x => x.Designation)
                .Include(x => x.Designation.Role)
                .Include(x => x.Designation.Role.RoleCategory)
                .Filter(x => x.DesignationId == DesignationId)
                .Get()
                .FirstOrDefault();
        }


        public List<Kra> GetKradataByDesgnationId(int DesignationId)
        {
            return repoKRA.Query().Filter(x => x.DesignationId == DesignationId && x.IsActive == true).Get().OrderBy(x => x.DisplayOrder).ToList();

        }

        #region "Dispose"
        public void Dispose()
        {
            if (repoKRA != null)
            {
                repoKRA.Dispose();
                repoKRA = null;
            }
        }





        #endregion
    }
}
