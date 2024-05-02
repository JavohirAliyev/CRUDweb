using iTransition4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iTransition4.Data;

namespace iTransition4.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly ApplicationDbContext _dbContext;
        private static User? _thisUser;

        public UsersController(ApplicationDbContext dbContext)
        {

            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel u)
        {
            
            
            if (String.IsNullOrEmpty(u.FullName) || String.IsNullOrEmpty(u.Password))
            {

                ModelState.AddModelError(string.Empty, "Full name and password cannot be empty");

            } else if (!String.IsNullOrEmpty(u.FullName) && !String.IsNullOrEmpty(u.Password))
            {
                var user = new User
                {
                    FullName = u.FullName
                ,
                    Email = u.Email
                ,
                    Password = u.Password
                ,
                    RegistrationTime = DateTime.Now
                };
                if (_thisUser == null)
                {
                    _thisUser = user;

                }
                await _dbContext.UsersDb.AddAsync(user);
                _dbContext.SaveChanges();
                return RedirectToAction("GetAll", "Users");
            }
            return View(u);
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                if (_thisUser is not null && _thisUser.IsBlocked == Status.Active)
                {
                var users = await _dbContext.UsersDb.ToListAsync();
                return View(users);
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _dbContext.UsersDb.FindAsync(id);
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User viewModel)
        {
            User? user = await _dbContext.UsersDb.FindAsync(viewModel.Id);

            if (String.IsNullOrEmpty(viewModel.FullName) || String.IsNullOrEmpty(viewModel.Password))
            {
                ModelState.AddModelError(string.Empty, "Full name and password cannot be empty");
                return View(viewModel);
            } else if (user is not null)
            {
                user.FullName = viewModel.FullName;
                user.Email = viewModel.Email;
                user.Password = viewModel.Password;

                await _dbContext.SaveChangesAsync();
            }

            return RedirectToAction("GetAll", "users");
        }


        [HttpPost]
        public async Task<IActionResult> Block(string userIds)
        {
            if (_thisUser is not null)
            {
            List<Guid>? selectedUsersList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Guid>>(userIds);

            foreach (Guid userGuid in selectedUsersList)
            {
                var user = _dbContext.UsersDb.FirstOrDefault(x => x.Id.Equals(userGuid));
                if (user is not null)
                {
                    user.IsBlocked = Status.Blocked;
                }
            }

            if (selectedUsersList.Contains(_thisUser.Id))
            {
                _thisUser.IsBlocked = Status.Blocked;
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }


            await _dbContext.SaveChangesAsync();
            return RedirectToAction("GetAll", "users");
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Unblock(string userIds)
        {
            if (_thisUser is not null)
            {
            List<Guid>? selectedUsersList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Guid>>(userIds);

            foreach (Guid userGuid in selectedUsersList)
            {
                var user = _dbContext.UsersDb.FirstOrDefault(x => x.Id.Equals(userGuid));
                if (user is not null)
                {
                    user.IsBlocked = Status.Active;
                }
            }

            await _dbContext.SaveChangesAsync();
            return RedirectToAction("GetAll", "users");
            } else
            {
                return RedirectToAction("Index","Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userIds)
        {

            if (_thisUser is not null)
            {
                List<Guid>? selectedUsersList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Guid>>(userIds);

                foreach (Guid userGuid in selectedUsersList)
                {
                    _dbContext.UsersDb.Remove(_dbContext.UsersDb.FirstOrDefault(x => x.Id.Equals(userGuid)));
                }

                if (selectedUsersList.Contains(_thisUser.Id))
                {
                    await _dbContext.SaveChangesAsync();
                    _thisUser = null;
                    return RedirectToAction("Index", "Home");
                } else
                {
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("GetAll", "users");
                }
            } else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpGet]
        public IActionResult Login()
        {
            var response = new LoginViewModel();
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            var user = await _dbContext.UsersDb.FirstOrDefaultAsync(x => x.Email.Equals(viewModel.Email));

            if (user != null)
            {
                //user is founf, check password
                if (user.IsBlocked == Status.Active && user.Password == viewModel.Password)
                {
                    _thisUser = user;
                    user.LastLoginTime = DateTime.Now;
                    await _dbContext.SaveChangesAsync();
                    return RedirectToAction("GetAll", "Users");
                }
                else if (user.IsBlocked == Status.Blocked)
                {
                    ModelState.AddModelError(string.Empty, "Sorry, this account is blocked!");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Wrong email and/or password");
                }
            }
            // User not found
            ModelState.AddModelError(string.Empty, "Wrong email and/or password");
            return View(viewModel);
        }


    }
}
