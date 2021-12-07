using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class VotingService : IVotingService
    {

        private readonly ApplicationContext _context;

        private readonly IMapper _mapper;

        private const short _minDescriptionLength = 400;

        private const short _maxNameLength = 250;

        private const short _maxVisibilityTerm = 31;

        private const decimal _minForPercentage = 50.0m;

        public VotingService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }

        public async Task AddAsync(VotingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");
            if (model.Name.Length > _maxNameLength)
                throw new ArgumentException($"Model's name cannot contain more than {_maxNameLength} characters",
                    nameof(model));
            if (string.IsNullOrEmpty(model.Description))
                throw new ArgumentNullException(nameof(model), "Model's description cannot be null or empty");
            if (model.Description.Length < _minDescriptionLength)
                throw new ArgumentException($"Model's description should contain at least {_minDescriptionLength} characters",
                    nameof(model));
            if (model.MinimalForPercentage < _minForPercentage || model.MinimalForPercentage > 100)
                throw new ArgumentException($"Model's minimal for percentage cannot be less than {_minForPercentage} or bigger than 100", 
                    nameof(model));
            if (model.MinimalAttendancePercentage <= 0 || model.MinimalAttendancePercentage > 100)
                throw new ArgumentException($"Model's minimal attendance percentage cannot be less than or equal 0 or greater than 100",
                    nameof(model));
            model.CreationDate = DateTime.Now;
            if (model.CompletionDate < model.CreationDate)
                throw new ArgumentException($"Model's completion date cannot be less than creation date",
                    nameof(model));
            if (model.VisibilityTerm <= 0 || model.VisibilityTerm > _maxVisibilityTerm)
                throw new ArgumentException($"Model's visibility term is invalid",
                    nameof(model));
            if (string.IsNullOrEmpty(model.AuthorId))
                throw new ArgumentNullException(nameof(model), "Model's author id cannot be null or empty");
            if (await _context.Users.FindAsync(model.AuthorId) is null)
                throw new InvalidOperationException("Author with such an id was not found");
            await _context.Votings.AddAsync(_mapper.Map<Voting>(model));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var model = await _context.Votings.FindAsync(id);
            if (model is null)
                throw new InvalidOperationException("Voting with such an id was not found");
            _context.Votings.Remove(model);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<VotingModel> GetAll()
        {
            return _mapper.Map<IEnumerable<VotingModel>>(_context.Votings.Include(v => v.Votes));
        }

        public async Task<VotingModel> GetByIdAsync(int id)
        {
            return _mapper.Map<VotingModel>(await _context.Votings.Include(v => v.Votes).SingleOrDefaultAsync(v => v.Id == id));
        }

        public async Task UpdateAsync(VotingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");
            if (model.Name.Length > _maxNameLength)
                throw new ArgumentException($"Model's name cannot contain more than {_maxNameLength} characters",
                    nameof(model));
            if (string.IsNullOrEmpty(model.Description))
                throw new ArgumentNullException(nameof(model), "Model's description cannot be null or empty");
            if (model.Description.Length < _minDescriptionLength)
                throw new ArgumentException($"Model's description should contain at least {_minDescriptionLength} characters",
                    nameof(model));
            if (model.MinimalForPercentage < _minForPercentage || model.MinimalForPercentage > 100)
                throw new ArgumentException($"Model's minimal for percentage cannot be less than or equal {_minForPercentage} or greater than 100",
                    nameof(model));
            if (model.MinimalAttendancePercentage <= 0 || model.MinimalAttendancePercentage > 100)
                throw new ArgumentException($"Model's minimal attendance percentage cannot be less than or equal 0 or greater than 100",
                    nameof(model));
            var existingModel = await _context.Votings.FindAsync(model.Id);
            if (!(existingModel is null) && model.CreationDate != existingModel.CreationDate)
                throw new ArgumentException("The creation date cannot be changed", nameof(model));
            if (!(existingModel is null) && model.AuthorId != existingModel.AuthorId)
                throw new ArgumentException("Author cannot be changed", nameof(model));
            if (model.CompletionDate < model.CreationDate)
                throw new ArgumentException($"Model's completion date cannot be less than creation date",
                    nameof(model));
            if (model.VisibilityTerm <= 0 || model.VisibilityTerm > _maxVisibilityTerm)
                throw new ArgumentException($"Model's visibility term is invalid",
                    nameof(model));
            if (string.IsNullOrEmpty(model.AuthorId))
                throw new ArgumentNullException(nameof(model), "Model's author id cannot be null or empty");
            if (await _context.Users.FindAsync(model.AuthorId) is null)
                throw new InvalidOperationException("Author with such an id was not found");
            existingModel = _mapper.Map(model, existingModel);
            _context.Votings.Update(existingModel);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<VotingModel>> GetFilteredAndSortedForUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User Id cannot be null");
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                throw new InvalidOperationException("No such a user");
            var activeVotings = await Task.Run(() => _context.Votings
                .Where(v => v.CompletionDate.AddDays(v.VisibilityTerm) >= DateTime.Now && v.Status == VotingStatus.Confirmed)
                .Include(v => v.Votes));
            var group = await _context.Groups.FindAsync(user.GroupId);
            Flow flow = null;
            Faculty faculty = null;
            if (!(group is null))
                flow = await _context.Flows.FindAsync(group.FlowId);
            if (!(flow is null))
                faculty = await _context.Faculties.FindAsync(flow.FacultyId);
            var userVotings = await Task.Run(() => activeVotings
            .Where(v => v.GroupId == null && v.FlowId == null && v.FacultyId == null)
            .OrderByDescending(v => v.CreationDate)
            .AsEnumerable());         // KPI level
            if (!(faculty is null))
                userVotings = await Task.Run(() => activeVotings
                .Where(v => v.GroupId == null && v.FlowId == null && v.FacultyId == faculty.Id)
                .OrderByDescending(v => v.CreationDate)
                .Concat(userVotings));                        // Faculty level
            if (!(flow is null))
                userVotings = await Task.Run(() => activeVotings
                .Where(v => v.GroupId == null && v.FlowId == flow.Id)
                .OrderByDescending(v => v.CreationDate)
                .Concat(userVotings));                        // Flow level
            if (!(group is null))
                userVotings = await Task.Run(() => activeVotings
                .Where(v => v.GroupId == group.Id)
                .OrderByDescending(v => v.CreationDate)
                .Concat(userVotings));                        // Group level
            return _mapper.Map<IEnumerable<VotingModel>>(userVotings);
        }

        public async Task<IEnumerable<VotingModel>> GetFilteredAndSortedForAdminAsync()
        {
            var votings = await Task.Run(() => _context.Votings.
                Where(v => v.CompletionDate.AddDays(v.VisibilityTerm) >= DateTime.Now && v.Status != VotingStatus.Denied).
                Include(v => v.Votes).
                OrderByDescending(v => v.CreationDate).
                AsEnumerable());                                    // actual votings
            var oldOrBannedVotings = await Task.Run(() => _context.Votings.
                Where(v => v.CompletionDate.AddDays(v.VisibilityTerm) < DateTime.Now || v.Status == VotingStatus.Denied).
                Include(v => v.Votes).
                OrderByDescending(v => v.CreationDate));            // archived votings
            votings = await Task.Run(() => votings.Concat(oldOrBannedVotings));
            return _mapper.Map<IEnumerable<VotingModel>>(votings);
        }

        public async Task<IEnumerable<VotingModel>> GetUserVotingsAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId), "User Id cannot be null");
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                throw new ArgumentNullException(nameof(userId), "No such a user");
            var votings = await Task.Run(() => _context.Votings
                .Include(v => v.Votes)
                .Where(v => v.Votes
                    .Where(vote => vote.UserId == userId)
                    .Any()
                    )
                .OrderByDescending(v => v.CreationDate)
                .AsEnumerable());
            return _mapper.Map<IEnumerable<VotingModel>>(votings);
        }

        public async Task ChangeStatusAsync(VotingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (model.StatusSetterId is null)
                throw new ArgumentNullException(nameof(model), "Status setter's id cannot be null");
            if (await _context.Users.FindAsync(model.StatusSetterId) is null)
                throw new InvalidOperationException("Status setter's was not found");
            await UpdateAsync(model);
        }

        public async Task<bool> IsVotingSuccessfulAsync(VotingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (model.Status != VotingStatus.Confirmed)
                throw new ArgumentException("Voting should be confirmed", nameof(model));
            if (model.CompletionDate >= DateTime.Now)
                throw new ArgumentException("Voting should be completed", nameof(model));
            return await IsVotingSuccessfulNowAsync(model);
        }

        public async Task<bool> IsVotingSuccessfulNowAsync(VotingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            var success = await GetActualAttendancePercentageAsync(model) >= model.MinimalAttendancePercentage / 100.0m
                && await GetActualForPercentageAsync(model) >= model.MinimalForPercentage / 100.0m;
            return success;
        }

        public async Task<decimal> GetActualAttendancePercentageAsync(VotingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (model.Status != VotingStatus.Confirmed)
                throw new ArgumentException("Voting should be confirmed", nameof(model));
            IEnumerable<User> total;
            var votes = await Task.Run(() => _context.Votes.Where(v => v.VotingId == model.Id));
            var notBannedUsers = await Task.Run(() => _context.Users
                .Include(u => u.Bans)
                    .Where(u => !u.Bans
                    .Any(b => b.DateTo >= model.CompletionDate && b.DateFrom <= model.CreationDate)));
            if (!(model.GroupId is null))
            {
                total = await Task.Run(() => notBannedUsers
                .Where(u => u.GroupId == model.GroupId));
            }
            else if (!(model.FlowId is null))
            {
                total = await Task.Run(() => notBannedUsers
                .Include(u => u.Group)
                .Where(u => u.Group.FlowId == model.FlowId));
            }
            else if (!(model.FacultyId is null))
            {
                total = await Task.Run(() => notBannedUsers
                .Include(u => u.Group)
                .ThenInclude(g => g.Flow)
                .Where(u => u.Group.Flow.FacultyId == model.FacultyId));
            }
            else
            {
                total = await Task.Run(() => notBannedUsers);
            }
            var totalNumber = await Task.Run(() => total.Count());
            var votesNumber = await Task.Run(() => votes.Count());
            var totalIdsOnly = await Task.Run(() => total.Select(u => u.Id));
            foreach(var vote in votes)
            {
                if (!totalIdsOnly.Contains(vote.UserId))
                    totalNumber++;
            }
            if (totalNumber != 0)
                return await Task.Run(() => votesNumber / (decimal)totalNumber);
            else
                return 0m;
        }

        public async Task<decimal> GetActualForPercentageAsync(VotingModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (model.Status != VotingStatus.Confirmed)
                throw new ArgumentException("Voting should be confirmed", nameof(model));
            var votes = await Task.Run(() => _context.Votes.Where(v => v.VotingId == model.Id));
            var votersNumber = await Task.Run(() => votes.Count());
            var votersForNumber = await Task.Run(() => votes.Where(v => v.Result == VoteResult.For).Count());
            if (votersNumber != 0)
                return await Task.Run(() => votersForNumber / (decimal)votersNumber);
            else
                return 0m;
        }

        public async Task<IEnumerable<VotingModel>> GetNotConfirmedAsync()
        {
            var votings = await Task.Run(() => _context.Votings
                .Where(v => v.Status == VotingStatus.NotConfirmed)
                .Include(v => v.Votes)
                .OrderBy(v => v.CreationDate));
            return _mapper.Map<IEnumerable<VotingModel>>(votings);
        }
    }
}
