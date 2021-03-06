//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CSS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Agreement
    {
        public Agreement()
        {
            this.Comments = new HashSet<Comment>();
            this.CreditNoteTexts = new HashSet<CreditNoteText>();
            this.RFOUsers = new HashSet<RFOUser>();
            this.RFONumbers = new HashSet<RFONumber>();
        }
    
        public int AgreementNumber { get; set; }
        public int VariantNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<System.DateTime> LastStatusUpdatedDate { get; set; }
        public string AuthorisedBy { get; set; }
        public Nullable<System.DateTime> AuthorisedDate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> LastUpdatedDate { get; set; }
        public string LastUpdatedBy { get; set; }
        public string AMISAccountCode { get; set; }
        public Nullable<int> HandlingCharge { get; set; }
        public Nullable<int> FundingMethodId { get; set; }
        public Nullable<bool> SignRequired { get; set; }
        public Nullable<bool> SignReceived { get; set; }
        public Nullable<System.DateTime> SignReceivedDate { get; set; }
        public string DealerVisibility { get; set; }
        public string Combinability { get; set; }
        public Nullable<int> NumberOfRegistrations { get; set; }
        public Nullable<int> VolumeId { get; set; }
        public Nullable<int> DiscountUnit { get; set; }
        public int VolumeDiscountTypeId { get; set; }
    
        public virtual FundingMethod FundingMethod { get; set; }
        public virtual AgreementStatu AgreementStatu { get; set; }
        public virtual Volume Volume { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<CreditNoteText> CreditNoteTexts { get; set; }
        public virtual ICollection<RFOUser> RFOUsers { get; set; }
        public virtual ICollection<RFONumber> RFONumbers { get; set; }
        public virtual VolumeDiscountType VolumeDiscountType { get; set; }
    }
}
