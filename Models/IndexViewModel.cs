using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace belt_retake.Models
{
    public class IndexViewModel
    {
        public User Register {get;set;}
        public LoginUser Login {get;set;}
    }
}