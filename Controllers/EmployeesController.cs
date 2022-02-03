using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Company.Data;
using Company.Models;
using Company.ViewModels;
using System.Collections.Generic;

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
        public async Task<IActionResult> Index()
        {
            var companyContext = _context.Employee.Include(e => e.Branch).Include(e => e.Clients).ThenInclude(e => e.Client);
            return View(await companyContext.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,BirthDate,Salary,JobTitle,BranchId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "Id", "Name", employee.BranchId);
            return View(employee);
        }

        // GET: Employees/Edit/5
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
/*        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,BirthDate,Salary,JobTitle,BranchId")] Employee employee)
*/        public async Task<IActionResult> Edit(int id, EmployeeClientsEditViewModel viewmodel)

        {
            if (id != viewmodel.Employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["BranchId"] = new SelectList(_context.Branch, "Id", "Name", viewmodel.Employee.BranchId);
            return View(viewmodel);
        }

        // GET: Employees/Delete/5
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
