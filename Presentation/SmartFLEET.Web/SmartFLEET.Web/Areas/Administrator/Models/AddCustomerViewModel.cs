using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using FluentValidation.Attributes;
using SmartFLEET.Web.Areas.Administrator.Validation;

namespace SmartFLEET.Web.Areas.Administrator.Models
{
    [Validator(typeof(AddcustomerValidator))]
    public class AddCustomerViewModel
    {
        public AddCustomerViewModel()
        {
            UserVms = new List<UserVm>();
        }
        public Guid Id { get; set; }
       
        public string Name { get; set; }
       
        public string Email { get; set; }
        
        public string Tel { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
       
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public int CustomerStatus { get; set; }
        public List<string> Countries => CountriesList();
        public static List<string> CountriesList()
        {

            List<string> CountryList = new List<string>();
            CultureInfo[] CInfoList = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo CInfo in CInfoList)
            {
                RegionInfo R = new RegionInfo(CInfo.LCID);
                if (!(CountryList.Contains(R.EnglishName)))
                {
                    CountryList.Add(R.EnglishName);
                }
            }

            CountryList.Sort();
            return CountryList;
        }

        public List<UserVm> UserVms { get; set; }
    }
}