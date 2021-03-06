using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Company.Data;
using Company.Models;
using Company.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Company.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly CompanyContext _context;

        public EmployeesController(CompanyContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string employeeJobTitle, string searchString)
        {
            /*            var companyContext = _context.Employee.Include(e => e.Branch).Include(e => e.Clients).ThenInclude(e => e.Client);
                        return View(await companyContext.ToListAsync());*/
            IQueryable<Employee> employees = _context.Employee.AsQueryable();
            IQueryable<string> jobTitleQuery = _context.Employee.OrderBy(m => m.JobTitle).Select(m => m.JobTitle).Distinct();
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.FirstName.Contains(searchString) || s.LastName.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(employeeJobTitle))
            {
                employees = employees.Where(x => x.JobTitle == employeeJobTitle);
            }
            employees = employees.Include(m => m.Branch)
            .Include(m => m.Clients).ThenInclude(m => m.Client);
            var employeeJobTItleVM = new EmployeeJobViewModel
            {
                JobTitles = new SelectList(await jobTitleQuery.ToListAsync()),
                Employees = await employees.ToListAsync()
            };
            return View(employeeJobTItleVM);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Branch)
                .Include(e => e.Clients).ThenInclude(e => e.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["BranchId"] = new SelectList(_context.Branch, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,FirstName,LastName,BirthDate,Salary,JobTitle,BranchId")] Employee employee)
        {
            if (file != null)
            {
                string filename = file.FileName;
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                employee.ProfilePicture = filename;
            }
            /*if (ModelState.IsValid)
            {            }*/
            _context.Add(employee);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            ViewData["BranchId"] = new SelectList(_context.Branch, "Id", "Name", employee.BranchId);
            //return View(employee);
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Edit/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            /* var employee = await _context.Employee.FindAsync(id);*/
            var employee = _context.Employee.Where(m => m.Id == id).Include(m => m.Clients).First();
            if (employee == null)
            {
                return NotFound();
            }
            var clients = _context.Client.AsEnumerable();
            clients = clients.OrderBy(s => s.Name);
            EmployeeClientsEditViewModel viewmodel = new EmployeeClientsEditViewModel
            {
                Employee = employee,
                ClientList = new MultiSelectList(clients, "Id", "Name"),
                SelectedClients = employee.Clients.Select(sa => sa.ClientId)
            };

            ViewData["BranchId"] = new SelectList(_context.Branch, "Id", "Name", employee.BranchId);
            return View(viewmodel);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id, IFormFile file, EmployeeClientsEditViewModel viewmodel)

        {
            if (id != viewmodel.Employee.Id)
            {
                return NotFound();
            }

            try
            {
                if (file != null)
                {
                    string filename = file.FileName;
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    viewmodel.Employee.ProfilePicture = filename;
                }

                _context.Update(viewmodel.Employee);
                await _context.SaveChangesAsync();

                IEnumerable<int> listClients = viewmodel.SelectedClients;
                IQueryable<ClientEmployee> toBeRemoved = _context.ClientEmployees.Where(s => !listClients.Contains(s.ClientId) && s.EmployeeId == id);
                _context.ClientEmployees.RemoveRange(toBeRemoved);

                IEnumerable<int> existClients = _context.ClientEmployees.Where(s => listClients.Contains(s.ClientId) && s.EmployeeId == id).Select(s => s.ClientId);
                IEnumerable<int> newActors = listClients.Where(s => !existClients.Contains(s));
                foreach (int actorId in newActors)
                    _context.ClientEmployees.Add(new ClientEmployee { ClientId = actorId, EmployeeId = id });

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(viewmodel.Employee.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            ViewData["BranchId"] = new SelectList(_context.Branch, "Id", "Name", viewmodel.Employee.BranchId);
            return RedirectToAction(nameof(Index));

        }

        // GET: Employees/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Branch)
                .Include(e => e.Clients).ThenInclude(e => e.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}