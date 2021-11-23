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

        public VotingService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }

        public async Task AddAsync(VotingModel model)
        {
            throw new NotImplementedException();
        }

        public async Task AddVoteAsync(VoteModel model)
        {
            if (await GetByIdAsync(model.VotingId) is null)
                throw new ArgumentNullException(nameof(model.VotingId), "Voting with such an id was not found");
            if (await _context.Users.FindAsync(model.UserId) is null)
                throw new ArgumentNullException(nameof(model.UserId), "User with such an id was not found");
            await _context.Votes.AddAsync(_mapper.Map<Vote>(model));
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
            return _mapper.Map<IEnumerable<VotingModel>>(_context.Votings);
        }

        public async Task<VotingModel> GetByIdAsync(int id)
        {
            return _mapper.Map<VotingModel>(await _context.Votings.FindAsync(id));
        }

        public Task UpdateAsync(VotingModel model)
        {
            throw new NotImplementedException();
        }
    }
}
