//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlinePapermarking.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblOnlineStudentMaster
    {
        public int StudentID { get; set; }
        public string StudentInitials { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentLastName { get; set; }
        public string StudentFullName { get; set; }
        public Nullable<int> Gender { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string ContactNo1 { get; set; }
        public string ContactNo2 { get; set; }
        public string StudentEmail { get; set; }
        public Nullable<int> Religion { get; set; }
        public string Nationality { get; set; }
        public byte[] StudentImage { get; set; }
        public string FirstLanguage { get; set; }
        public string StudentNIC { get; set; }
        public string PostalID { get; set; }
        public string PassportID { get; set; }
        public string SessionID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public Nullable<System.DateTime> InactivationDate { get; set; }
        public string PreviousStudentNo { get; set; }
        public string School { get; set; }
        public string PreviousSchoolAddress { get; set; }
        public string AcademicYear { get; set; }
        public string CurrentYear { get; set; }
        public string CurrentGrade { get; set; }
        public string CurrentClass { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string Email { get; set; }
        public string AlternativeEmail { get; set; }
        public Nullable<int> LoginID { get; set; }
        public Nullable<int> OnlineStudentID { get; set; }
        public Nullable<System.DateTime> DownloadDate { get; set; }
        public Nullable<System.DateTime> RegistartionDate { get; set; }
        public string RegistrationFromTime { get; set; }
        public string RegistrationToTime { get; set; }
        public Nullable<bool> IsExported { get; set; }
        public Nullable<int> DistrictId { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public Nullable<int> ExamTypeId { get; set; }
        public Nullable<int> MediumId { get; set; }
        public int PurchasedPaperCount { get; set; }
        public int DownloadedPaperCount { get; set; }
        public bool IsPromoCodeUsed { get; set; }
    
        public virtual ExamType ExamType { get; set; }
        public virtual Medium Medium { get; set; }
        public virtual District District1 { get; set; }
        public virtual Province Province { get; set; }
    }
}