using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practise_002.Models;
using Practise_002.Models.ViewModels;

namespace Practise_002.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly CandidateDbContext _Context;
        private readonly IWebHostEnvironment _he;
        public CandidatesController(CandidateDbContext context,IWebHostEnvironment he)
        {
            _Context = context;
            _he = he;
        }
        public async Task<IActionResult> Index(string usertext, string sortOrder, int page)
        {
            ViewBag.sWord = usertext;
            ViewBag.sortParam = string.IsNullOrEmpty(sortOrder) ? "desc_name" : "";

            IQueryable<Candidate> emp = _Context.Candidates.Include(x => x.CandidateSkills).ThenInclude(x => x.Skill);

            if (!string.IsNullOrEmpty(usertext))
            {

                usertext = usertext.ToLower();
                emp = emp.Where(e => e.CandidateName.ToLower().Contains(usertext));
            }
            switch (sortOrder)
            {
                case "desc_name":
                    emp = emp.OrderByDescending(e => e.CandidateName);
                    break;
                default:
                    emp = emp.OrderBy(e => e.CandidateName);
                    break;
            }
            ViewBag.count = emp.Count();
            if (page <= 0) page = 1;
            int pageSize = 2;
            ViewBag.pSize = pageSize;
            return View(await PaginatedList<Candidate>.CreateAsync(emp, page, pageSize));
        }
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult AddNewSkills(int? id)
        {
            ViewBag.skill = new SelectList(_Context.Skills, "SkillId", "SkillName", id.ToString() ?? "");
            return PartialView("_addNewSills");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CandidateVM candidateVM, int[] skillId)
        {
            if(ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateName=candidateVM.CandidateName,
                    dateofBirth= candidateVM.dateofBirth,
                    Phone=candidateVM.Phone,
                    Fresher=candidateVM.Fresher,
                };
                //for Image
                var file = candidateVM.ImagePath;
                string webroot = _he.WebRootPath;
                string folder = "Images";
                string imgFileName = Path.GetFileName(candidateVM.ImagePath.FileName);
                string fileToSave = Path.Combine(webroot, folder, imgFileName);
                if (file != null)
                {
                    using (var stream = new FileStream(fileToSave, FileMode.Create))
                    {
                        candidateVM.ImagePath.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }
                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {
                        Candidate = candidate,
                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    _Context.CandidateSkills.Add(candidateSkill);
                }
                await _Context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View();
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var candidate = await _Context.Candidates.FirstOrDefaultAsync(x => x.CandidateId == id);

            CandidateVM candidateVM = new CandidateVM()
            {
                CandidateId = candidate.CandidateId,
                CandidateName = candidate.CandidateName,
                dateofBirth = candidate.dateofBirth,
                Phone = candidate.Phone,
                Image = candidate.Image,
                Fresher = candidate.Fresher
            };
            var existSkill = _Context.CandidateSkills.Where(x => x.CandidateId == id).ToList();
            foreach (var item in existSkill)
            {
                candidateVM.SkillList.Add(item.SkillId);
            }
            return View(candidateVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CandidateVM candidateVM, int[] skillId)
        {
            if (ModelState.IsValid)
            {
                Candidate candidate = new Candidate()
                {
                    CandidateId = candidateVM.CandidateId,
                    CandidateName = candidateVM.CandidateName,
                    dateofBirth = candidateVM.dateofBirth,
                    Phone = candidateVM.Phone,
                    Fresher = candidateVM.Fresher,
                    Image = candidateVM.Image
                };
                var file = candidateVM.ImagePath;
                if (file != null)
                {
                    string webroot = _he.WebRootPath;
                    string folder = "Images";
                    string imgFileName = Path.GetFileName(candidateVM.ImagePath.FileName);
                    string fileToSave = Path.Combine(webroot, folder, imgFileName);
                    using (var stream = new FileStream(fileToSave, FileMode.Create))
                    {
                        candidateVM.ImagePath.CopyTo(stream);
                        candidate.Image = "/" + folder + "/" + imgFileName;
                    }
                }

                var existSkill = _Context.CandidateSkills.Where(x => x.CandidateId == candidate.CandidateId).ToList();
                foreach (var item in existSkill)
                {
                    _Context.CandidateSkills.Remove(item);
                }
                foreach (var item in skillId)
                {
                    CandidateSkill candidateSkill = new CandidateSkill()
                    {

                        CandidateId = candidate.CandidateId,
                        SkillId = item
                    };
                    _Context.CandidateSkills.Add(candidateSkill);
                }
                _Context.Update(candidate);
                await _Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        public IActionResult  Delete(int id)
        {
            var candidate = _Context.Candidates.Find(id);
            if(candidate != null)
            {
                var deleteSkill=_Context.CandidateSkills.Where(x=>x.SkillId==id).ToList();
                _Context.CandidateSkills.RemoveRange(deleteSkill);
                _Context.Candidates.Remove(candidate);
               _Context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}

