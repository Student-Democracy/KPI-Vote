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

        public IEnumerable<VotingModel> GetFilteredAndSortedForUser(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VotingModel> GetFilteredAndSortedForAdmin()
        {
            throw new NotImplementedException();
        }
    }
}
