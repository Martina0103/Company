using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Company.Data;
using Company.Models;
using Company.ViewModels;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Company.Controllers
{
    public class ClientsController : Controller
    {
        private readonly CompanyContext _context;

        public ClientsController(CompanyContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string clientName, string searchString)
        {
            /*var companyContext = _context.Client.Include(e => e.Employees).ThenInclude(e => e.Employee);
            // var companyContext = _context.Client.Include(m => m.Employees).ThenInclude(m => m.Employee);
            return View(await companyContext.ToListAsync());*/

            IQueryable<Client> clients = _context.Client.AsQueryable();
            IQueryable<string> nameQuery = _context.Client.OrderBy(m => m.Name).Select(m => m.Name).Distinct();
            if (!string.IsNullOrEmpty(searchString))
            {
                clients = clients.Where(s => s.Location.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(clientName))
            {
                clients = clients.Where(x => x.Name == clientName);
            }
            clients = clients.Include(m => m.Employees).ThenInclude(m => m.Employee);
            var clientNameVM = new ClientNameViewModel
            {
                Names = new SelectList(await nameQuery.ToListAsync()),
                Clients = await clients.ToListAsync()
            };
            return View(clientNameVM);

        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(m => m.Employees).ThenInclude(m => m.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Location")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var client = await _context.Client.FindAsync(id);

            var client = _context.Client.Where(m => m.Id == id).Include(m => m.Employees).First();

            if (client == null)
            {
                return NotFound();
            }

            var employees = _context.Employee.AsEnumerable();
            employees = employees.OrderBy(s => s.FullName);
            ClientEmployeesEditViewModel viewmodel = new ClientEmployeesEditViewModel
            {
                Client = client,
                EmployeeList = new MultiSelectList(employees, "Id", "FullName"),
                SelectedEmployees = client.Employees.Select(e => e.EmployeeId)
            };

            return View(viewmodel);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ClientEmployeesEditViewModel viewmodel)
        {
            if (id != viewmodel.Client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewmodel.Client);
                    await _context.SaveChangesAsync();

                    IEnumerable<int> listEmployees = viewmodel.SelectedEmployees;
                    IQueryable<ClientEmployee> toBeRemoved = _context.ClientEmployees.Where(s => !listEmployees.Contains(s.EmployeeId) && s.ClientId == id);
                    _context.ClientEmployees.RemoveRange(toBeRemoved);
                    IEnumerable<int> existEmployees = _context.ClientEmployees.Where(s => listEmployees.Contains(s.EmployeeId) && s.ClientId == id).Select(s => s.EmployeeId);
                    IEnumerable<int> newEmployees = listEmployees.Where(s => !existEmployees.Contains(s));
                    foreach (int actorId in newEmployees) _context.ClientEmployees.Add(new ClientEmployee { EmployeeId = actorId, ClientId = id });

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(viewmodel.Client.Id))
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
            return View(viewmodel.Client);
        }

        // GET: Clients/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .Include(m => m.Employees).ThenInclude(m => m.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Client.FindAsync(id);
            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Client.Any(e => e.Id == id);
        }
    }
}