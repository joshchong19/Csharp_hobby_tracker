using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace belt_retake.Models
{
    public class Hobby
    {
        [Key]
        public int HobbyId {get;set;}
        [Required(ErrorMessage = "Name is required")]
        public string Name {get;set;}
        [Required(ErrorMessage = "Description is required")]
        public string Description {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int UserId {get;set;}
        public User Creator {get;set;}
        public List<Like> Likedby {get;set;}
    }
}