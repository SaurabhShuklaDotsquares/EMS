using System.Collections.Generic;
using System.Linq;
using EMS.Data;
using EMS.Repo;
using EMS.Dto;
using System;
using EMS.Core;
using EMS.Data.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Cryptography;
using Castle.Core.Internal;

namespace EMS.Service
{
    public class StudyDocumentsService : IStudyDocumentsService
    {
        private readonly IRepository<StudyDocuments> _repoStudyDocuments;
        private readonly IRepository<StudyDocumentFiles> _repoStudyDocumentFiles;
        private readonly IRepository<StudyDocumentsPermissions> _repoStudyDocumentsPermissions;
        private readonly IRepository<RequestedStudyDocuments> _repoRequestedStudyDocuments;

        public StudyDocumentsService(IRepository<StudyDocuments> repoStudyDocuments,
            IRepository<StudyDocumentFiles> repoStudyDocumentFiles,
            IRepository<StudyDocumentsPermissions> repoStudyDocumentsPermissions,
            IRepository<RequestedStudyDocuments> repoRequestedStudyDocuments)
        {
            _repoStudyDocuments = repoStudyDocuments;
            _repoStudyDocumentFiles = repoStudyDocumentFiles;
            _repoStudyDocumentsPermissions = repoStudyDocumentsPermissions;
            _repoRequestedStudyDocuments = repoRequestedStudyDocuments;
        }

        public List<StudyDocuments> GetStudyDocuments(PagingService<StudyDocuments> pagingService, out int total)
        {
            return _repoStudyDocuments.Query()
                    .AsTracking()
                    .Filter(pagingService.Filter)
                    .Include(x => x.StudyDocumentFiles)
                    .Include(x => x.Technology)
                    .OrderBy(pagingService.Sort)
                    .GetPage(pagingService.Start, pagingService.Length, out total)
                    .ToList();
        }
        public StudyDocumentsDto GetStudyDocumentsById(int id)
        {
            // main
            var entity = _repoStudyDocuments
                .Query()
                .Get()
                .SingleOrDefault(x => x.Id == id && x.Isdelete == false);
            if (entity == null)
            {
                return null;
            }
            var model = new StudyDocumentsDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                IsActive = entity.Isactive.Value,
                KeyId = entity.Keyid,
                AddedBy = entity.Addedby,
                UpdatedBy = entity.Updatedby,
                AddedDate = entity.Addeddate.Value,
                UpdatedDate = entity.Updateddate.Value,
                IsApproved = entity.Isapproved.Value,
            };

            // child
            model.studyDocumentFiles.AddRange(GetStudyDocumentFilesByStudyDocumentsId(id));
            return model;
        }
        public StudyDocumentsDto GetStudyDocumentsAndFilesByKeyId(string keyId)
        {
            // main
            var entity = _repoStudyDocuments
                .Query()
                .Get()
                .SingleOrDefault(x => x.Keyid == keyId && x.Isdelete == false);
            if (entity == null)
            {
                return null;
            }
            var model = new StudyDocumentsDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                IsActive = entity.Isactive.Value,
                KeyId = entity.Keyid,
                AddedBy = entity.Addedby,
                UpdatedBy = entity.Updatedby,
                AddedDate = entity.Addeddate.Value,
                UpdatedDate = entity.Updateddate.Value,
                IsApproved = entity.Isapproved.Value,
                TechnologyId = entity.Technologyid ?? 0,
            };

            // child
            model.studyDocumentFiles.AddRange(GetStudyDocumentFilesByStudyDocumentsId(entity.Id));
            return model;
        }
        public List<int> GetUnApprovedStudyDocIds(string id)
        {
            var ids = id.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToList();

            var entities = _repoStudyDocuments.Query()
                .Filter(x => x.Isapproved == false && ids.Contains(x.Id.ToString()))
                .Get()
                .Select(x => x.Id)
                .ToList();

            return entities;
        }
        public List<StudyDocumentsDto> GetStudyDocuments()
        {
            var entities = _repoStudyDocuments.Query().Get().ToList();
            List<StudyDocumentsDto> model = new List<StudyDocumentsDto>();
            entities.ForEach(x =>
            {
                model.Add(new StudyDocumentsDto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    IsActive = x.Isactive.Value,
                    KeyId = x.Keyid,
                    AddedBy = x.Addedby,
                    UpdatedBy = x.Updatedby,
                    AddedDate = x.Addeddate.Value,
                    UpdatedDate = x.Updateddate.Value,
                    IsApproved = x.Isapproved.Value
                });
            });
            return model;
        }
        public void SaveStudyDocuments(StudyDocumentsDto model)
        {
            var entity = new StudyDocuments
            {
                Title = model.Title,
                Description = model.Description,
                Isactive = model.IsActive,
                Isdelete = model.IsDelete,
                Isapproved = model.IsApproved,
                Keyid = model.KeyId,
                Addedby = model.AddedBy,
                Updatedby = model.UpdatedBy,
                Addeddate = DateTime.Now,
                Updateddate = DateTime.Now,
                Ip = model.Ip,
                Technologyid = model.TechnologyId
            };
            // child
            model.studyDocumentFiles.ForEach(x =>
            {
                entity.StudyDocumentFiles.Add(new StudyDocumentFiles
                {
                    Id = x.Id,
                    Filename = x.FileName,
                    Displayname = x.DisplayName,
                    Keyid = x.KeyId,
                });
            });
            _repoStudyDocuments.Insert(entity);
        }
        public void UpdateStudyDocumentsOnly(StudyDocumentsDto model)
        {
            var entity = _repoStudyDocuments
                .Query()
                .Get()
                .SingleOrDefault(x => x.Id == model.Id && x.Isdelete == false);

            // main
            entity.Updateddate = DateTime.Now;
            entity.Updatedby = model.UpdatedBy;
            entity.Ip = model.Ip;

            _repoStudyDocuments.Update(entity);
        }
        public StudyDocuments UpdateStudyDocumentsOnlyByKeyId(StudyDocumentsDto model)
        {
            var entity = _repoStudyDocuments
                .Query()
                .Include(x => x.StudyDocumentFiles)
                .Get()
                .SingleOrDefault(x => x.Keyid == model.KeyId && x.Isdelete == false);

            // main
            entity.Updateddate = DateTime.Now;
            entity.Updatedby = model.UpdatedBy;
            entity.Ip = model.Ip;

            _repoStudyDocuments.Update(entity);

            return entity;

        }
        public List<StudyDocumentFiles> UpdateStudyDocuments(StudyDocumentsDto model)
        {
            var entity = _repoStudyDocuments
                .Query()
                .Get()
                .SingleOrDefault(x => x.Id == model.Id && x.Isdelete == false);

            // main
            entity.Title = model.Title;
            entity.Description = model.Description;
            entity.Isactive = model.IsActive;
            entity.Isdelete = model.IsDelete;
            entity.Isapproved = model.IsApproved;
            entity.Updateddate = DateTime.Now;
            entity.Updatedby = model.UpdatedBy;
            entity.Ip = model.Ip;
            entity.Technologyid = model.TechnologyId;

            // child
            var entityFile = _repoStudyDocumentFiles.Query().Filter(x => x.Studydocumentid == model.Id).Get().ToList();

            // get deleteable and editiable ids from model
            var modelEditDelIds = model.studyDocumentFiles.Where(x => x.Id != 0).Select(x => x.Id);

            //deletable data, whose id not exists in model but in table            
            var deletableData = entityFile.Where(x => modelEditDelIds.Contains(x.Id) == false).ToList();

            // editable data, whose id exists in table and model
            var editableData = entityFile.Where(x => modelEditDelIds.Contains(x.Id) == true).ToList();
            foreach (var item in editableData)
            {
                var dto = model.studyDocumentFiles.FirstOrDefault(x => x.Id == item.Id);
                item.Displayname = dto.DisplayName;
            }

            // addable data, whose id in model is 0
            var addableData = new List<StudyDocumentFiles>();
            var addableNodelData = model.studyDocumentFiles.Where(x => x.Id == 0).ToList();
            addableNodelData.ForEach(x =>
            {
                addableData.Add(new StudyDocumentFiles
                {
                    Id = x.Id,
                    Filename = x.FileName,
                    Keyid = x.KeyId,
                    Displayname = x.DisplayName,
                    Studydocumentid = model.Id
                });
            });

            // change entity state
            _repoStudyDocuments.ChangeEntityCollectionState(deletableData, ObjectState.Deleted);
            _repoStudyDocuments.ChangeEntityCollectionState(editableData, ObjectState.Modified);
            _repoStudyDocuments.ChangeEntityCollectionState(addableData, ObjectState.Added);

            _repoStudyDocuments.Update(entity);

            return deletableData;// need to delete from folder
        }
        public void DeleteStudyDocuments(string id, int uid)
        {
            var ids = id.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToList();

            var entities = _repoStudyDocuments.Query()
                .Filter(x => ids.Contains(x.Id.ToString()))
                .Get()
                .ToList();

            entities.ForEach(x =>
            {
                x.Isdelete = true;
                x.Updateddate = DateTime.Now;
                x.Updatedby = uid;
            });

            _repoStudyDocuments.UpdateCollection(entities);
        }

        public StudyDocumentFilesDto GetStudyDocumentFilesById(int id)
        {
            var entity = _repoStudyDocumentFiles
                .Query()
                .Get()
                .SingleOrDefault(x => x.Id == id);
            if (entity == null)
            {
                return null;
            }
            var model = new StudyDocumentFilesDto
            {
                Id = entity.Id,
                FileName = entity.Filename,
                KeyId = entity.Keyid,
                DisplayName = entity.Displayname,
                StudyDodumentId = entity.Studydocumentid.Value
            };
            return model;
        }
        public List<StudyDocumentFilesDto> GetStudyDocumentFilesByStudyDocumentsId(int id)
        {
            var entities = _repoStudyDocumentFiles.Query().Filter(x => x.Studydocumentid == id).Get().ToList();
            List<StudyDocumentFilesDto> model = new List<StudyDocumentFilesDto>();
            entities.ForEach(x =>
            {
                model.Add(new StudyDocumentFilesDto
                {
                    Id = x.Id,
                    FileName = x.Filename,
                    DisplayName = x.Displayname,
                    KeyId = x.Keyid,
                    StudyDodumentId = x.Studydocumentid.Value
                });
            });
            return model;
        }
        public List<StudyDocumentFilesDto> GetStudyDocumentFiles()
        {
            var entities = _repoStudyDocumentFiles.Query().Get().ToList();
            List<StudyDocumentFilesDto> model = new List<StudyDocumentFilesDto>();
            entities.ForEach(x =>
            {
                model.Add(new StudyDocumentFilesDto
                {
                    Id = x.Id,
                    FileName = x.Filename,
                    KeyId = x.Keyid,
                    StudyDodumentId = x.Studydocumentid.Value
                });
            });
            return model;
        }
        public void SaveStudyDocumentFiles(StudyDocumentFilesDto model)
        {
            var entity = new StudyDocumentFiles
            {
                Id = model.Id,
                Filename = model.FileName,
                Keyid = model.KeyId,
                Studydocumentid = model.StudyDodumentId
            };
            _repoStudyDocumentFiles.Insert(entity);
        }
        public void UpdateStudyDocumentFiles(StudyDocumentFilesDto model)
        {
            var entity = _repoStudyDocumentFiles
                .Query()
                .Get()
                .SingleOrDefault(x => x.Id == model.Id);
            entity.Id = model.Id;
            entity.Filename = model.FileName;
            entity.Keyid = model.KeyId;
            entity.Studydocumentid = model.StudyDodumentId;
            _repoStudyDocumentFiles.Update(entity);
        }
        public void SaveStudyDocumentFiles(List<StudyDocumentFilesDto> model)
        {
            List<StudyDocumentFiles> entities = new List<StudyDocumentFiles>();
            entities.ForEach(x =>
            {
                model.Add(new StudyDocumentFilesDto
                {
                    Id = x.Id,
                    FileName = x.Filename,
                    KeyId = x.Keyid,
                    StudyDodumentId = x.Studydocumentid.Value
                });
            });
            _repoStudyDocumentFiles.InsertCollection(entities);
        }
        public void ApproveStudyDocumentBySDId(int id, int uid, bool val)
        {
            var entity = _repoStudyDocuments
                .Query()
                .Get()
                .SingleOrDefault(x => x.Id == id);
            if (entity != null)
            {
                entity.Updatedby = uid;
                entity.Updateddate = DateTime.Now;
                entity.Isapproved = val;
            }
            _repoStudyDocuments.Update(entity);
        }
        public void ApproveStudyDocumentsBySDIds(StudyDocumentsUnapprovedReasonDto model)
        {
            var ids = model.StudyDocumentIds.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToList();

            var entities = _repoStudyDocuments.Query()
                .Filter(x => ids.Contains(x.Id.ToString()))
                .Get()
                .ToList();

            entities.ForEach(x =>
            {
                if (model.UnapprovedReason.Equals("na", StringComparison.InvariantCultureIgnoreCase))
                {
                    x.Unapprovedreson = null;
                }
                else
                {
                    x.Unapprovedreson = model.UnapprovedReason;
                }
                x.Isapproved = model.IsApproved;
                x.Updateddate = DateTime.Now;
                x.Updatedby = model.UpdatedBy;
                x.Ip = model.Ip;
            });

            _repoStudyDocuments.UpdateCollection(entities);
        }

        public List<StudyDocuments> GetStudyDocumentsBySearchText(PagingService<StudyDocuments> pagingService, out int total)
        {
            var entity = _repoStudyDocuments
                .Query()
                .AsTracking()
                .Filter(pagingService.Filter)
                .OrderBy(pagingService.Sort)
                .GetPage(pagingService.Start, pagingService.Length, out total)
                .ToList();

            return entity;
        }
        public List<StudyDocuments> GetAllStudyDocumentsByKeyId(string id)
        {
            var entity = _repoStudyDocuments
                .Query()
                .Filter(x => x.Isdelete == false && x.Isactive == true && x.Keyid == id)
                .Include(x => x.StudyDocumentFiles)
                .Include(x => x.StudyDocumentsPermissions)
                .Include(x => x.Technology)
                .Get()
                .ToList();

            return entity;
        }
        public StudyDocuments GetStudyDocumentsByKeyId(string id)
        {
            // main
            var entity = _repoStudyDocuments
                .Query()
                .Filter(x => x.Keyid == id && x.Isdelete == false && x.Isactive == true)
                .Include(x => x.StudyDocumentFiles)
                .Include(x => x.Technology)
                .Get()
                .SingleOrDefault();

            return entity;
        }
        public StudyDocumentsPermissions GetStudyDocumentsPermissionsBySDId(int id, int uid)
        {
            // main
            var entity = _repoStudyDocumentsPermissions
                .Query()
                .Filter(x => x.Studydocumentsid == id && x.Userid == uid)
                .Get()
                .SingleOrDefault();

            return entity;
        }
        public List<StudyDocumentsPermissions> GetAllStudyDocumentsPermissionsBySDId(int id)
        {
            // main
            var entity = _repoStudyDocumentsPermissions
                .Query()
                .Filter(x => x.Studydocumentsid == id)
                .Get()
                .ToList();

            return entity;
        }
        public StudyDocumentFiles DeleteStudyDocumentFile(string fileKeyId)
        {
            var entity = _repoStudyDocumentFiles.Query().Filter(x => x.Keyid == fileKeyId).Get().SingleOrDefault();
            if (entity != null)
            {
                _repoStudyDocumentFiles.Delete(entity);
            }
            return entity;
        }

        public void SaveStudyDocumentsPermission(StudyDocumentsPermissions entity)
        {
            _repoStudyDocumentsPermissions.Insert(entity);
        }
        public void UpdateStudyDocumentsPermission(StudyDocumentsPermissions entity)
        {
            _repoStudyDocumentsPermissions.Update(entity);
        }
        public void DeleteStudyDocumentsPermission(StudyDocumentsPermissions entity)
        {
            _repoStudyDocumentsPermissions.Delete(entity);
        }
        public void UpdateStudyDocumentsUserPermission(StudyDocumentAddDelUsersPermission model)
        {
            var ids = model.StudyDocumentIds.Split(",").Where(x => !string.IsNullOrEmpty(x)).ToList();

            var entities = _repoStudyDocuments.Query()
                .Filter(x => ids.Contains(x.Id.ToString()) && x.Isactive == true && x.Isdelete == false && x.Isapproved == true)
                .Get()
                .ToList();
            // main
            entities.ForEach(x =>
            {
                x.Updateddate = model.UpdatedDate;
                x.Updatedby = model.UpdatedBy;
                x.Ip = model.Ip;
            });

            // child
            var entityUserPermission = _repoStudyDocumentsPermissions.Query().Filter(x => ids.Contains(x.Studydocumentsid.ToString())).Get().ToList();

            if (model.AddDelPermission)// add permission
            {
                // editable data, whose id exists in table and model same
                var editableData = entityUserPermission.Where(x => model.UserId.Contains(x.Userid) == true).ToList();
                foreach (var item in editableData)
                {
                    item.Startdate = model.StartDate.ToDateTimeDDMMYYYY();
                    item.Enddate = model.EndDate.ToDateTimeDDMMYYYY();
                }

                // addable data, whose id exists in model but not in db
                var addableData = new List<StudyDocumentsPermissions>();
                var addableNodelData = model.UserId.FindAll(x => editableData.Select(x1 => x1.Userid).Contains(x) == false);
                foreach (var sdId in ids)
                {
                    foreach (var userid in addableNodelData)
                    {
                        addableData.Add(new StudyDocumentsPermissions
                        {
                            Startdate = model.StartDate.ToDateTimeDDMMYYYY(),
                            Enddate = model.EndDate.ToDateTimeDDMMYYYY(),
                            Studydocumentsid = int.Parse(sdId),
                            Userid = userid
                        });
                    }
                }

                // change entity state
                _repoStudyDocuments.ChangeEntityCollectionState(editableData, ObjectState.Modified);
                _repoStudyDocuments.ChangeEntityCollectionState(addableData, ObjectState.Added);
            }
            else// delete permission
            {
                // editable data (bcz model only have delete users), whose id exists in table and model same
                var deletableData = entityUserPermission.Where(x => model.UserId.Contains(x.Userid) == true).ToList();

                // change entity state
                _repoStudyDocuments.ChangeEntityCollectionState(deletableData, ObjectState.Deleted);
            }

            _repoStudyDocuments.UpdateCollection(entities);
        }

        public void InsertRequestedStudyDocument(RequestedStudyDocuments entity)
        {
            _repoRequestedStudyDocuments.Insert(entity);
        }


        public void Dispose()
        {
            if (_repoStudyDocuments != null)
            {
                _repoStudyDocuments.Dispose();
            }
            if (_repoStudyDocumentFiles != null)
            {
                _repoStudyDocumentFiles.Dispose();
            }
        }

    }
}
