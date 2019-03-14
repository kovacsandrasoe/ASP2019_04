using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoIdentity.Data;
using TodoIdentity.Models;

namespace TodoIdentity.Controllers
{
    public class HomeController : Controller
    {

        RoleManager<IdentityRole> rolemanager; //szerepkörkezelő
        UserManager<IdentityUser> usermanager; //felhasználókezelő
        ApplicationDbContext database; //adatbáziskezelés

        public HomeController(RoleManager<IdentityRole> rolemanager,
            UserManager<IdentityUser> usermanager,
            ApplicationDbContext database)
        {
            this.usermanager = usermanager;
            this.rolemanager = rolemanager;
            this.database = database;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        [Authorize] //csak regisztráltaknak!
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        //todo kezelő action-ök

        [Authorize]
        [HttpGet]
        public IActionResult GetMyTodos()
        {
            var myself = this.User;
            var my_id = usermanager.GetUserId(myself);


            var mytodos = database.Todos.Where(t => t.TodoOwner == my_id);

            return View(mytodos);
        }

        [Authorize]
        [HttpGet]
        public IActionResult DeleteTodo(int id, bool admin = false)
        {
            var todo = database.Todos.Where(t => t.TodoId == id).FirstOrDefault();

            database.Todos.Remove(todo);
            database.SaveChanges();

            if (admin)
            {
                return RedirectToAction("Admin");
            }
            else
            {
                return RedirectToAction("GetMyTodos");
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddTodo()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddTodo(string todoname)
        {
            Todo t = new Todo();
            t.TodoName = todoname;

            //szerezzük meg a jelenleg belépett actual user
            //id-ját

            var myself = this.User;
            var my_id = usermanager.GetUserId(myself);

            t.TodoOwner = my_id;
            database.Todos.Add(t);
            database.SaveChanges();

            return RedirectToAction("GetMyTodos");
        }

        [Authorize(Roles = "Admins")] //Admins nevű csoport férhet hozzá
        public IActionResult Admin()
        {
            return View(database.Todos);
        }










        public async Task<IActionResult> Setup()
        {
            //csak egyszer hívjuk meg kézzel

            //1. Létrehozunk egy admin szerepkört

            IdentityRole adminrole = new IdentityRole()
            {
                Name = "Admins"
            };

            await rolemanager.CreateAsync(adminrole); //létrejön a szerepkör

            //2. A legelső user-t beletesszük az admin szerepkörbe

            var firstuser = usermanager.Users.FirstOrDefault(); //első user

            await usermanager.AddToRoleAsync(firstuser, "Admins"); //adminsokhoz adjuk


            return RedirectToAction("Index");
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
