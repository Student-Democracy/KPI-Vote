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

        public VotingService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }

        public async Task AddAsync(VotingModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");
            if (string.IsNullOrEmpty(model.Description))
                throw new ArgumentNullException(nameof(model), "Model's description cannot be null or empty");
            if (model.Description.Length < _minDescriptionLength)
                throw new ArgumentException($"Model's description should contain at least {_minDescriptionLength} characters", 
                    model.Description);
            if (model.MinimalForPercentage <= 0)
                throw new ArgumentException($"Model's minimal for percentage cannot be less than or equal 0", 
                    nameof(model));
            model.CreationDate = DateTime.Now;
            if (model.CompletionDate < model.CreationDate)
                throw new ArgumentException($"Model's completion date cannot be less than creation date",
                    nameof(model));
            if (model.VisibilityTerm <= 0)
                throw new ArgumentException($"Model's visibility term cannot be less than or equal 0",
                    nameof(model));
            if (string.IsNullOrEmpty(model.AuthorId))
                throw new ArgumentNullException(nameof(model), "Model's author id cannot be null or empty");
            if (await _context.Users.FindAsync(model.AuthorId) is null)
                throw new ArgumentNullException(nameof(model), "Author with such an id was not found");
            await _context.Votings.AddAsync(_mapper.Map<Voting>(model));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var model = await _context.Votings.FindAsync(id);
            if (model is null)
                throw new ArgumentNullException(nameof(id), "Voting with such an id was not found");
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
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");
            if (string.IsNullOrEmpty(model.Description))
                throw new ArgumentNullException(nameof(model), "Model's description cannot be null or empty");
            if (model.Description.Length < _minDescriptionLength)
                throw new ArgumentException($"Model's description should contain at least {_minDescriptionLength} characters",
                    model.Description);
            if (model.MinimalForPercentage <= 0)
                throw new ArgumentException($"Model's minimal for percentage cannot be less than or equal 0",
                    nameof(model));
            if (model.CompletionDate < model.CreationDate)
                throw new ArgumentException($"Model's completion date cannot be less than creation date",
                    nameof(model));
            if (model.VisibilityTerm <= 0)
                throw new ArgumentException($"Model's visibility term cannot be less than or equal 0",
                    nameof(model));
            if (string.IsNullOrEmpty(model.AuthorId))
                throw new ArgumentNullException(nameof(model), "Model's author id cannot be null or empty");
            if (await _context.Users.FindAsync(model.AuthorId) is null)
                throw new ArgumentNullException(nameof(model), "Author with such an id was not found");
            _context.Votings.Update(_mapper.Map<Voting>(model));
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<VotingModel>> GetFilteredAndSortedForUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                throw new ArgumentNullException(nameof(userId), "No such a user");
            var activeVotings = _context.Votings.
                Where(v => v.CompletionDate.AddDays(v.VisibilityTerm) >= DateTime.Now && v.Status != VotingStatus.Denied);
            var group = await _context.Groups.FindAsync(user.GroupId);
            Flow flow = null;
            Faculty faculty = null;
            if (!(group is null))
                flow = await _context.Flows.FindAsync(group.FlowId);
            if (!(flow is null))
                faculty = await _context.Faculties.FindAsync(flow.FacultyId);
            var userVotings = activeVotings.
                Where(v => v.GroupId == null && v.FlowId == null && v.FacultyId == null).
                OrderByDescending(v => v.CreationDate).
                AsEnumerable();         // KPI level
            if (!(faculty is null))
                userVotings = activeVotings.Where(v => v.GroupId == null && v.FlowId == null && v.FacultyId == faculty.Id).
                    OrderByDescending(v => v.CreationDate).
                    Concat(userVotings);                        // Faculty level
            if (!(flow is null))
                userVotings = activeVotings.Where(v => v.GroupId == null && v.FlowId == flow.Id).
                    OrderByDescending(v => v.CreationDate).
                    Concat(userVotings);                        // Flow level
            if (!(group is null))
                userVotings = activeVotings.Where(v => v.GroupId == group.Id).
                    OrderByDescending(v => v.CreationDate).
                    Concat(userVotings);                        // Group level
            return _mapper.Map<IEnumerable<VotingModel>>(userVotings);
        }

        public IEnumerable<VotingModel> GetFilteredAndSortedForAdmin()
        {
            var votings = _context.Votings.
                Where(v => v.CompletionDate.AddDays(v.VisibilityTerm) >= DateTime.Now && v.Status != VotingStatus.Denied).
                OrderByDescending(v => v.CreationDate).
                AsEnumerable();
            var oldOrBannedVotings = _context.Votings.
                Where(v => v.CompletionDate.AddDays(v.VisibilityTerm) < DateTime.Now || v.Status == VotingStatus.Denied).
                OrderByDescending(v => v.CreationDate);
            votings = votings.Concat(oldOrBannedVotings);
            return _mapper.Map<IEnumerable<VotingModel>>(votings);
        }

        public async Task<IEnumerable<VotingModel>> GetUserVotingsAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                throw new ArgumentNullException(nameof(userId), "No such a user");
            var votings = _context.Votings
                .Include(v => v.Votes)
                .Where(v => v.Votes
                    .Where(vote => vote.UserId == userId)
                    .Any()
                    )
                .OrderByDescending(v => v.CreationDate)
                .AsEnumerable();
            return _mapper.Map<IEnumerable<VotingModel>>(votings);
        }
    }
}
