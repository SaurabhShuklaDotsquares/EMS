using FluentValidation;
using System;
using System.Collections.Generic;
using EMS.Dto;
using EMS.Dto.LibraryManagement;

namespace EMS.Validations
{
    public class ValidatorFactory : ValidatorFactoryBase
    {
        private static Dictionary<Type, IValidator> _validators = new Dictionary<Type, IValidator>();

        static ValidatorFactory()
        {
            _validators.Add(typeof(IValidator<LoginDto>), new LoginDtoValidator());
            _validators.Add(typeof(IValidator<ForgotPasswordDto>), new ForgotPasswordDtoValidator());
            _validators.Add(typeof(IValidator<UserProfileDto>), new UserProfileDtoValidator());
            _validators.Add(typeof(IValidator<CurrentOpeningDto>), new CurrentOpeningDtoValidator());
            _validators.Add(typeof(IValidator<JobReferFriendDto>), new JobReferFriendDtoValidator());
            _validators.Add(typeof(IValidator<ChangePasswordDto>), new ChangePasswordDtoValidator());
            _validators.Add(typeof(IValidator<DepartmentDto>), new DepartmentDtoValidator());
            _validators.Add(typeof(IValidator<VirtualDeveloperDto>), new VirtualDeveloperDtoValidator());
            _validators.Add(typeof(IValidator<TimeSheetReviewDto>), new TimeSheetReviewDtoValidator());
            _validators.Add(typeof(IValidator<ProjectDto>), new ProjectDtoValidator());
            _validators.Add(typeof(IValidator<LeaveActivityDto>), new LeaveActivityDtoValidator());
            _validators.Add(typeof(IValidator<UploadDocumentDto>), new UploadDocumentDtoValidator());
            _validators.Add(typeof(IValidator<ManageUserDto>), new ManageUserDtoValidator());
            _validators.Add(typeof(IValidator<PreferenceDto>), new PreferenceDtoValidator());
            _validators.Add(typeof(IValidator<ComponentDto>), new ComponentDtoValidator());
            //_validators.Add(typeof(IValidator<LeadClientDto>), new LeadClientDtoValidator());
            //_validators.Add(typeof(IValidator<ConclusionDto>), new ConclusionDtoValidator());
            //_validators.Add(typeof(IValidator<LeadStatusDto>), new LeadStatusDtoValidator());
            //_validators.Add(typeof(IValidator<LeadDto>), new LeadDtoValidator());
            _validators.Add(typeof(IValidator<MedicalDataDto>), new MedicalDataDtoValidator());
            _validators.Add(typeof(IValidator<TaskCommentDto>), new TaskCommentDtoValidator());
            _validators.Add(typeof(IValidator<CreateTaskDto>), new CreateTaskDtoValidator());
            _validators.Add(typeof(IValidator<Commentdto>), new CommentDtoValidator());
            _validators.Add(typeof(IValidator<AddMeetingRoomDto>), new MeetingRoomDtoValidator());
            _validators.Add(typeof(IValidator<ProjectClosureDto>), new ProjectClosureDtoValidator());
            _validators.Add(typeof(IValidator<ProjectClosureStatusDto>), new ProjectClosureStatusDtoValidator());
            _validators.Add(typeof(IValidator<ProjectClousreDetailDto>), new ProjectClosureDetailDtoValidator());
            _validators.Add(typeof(IValidator<ProjectAdditionalSupportDto>), new ProjectAdditionalSupportDtoValidator());
            _validators.Add(typeof(IValidator<InvestmentDto>), new InvestmentDtoValidator());
            _validators.Add(typeof(IValidator<OrgDocumentDto>), new OrgDocumentDtoValidator());
            _validators.Add(typeof(IValidator<ProjectLessonDto>), new ProjectLessonDtoValidator());
            _validators.Add(typeof(IValidator<ProjectLessonLearnedDto>), new ProjectLessonLearnedDtoValidator());
            _validators.Add(typeof(IValidator<ProjectNCLogDto>), new ProjectNCLogDtoValidator());
            _validators.Add(typeof(IValidator<ProjectNCLogAuditeeDto>), new ProjectNCLogAuditeeDtoValidator());
            _validators.Add(typeof(IValidator<PILogRequestDto>), new PILogRequestDtoValidator());
            _validators.Add(typeof(IValidator<PILogApprovalDto>), new PILogApprovalDtoValidator());
            //_validators.Add(typeof(IValidator<MinutesOfMeetingDto>), new MinutesOfMeetingDtoValidator());
            _validators.Add(typeof(IValidator<DeviceDto>), new DeviceDtoValidator());
            _validators.Add(typeof(IValidator<InvoiceDto>), new InvoiceDtoValidator());
            _validators.Add(typeof(IValidator<InvoiceChaseDto>), new InvoiceChaseDtoValidator());
            _validators.Add(typeof(IValidator<InvoiceStatusDto>), new InvoiceStatusDtoValidator());
            _validators.Add(typeof(IValidator<LeadStatusDto>), new LeadStatusDtoValidator());
            _validators.Add(typeof(IValidator<ConclusionDto>), new ConclusionDtoValidator());
            _validators.Add(typeof(IValidator<LeadDto>), new LeadDtoValidator());
            _validators.Add(typeof(IValidator<ExpenseDto>), new ExpenseDtoValidator());
            _validators.Add(typeof(IValidator<DeviceDataDto>), new DeviceDataDtoValidator());
            _validators.Add(typeof(IValidator<AssignDeviceDto>), new AssignDeviceDtoValidator());
            _validators.Add(typeof(IValidator<ReturnDeviceDto>), new ReturnDeviceDtoValidator());
            _validators.Add(typeof(IValidator<AppraiseDto>), new AppraiseDtoValidator());
            _validators.Add(typeof(IValidator<ComplaintDto>), new ComplaintDtoValidator());
            _validators.Add(typeof(IValidator<TechnologyDto>), new TechnologyDtoValidator());
            _validators.Add(typeof(IValidator<BugReportDto>), new BugReportDtoValidator());
            _validators.Add(typeof(IValidator<BucketModelDto>), new BucketModelDtoValidator());
            _validators.Add(typeof(IValidator<LeadStatusModelDto>), new LeadStatusModelDtoValidator());
            _validators.Add(typeof(IValidator<BugStatusDto>), new BugStatusDtoValidator());
            _validators.Add(typeof(IValidator<ProjectClosureReviewDto>), new ProjectClosureReviewDtoValidator());
            _validators.Add(typeof(IValidator<LibraryDto>), new LibraryDtoValidator());
            _validators.Add(typeof(IValidator<LibraryDownloadDto>), new LibraryDownloadDtoValidator());
            _validators.Add(typeof(IValidator<ProjectClientFeedbackDetailDto>), new ProjectClientFeedbackDetailDtoValidator());
            
            _validators.Add(typeof(IValidator<OrgImprovementDto>), new OrgImprovementDtoValidator());
            _validators.Add(typeof(IValidator<EstimateHourDto>), new EstimateHourDtoValidator());
            _validators.Add(typeof(IValidator<EstimateCalculationDto>), new EstimateCalculationDtoValidator());
            _validators.Add(typeof(IValidator<EstimateFormDto>), new EstimateFormDtoValidator());
            _validators.Add(typeof(IValidator<SalesKitTypeDto>), new SalesKitDtoValidator());
            _validators.Add(typeof(IValidator<CvsTypeDto>), new CVsDtoValidator());

            //_validators.Add(typeof(IValidator<EscalationDto>), new EscalationDtoValidator());


        }

        /// <summary>
        /// Creates an instance of a validator with the given type.
        /// </summary>
        /// <param name="validatorType">Type of the validator.</param>
        /// <returns>The newly created validator</returns>
        public override IValidator CreateInstance(Type validatorType)
        {
            IValidator validator;
            if (_validators.TryGetValue(validatorType, out validator))
                return validator;
            return validator;
        }
    }
}