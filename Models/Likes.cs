using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace belt_retake.Models
{
    public class Like
    {
        [Key]
        public int LikeId {get;set;}
        public int UserId {get;set;}
        public int HobbyId {get;set;}
        public User User {get;set;}
        public Hobby Hobby {get;set;}
        public string Proficiency {get;set;}
    }
}