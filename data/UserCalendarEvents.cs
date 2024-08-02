﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agri_Smart.data
{
    public class UserCalendarEvents
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? Title { get; set; }
        public List<string>? MetaDetails { get; set; }
        public Guid UserID { get; set; }
    }
}
