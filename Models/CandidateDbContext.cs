using Microsoft.EntityFrameworkCore;

namespace Practise_002.Models
{
    public class CandidateDbContext:DbContext
    {
        public CandidateDbContext(DbContextOptions<CandidateDbContext> options):base(options)
        {
            
        }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Skill> Skills { get; set; }

        public DbSet<CandidateSkill> CandidateSkills { get; set; }


    }
}
