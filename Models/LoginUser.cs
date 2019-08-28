using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace belt_retake.Models
{
    public class LoginUser
    {
        [Required(ErrorMessage = "Please enter your email")]
        public string Email {get;set;}
        [Required(ErrorMessage = "Please enter your password")]
        [DataType(DataType.Password)]
        public string Password {get;set;}
    }
}