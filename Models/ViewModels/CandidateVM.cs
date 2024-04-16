using System.ComponentModel.DataAnnotations;

namespace Practise_002.Models.ViewModels
{
    public class CandidateVM
    {
        public CandidateVM()
        {
            this.SkillList = new List<int>();
        }
        public int CandidateId { get; set; }
        [Required, StringLength(50), Display(Name = "Candidate Name")]
        public string CandidateName { get; set; } = default!;
        [Required,  Display(Name = "Date of Birth"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime dateofBirth { get; set; }
        public string Phone { get; set; } = default!;
        public bool Fresher { get; set; }
        public IFormFile? ImagePath { get; set; } = default!;
        public string? Image { get; set; }
        public List<int> SkillList { get; set; } 
    }
}

