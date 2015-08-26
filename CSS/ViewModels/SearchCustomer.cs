using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSS.Models;
using System.ComponentModel;

namespace CSS.ViewModels
{
    public class SearchCustomer
    {
        public int? RFONumber { get; set; }
        public CustomerType SelectedCustomerType { get; set; }
        public string Name { get; set; }
        public string PostCode { get; set; }
        public string BusinessArea { get; set; }

        [DisplayName("select")]
        public bool? IsSelected { get; set; }

        public SearchCustomer()
        {
            SelectedCustomerType = new CustomerType();
            Name = string.Empty;
            PostCode = string.Empty;
            BusinessArea = string.Empty;
            IsSelected = false;
        }
    }
}