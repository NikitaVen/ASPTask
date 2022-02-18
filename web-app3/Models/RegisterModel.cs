using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace web_app3.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Enter Name")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

       
    }
}
