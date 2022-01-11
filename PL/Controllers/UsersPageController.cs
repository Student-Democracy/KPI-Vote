using BLL.Interfaces;
using BLL.Models;
using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using AutoMapper;

namespace PL.Controllers
{
    [Authorize]
    public class UsersPageController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly IGroupService _groupService;

        private readonly IFlowService _flowService;

        private readonly IFacultyService _facultyService;

        private readonly ApplicationContext _context;

        private readonly IMapper _mapper;

        private IBlockService _blockService;

        public UsersPageController(UserManager<User> userManager, IBlockService blockService, IGroupService groupService, IFlowService flowService, IFacultyService facultyService, ApplicationContext context, IMapper mapper)
        {
            _userManager = userManager;
            _blockService = blockService;
            _groupService = groupService;
            _flowService = flowService;
            _facultyService = facultyService;
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }


        [HttpGet]
        public IActionResult Index()
        {
            var modelUsersPage = new UsersPageViewModel();
            var faculties = _facultyService.GetAll();
            var flows = _flowService.GetAll();
            var groups = _groupService.GetAll();

            var users = _mapper.Map<IEnumerable<User>>(_context.Users);

            var bans = _blockService.GetSortedActiveBlocksAsync().Result.ToList();
            List<string> activeBans = new List<string>();
            for(int i = 0; i<bans.Count; i++)
            {
                activeBans.Add(bans[i].UserId);
            }

            modelUsersPage.Faculties = faculties;
            modelUsersPage.Flows = flows;
            modelUsersPage.Groups = groups;
            modelUsersPage.Users = users;
            modelUsersPage.ActiveBlocks = activeBans;

            return View(modelUsersPage);
        }

        [HttpGet]
        [Route("UsersPage/Block")]
        public async Task<IActionResult> Block(string id)
        {            
            var block = new BlockViewModel();
            var admin = await _userManager.GetUserAsync(User);
            block.UserId = id;
            block.User = _context.Users.SingleOrDefault(u => u.Id == id);
            block.AdminId = admin.Id;
            block.Admin = admin;
            return View(block);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UsersPage/Block")]
        public async Task<IActionResult> Block(BlockViewModel model, string id)
        {
            var block = new BlockModel();
            block.Hammer = model.Hammer;
            block.DateTo = model.DateTo;
            var admin = await _userManager.GetUserAsync(User);
            block.AdminId = admin.Id;
            block.UserId = id;
            await _blockService.AddAsync(block);
            return RedirectToAction("Index", "UsersPage");
        }

        [HttpGet]
        [Route("UsersPage/Unlock")]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var ban = await _blockService.GetByUserIdAsync(user.Id);
            if (ban is null)
                return RedirectToAction("Index", "UsersPage");
            var unlock = new UnlockViewModel()
            {
                Id = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email
            };
            var admin = await _userManager.FindByIdAsync(ban.AdminId);
            var adminName = admin.LastName + " " + admin.FirstName;
            if (!(admin.Patronymic is null))
                adminName += " " + admin.Patronymic;
            unlock.Ban = new BanReducedViewModel()
            {
                DateTo = ban.DateTo,
                Hammer = ban.Hammer,
                AdminEmail = admin.Email,
                AdminTelegramTag = admin.TelegramTag,
                AdminName = adminName
            };
            return View(unlock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UsersPage/UnlockConfirmed")]
        public async Task<IActionResult> UnlockConfirmed(string id)
        {
                await _blockService.DeleteByUserIdAsync(id);
                TempData["Message"] = "Користувача було розблоковано успішно";
            return RedirectToAction("Index", "UsersPage");
        }

        [HttpGet]
        [Route("UsersPage/CreateFaculty")]
        public IActionResult CreateFaculty()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UsersPage/CreateFaculty")]
        public async Task<IActionResult> CreateFaculty(FacultyViewModel model)
        {
            var faculty= new FacultyModel();
            faculty.Name = model.Name;

            await _facultyService.AddAsync(faculty);
            return RedirectToAction("Index", "UsersPage");
        }

        [HttpGet]
        [Route("UsersPage/CreateFlow")]
        public async Task<IActionResult> CreateFlow()
        {
            var facultiesGotten = _facultyService.GetAll().ToList();
            List<string> _facultiesName = new List<string>();
            var flow = new FlowViewModel();
            var groups = _groupService.GetAll();
            var author = await _userManager.GetUserAsync(User);
            var flows = _flowService.GetAll();
            flow.Flows = flows;
            flow.Author = author;
            flow.Groups = groups;
            for(int i = 0; i<facultiesGotten.Count; i++)
            {
                _facultiesName.Add(facultiesGotten[i].Name);
            }
            flow.Faculties = _facultiesName;
            flow.FacultiesIds = facultiesGotten;
            return View(flow);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UsersPage/CreateFlow")]
        public async Task<IActionResult> CreateFlow(FlowViewModel model)
        {
            var flow = new FlowModel();
            flow.Name = model.Name;
            if (model.Postfix != null)
                flow.Postfix = model.Postfix;
            else
                flow.Postfix = "";
            var facultiesGotten = _facultyService.GetAll().ToList();
            foreach (var faculty in facultiesGotten)
            {
                if (model.FacultyNameChoose == faculty.Name)
                {
                    flow.FacultyId = faculty.Id;
                    break;
                }
            }
            await _flowService.AddAsync(flow);            
            return RedirectToAction("Index", "UsersPage");
        }

        [HttpGet]
        [Route("UsersPage/CreateGroup")]
        public async Task<IActionResult> CreateGroup()
        {
            var flows = _flowService.GetAll();
            var group = new GroupViewModel();
            var author = await _userManager.GetUserAsync(User);
            var groups = _groupService.GetAll();
            group.Author = author;
            group.Flows = flows;
            group.Groups = groups;
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UsersPage/CreateGroup")]
        public async Task<IActionResult> CreateGroup(GroupViewModel model)
        {
            string[] full_flow_name = model.FlowName.Split('/');
            var flows = _flowService.GetAll();
            int flowId = 0;            
            FlowModel _flow = new FlowModel();
            foreach(var flow in flows)
            {
                if(full_flow_name[0] == flow.Name)
                {
                    if(full_flow_name.Length == 1 && (flow.Postfix == null || flow.Postfix == ""))
                    {
                        flowId = flow.Id;
                        _flow = flow;
                        break;
                    }
                    else if(full_flow_name[1] == flow.Postfix)
                    {
                        flowId = flow.Id;
                        _flow = flow;
                        break;
                    }
                }
            }

            var groups = _groupService.GetAll();
            int flowGroupsCount = 0;
            foreach(var groupLocal in groups)
            {
                if (groupLocal.FlowId == _flow.Id)
                    flowGroupsCount += 1;
            }

            var group = new GroupModel();
            group.FlowId = flowId;
            group.Number = Convert.ToInt16(flowGroupsCount + 1);
            

            await _groupService.AddAsync(group);
            var groupIdNew = _groupService.GetAll().ToList().Last().Id;
            _flow.GroupIds.Add(groupIdNew);
            await _flowService.UpdateAsync(_flow);

            var newFlow = await _flowService.GetByIdAsync(7);
            

            return RedirectToAction("Index", "UsersPage");
        }

        [HttpGet]
        [Route("UsersPage/AddUser")]
        public async Task<IActionResult> AddUser()
        {
            var groups = _groupService.GetAll();
            var flows = _flowService.GetAll();
            var userRegistration = new UserRegistrationViewModel();
            var author = await _userManager.GetUserAsync(User);
            userRegistration.Author = author;
            userRegistration.Groups = groups;
            userRegistration.Flows = flows;
            return View(userRegistration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UsersPage/AddUser")]
        public async Task<IActionResult> AddUser(UserRegistrationViewModel model)
        {
            var user = new User();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Patronymic = model.Patronymic;
            user.RegistrationDate = DateTime.Now;
            user.Email = model.Email;
            user.TelegramTag = model.TelegramTag;
            user.PasswordChanged = false;

            string[] full_name_group = model.GroupName.Split(' ');
            var flows = _flowService.GetAll();
            var groups = _groupService.GetAll();
            int flowId = 0;
            int groupId = 0;
            foreach(var flow in flows)
            {
                if(flow.Name == full_name_group[0].Substring(0, 4))
                {
                    if ((full_name_group.Length == 1 && (flow.Postfix == null || flow.Postfix == "")) 
                        || full_name_group[1] == "" && (flow.Postfix == null || flow.Postfix == ""))
                    {
                        flowId = flow.Id;
                        break;
                    }
                    else if (full_name_group[1] == flow.Postfix)
                    {
                        flowId = flow.Id;
                        break;
                    }
                }
            }

            foreach(var group in groups)
            {
                if(group.FlowId == flowId && group.Number == Convert.ToInt16(full_name_group[0].Substring(4,1)))
                {
                    groupId = group.Id;
                    break;
                }
            }

            user.GroupId = groupId;

            User newUser = await _userManager.FindByEmailAsync(user.Email);
            var author = await _userManager.GetUserAsync(User);
            model.Author = author;
            model.Groups = groups;
            model.Flows = flows;
            if (_userManager.Users.AsEnumerable().Where(u => u.Id != user.Id).Select(u => u.TelegramTag.ToUpperInvariant()).Contains(model.TelegramTag.ToUpperInvariant()))
            {
                ModelState.AddModelError("TgTagIsTakenError", "Такий тег телеграм вже зайнято");
                return View(model);
            }
                if (newUser is null)
            {
                user.UserName = user.Email;
                user.PasswordHash = null;
                var result = await _userManager.CreateAsync(user, "P@$$w0rd");
                    newUser = await _userManager.FindByEmailAsync(user.Email);
                    if (model.RoleChoose != "Студент")
                        await _userManager.AddToRoleAsync(newUser, "Студент");
                    await _userManager.AddToRoleAsync(newUser, model.RoleChoose);
                return RedirectToAction("Index", "UsersPage");
            }
            else
            {
                ModelState.AddModelError("EmailIsTakenError", "Така електронна пошта вже зайнята");
                return View(model);
            }
        }

        [HttpGet]
        [Route("UsersPage/UserProfileReader")]
        public async Task<IActionResult> UserProfileReader(string id)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == id);
            var userProfile = new UserProfileViewModel();
            userProfile.Name = user.LastName + " " + user.FirstName + " " + user.Patronymic;
            userProfile.TelegramTag = user.TelegramTag;
            userProfile.Email = user.Email;
            string groupName = "";
            string facultyName = "";
            var groups = _groupService.GetAll();
            var flows = _flowService.GetAll();
            var faculties = _facultyService.GetAll().ToList();
            foreach (var group in groups)
            {
                if(group.Id == user.GroupId)
                {
                    foreach(var flow in flows)
                    {
                        if(flow.Id == group.FlowId)
                        {
                            groupName = flow.Name + group.Number.ToString() + " " + flow.Postfix;
                            break;
                        }
                    }
                }
            }
            userProfile.Group = groupName;
            foreach (var group in groups)
            {
                if (group.Id == user.GroupId)
                {
                    foreach (var flow in flows)
                    {
                        if (flow.Id == group.FlowId)
                        {
                            foreach(var faculty in faculties)
                            {
                                if(flow.FacultyId == faculty.Id)
                                {
                                    facultyName = faculty.Name;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            userProfile.Faculty = facultyName;
            userProfile.Roles = await _userManager.GetRolesAsync(user);

            return View(userProfile);
        }
    }
}
