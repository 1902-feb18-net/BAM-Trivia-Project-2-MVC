using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BAMTriviaProject2MVC.ViewModels
{
    public class UsersViewModel
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [BindNever]
        [Display(Name = "Password")]
        public string PW { get; set; }

        [BindNever]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Credit Card Number")]
        public long? CreditCardNumber { get; set; }

        [BindNever]
        [Display(Name = "Point Total")]
        public int PointTotal { get; set; }

        [BindNever]
        [Display(Name = "Account Type")]
        public bool? AccountType { get; set; }
    }
}
