using System.ComponentModel.DataAnnotations;

namespace Practise_002.Models
{
    public class Candidate
    {
        public int CandidateId { get; set; }
        [Required,StringLength(50),Display(Name = "Candidate Name")]
        public string CandidateName { get; set; } = default!;
        [Required,  Display(Name = "Date of Birth"),DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}",ApplyFormatInEditMode =true)]
        public DateTime dateofBirth { get; set; }
        public string Phone { get; set; } = default!;
        public bool Fresher { get; set; }
        public string Image {  get; set; } = default!;
        public virtual ICollection<CandidateSkill> CandidateSkills { get; set; } = new List<CandidateSkill>();
    }
}
