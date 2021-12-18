using DAL.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PL.Models
{
    public class VotingViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "MinimalForPercentage")]
        public decimal? MinimalForPercentage { get; set; }

        [Required]
        [Display(Name = "MinimalAttendancePercentage")]
        public decimal? MinimalAttendancePercentage { get; set; }

        public decimal ForPercentage { 
            get {
                if (VotesTotally != 0)
                    return (decimal)VotesFor / VotesTotally * 100.0m;
                else
                    return 0m;
            }
        }

        public decimal AttendancePercentage { get; set; }

        public DateTime? CreationDate { get; set; }

        [Required]
        [Display(Name = "CompletionDate")]
        public DateTime? CompletionDate { get; set; }

        [Required]
        [Display(Name = "VisibilityTerm")]
        public short? VisibilityTerm { get; set; }

        public string Status { get; set; }


        public string Author { get; set; }

        public string AuthorId { get; set; }

        public string StatusSetter { get; set; }

        public string StatusSetterId { get; set; }

        [Required]
        [Display(Name = "Level")]
        public string Level { get; set; }


        public int VotesFor { get; set; }

        public int VotesTotally { get; set; }

        public bool IsSuccessfulNow { get; set; }

        [Display(Name = "UserVote")]
        public string UserVote { get; set; }

        public bool IsUserAbleToVote { get; set; }

        public bool IsUserAbleToChangeStatus { get; set; }

        public UserAsAuthorViewModel User { get; set; }

        public bool IsUserAbleToEdit { get; set; }
    }
}
