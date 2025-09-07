using contact_manager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace contact_manager.Controllers
{
    public class ContactController : Controller
    {
        private readonly ContactManagementDbContext _context;

        public ContactController(ContactManagementDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var contacts = await _context.Contacts.ToListAsync() ?? new List<Contact>();
            Console.WriteLine($"Loaded {contacts.Count} contacts");
            return View("Index", contacts);
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View("Upload");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile postedFile)
        {

            if (postedFile != null && postedFile.Length > 0)
            {
                using var reader = new StreamReader(postedFile.OpenReadStream());
                var contacts = new List<Contact>();

                await reader.ReadLineAsync();

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    var values = line.Split(',');

                    var contact = new Contact
                    {
                        Name = values[0],
                        DateOfBirth = DateOnly.Parse(values[1]),
                        Married = bool.Parse(values[2]),
                        Phone = values[3],
                        Salary = decimal.Parse(values[4], CultureInfo.InvariantCulture)
                    };

                    bool exists = await _context.Contacts
                        .AsNoTracking()
                        .AnyAsync(c => c.Name == contact.Name
                           && c.DateOfBirth == contact.DateOfBirth
                           && c.Phone == contact.Phone);

                    bool existsInList = contacts.Any(c => c.Name == contact.Name
                                               && c.DateOfBirth == contact.DateOfBirth
                                               && c.Phone == contact.Phone);

                    if (!exists && !existsInList)
                    {
                        contacts.Add(contact);
                    }

                }
                _context.ChangeTracker.Clear();
                await _context.Contacts.AddRangeAsync(contacts);
                await _context.SaveChangesAsync();

                var allContacts = await _context.Contacts.ToListAsync();

                TempData["ToastMessage"] = "The file was uploaded succsesfuly!";
                TempData["ToastType"] = "success";
                return View("Index", allContacts);

            }
            ModelState.AddModelError("", "Please upload a valid CSV file.");
            return View("Upload");

        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            TempData["ToastMessage"] = "The row was deleted succsesfuly!";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] ContactDTO updatedContact)
        {
            if (updatedContact == null)
                return BadRequest("No data received");

            var contact = await _context.Contacts.FindAsync(updatedContact.Id);
            if (contact == null)
                return NotFound();

            contact.Name = updatedContact.Name;
            contact.DateOfBirth = DateOnly.Parse(updatedContact.DateOfBirth);
            contact.Married = updatedContact.Married;
            contact.Phone = updatedContact.Phone;
            contact.Salary = updatedContact.Salary;

            await _context.SaveChangesAsync();

            TempData["ToastMessage"] = "The row was updated succsesfuly!";
            TempData["ToastType"] = "success";
            return Ok();
        }
    }
}
