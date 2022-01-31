using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMSProject.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace EMSProject.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private EMSDBContext EMSDBContext;
        public IHostingEnvironment Environment { get;}

        public EmployeeController(EMSDBContext DbContext, IHostingEnvironment environment)
        {
            Environment = environment;
            EMSDBContext = DbContext;
        }



        public IActionResult Index()
        { 
            return View();
        }

        [HttpPost]
        public IActionResult Index(Employees employees)
        {
            var files = Request.Form.Files; 
            string dbPath = string.Empty;
            if (files.Count > 0)
            {
                //Image path Create code
                string path = Environment.WebRootPath;
                string fullpath = Path.Combine(path, "image", files[0].FileName);

                dbPath = "image/" + files[0].FileName;
                FileStream stream = new FileStream(fullpath, FileMode.Create);

                files[0].CopyTo(stream);
            }


            employees.ImageUrl = dbPath;
            EMSDBContext.employees.Add(employees);
            EMSDBContext.SaveChanges();
            return RedirectToAction("ShowAll");
        }

        public IActionResult ShowALl()
        {
            var Emp = EMSDBContext.employees.ToList();
            return View(Emp);
        }

       

        

        public IActionResult Edit(int Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var Employee = EMSDBContext.employees.Find(Id);
            if (Employee == null)
            {
                return NotFound();
            }
            return View(Employee);
        }


        [HttpPost]
        public IActionResult Edit(int Id, [Bind("Id,FirstName,LastName,Email,Mobile")] Employees employees)
        {
            if (Id != employees.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var files = Request.Form.Files;
                    string dbPath = string.Empty;
                    if (files.Count > 0)
                    {
                        //Image path Create code
                        string path = Environment.WebRootPath;
                        string fullpath = Path.Combine(path, "image", files[0].FileName);

                        dbPath = "image/" + files[0].FileName;
                        FileStream stream = new FileStream(fullpath, FileMode.Create);

                        files[0].CopyTo(stream);
                    }
                    employees.ImageUrl = dbPath;
                    EMSDBContext.Update(employees);
                    EMSDBContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!employeeExists(employees.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ShowALl));
            }
            return View(employees);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = EMSDBContext.employees
                .FirstOrDefault(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int Id)
        {
            var Emp = EMSDBContext.employees.Find(Id);
            EMSDBContext.employees.Remove(Emp);
            EMSDBContext.SaveChanges();
            return RedirectToAction(nameof(ShowALl));
        }
        private bool employeeExists(int id)
        {
            return EMSDBContext.employees.Any(e => e.Id == id);
        }

    }
}
