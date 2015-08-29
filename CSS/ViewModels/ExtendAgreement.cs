using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CSS.ViewModels
{
    public class ExtendAgreement
    {
        public int RFONumber { get; set; }
        public string Name { get; set; }
        public int AgreementNumber { get; set; }
        public int VariantNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:mm/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:mm/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime EndDate { get; set; }
        public int StatusId { get; set; }

        public ExtendAgreement()
        {
        }
    }
}