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
        [Route("UsersPage/CreateFaculty")]
        public async Task<IActionResult> CreateFaculty()
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
            for(int i = 0; i<facultiesGotten.Count; i++)
            {
                _facultiesName.Add(facultiesGotten[i].Name);
            }
            flow.Faculties = _facultiesName;
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
            group.Flows = flows;
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
            user.PasswordHash = "sample1";
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
            if (newUser is null)
            {
                user.UserName = user.Email;
                user.PasswordHash = null;
                await _userManager.CreateAsync(user, "P@$$w0rd");
                newUser = await _userManager.FindByEmailAsync(user.Email);
                await _userManager.AddToRoleAsync(newUser, model.RoleChoose);
            }
            return RedirectToAction("Index", "UsersPage");
        }


    }
}
