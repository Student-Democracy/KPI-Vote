using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL;
using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class VoteService : IVoteService
    {
        private readonly ApplicationContext _context;

        private readonly IMapper _mapper;

        public VoteService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }
        public async Task AddAsync(VoteModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (await _context.Votings.FindAsync(model.VotingId) is null)
                throw new InvalidOperationException("Voting with such an id was not found");
            if (model.UserId is null)
                throw new ArgumentNullException(nameof(model), "User id cannot be null");
            if (await _context.Users.FindAsync(model.UserId) is null)
                throw new InvalidOperationException("User with such an id was not found");
            if (await _context.Votes.FindAsync(model.VotingId, model.UserId) != null)
                throw new ArgumentException("Such a vote already exists", nameof(model));
            await _context.Votes.AddAsync(_mapper.Map<Vote>(model));
            await _context.SaveChangesAsync();
        }

        public IEnumerable<VoteModel> GetAll()
        {
            return _mapper.Map<IEnumerable<VoteModel>>(_context.Votes);
        }

        public async Task<VoteModel> GetByIdAsync(string userId, int votingId)
        {
            var found = await _context.Votes.FindAsync(votingId, userId);
            return _mapper.Map<VoteModel>(found);
        }
    }
}
