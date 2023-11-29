using System;
using System.Collections.Generic;

namespace EMS.Data
{
    public partial class SelfDeclaration
    {
        public int Id { get; set; }
        public string EmpCode { get; set; }
        public string Name { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string MobileNumber { get; set; }
        public string EmailPersonal { get; set; }
        public string Address { get; set; }
        public string LocalAddress { get; set; }
        public bool RecentlyInJaipur { get; set; }
        public string Location { get; set; }
        public string Purpose { get; set; }
        public bool HasDiseaseSymptoms { get; set; }
        public DateTime? SymptomsStartDate { get; set; }
        public DateTime? SymptomsEndDate { get; set; }
        public int Uid { get; set; }
        public string DeclarationName { get; set; }
        public string Ip { get; set; }
        public DateTime? AddDate { get; set; }

        public virtual UserLogin U { get; set; }

        public bool? HasCoughSymptoms { get; set; }
        public bool? HasFeverSymptoms { get; set; }
        public bool? HasBreathingSymptoms { get; set; }
        public bool? HasSmellAndTasteSymptoms { get; set; }
        public bool? HasDiabetesProblem { get; set; }
        public bool? HasHypertensionProblem { get; set; }
        public bool? HasLungProblem { get; set; }
        public bool? HasHeartProblem { get; set; }
        public bool? HasKidneyProblem { get; set; }
        public bool? HasTraveledInternationally { get; set; }
    }
}
