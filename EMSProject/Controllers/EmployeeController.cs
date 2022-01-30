using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMSProject.Models;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

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
                string fullpath = Path.Combine(path, "Image", files[0].FileName);

                dbPath = "Image/" + files[0].FileName;
                FileStream stream = new FileStream(fullpath, FileMode.Create);

                files[0].CopyTo(stream);
            }


            employees.Image = dbPath;
            EMSDBContext.employees.Add(employees);
            EMSDBContext.SaveChanges();
            return RedirectToAction("ShowAll");
        }

        public IActionResult ShowALl()
        {
            var Emp = EMSDBContext.employees.ToList();
            return View(Emp);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            var emp = EMSDBContext.employees.Where(e => e.Id == Id).FirstOrDefault();
            return View(emp);
        }
        [HttpPost]
        public IActionResult Edit(Employees emp)
        {
            var files = Request.Form.Files;
            string dbPath = string.Empty;
            if (files.Count > 0)
            {
                //Image path Create code
                string path = Environment.WebRootPath;
                string fullpath = Path.Combine(path, "Image", files[0].FileName);

                dbPath = "Image/" + files[0].FileName;
                FileStream stream = new FileStream(fullpath, FileMode.Create);

                files[0].CopyTo(stream);
            }


            emp.Image = dbPath;
            EMSDBContext.Entry(emp).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            EMSDBContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int Id)
        {
            var emp = EMSDBContext.employees.FirstOrDefault(e => e.Id == Id);
            EMSDBContext.employees.Remove(emp);
            EMSDBContext.SaveChanges();
            return RedirectToAction("ShowAll");
        }

    }
}
