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
    public class BlockService : IBlockService
    {
        private readonly ApplicationContext _context;

        private readonly IMapper _mapper;

        private const short _minHammerLength = 10;
        private const short _maxHammerLength = 2000;

        public BlockService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }

        public async Task AddAsync(BlockModel model)
        {
            if(model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            model.DateFrom = DateTime.Now;            
            if (model.DateTo < model.DateFrom)
                throw new ArgumentException($"Ban date to the end cannot be less than date from the beginning",
                    nameof(model));
            if (string.IsNullOrEmpty(model.Hammer))
                throw new ArgumentNullException(nameof(model), "Ban hammer cannot be null or empty");
            if (model.Hammer.Length < _minHammerLength)
                throw new ArgumentException($"Ban hammer should contain at least {_minHammerLength} characters",
                    model.Hammer);
            if (model.Hammer.Length > _maxHammerLength)
                throw new ArgumentException($"Ban hammer should contain at not more {_maxHammerLength} characters",
                    model.Hammer);
            if (string.IsNullOrEmpty(model.UserId))
                throw new ArgumentNullException(nameof(model), "Model's user id cannot be null or empty");
            if (await _context.Users.FindAsync(model.UserId) is null)
                throw new ArgumentNullException(nameof(model), "User with such an id was not found");
            if (!(await GetByUserIdAsync(model.UserId) is null))
                throw new ArgumentException("This user is already blocked", nameof(model)); 
            if (string.IsNullOrEmpty(model.AdminId))
                throw new ArgumentNullException(nameof(model), "Model's admin id cannot be null or empty");
            if (await _context.Users.FindAsync(model.AdminId) is null)
                throw new ArgumentNullException(nameof(model), "Admin with such an id was not found");
            if (model.UserId == model.AdminId)
                throw new ArgumentException(nameof(model), "Admin cannot block himself/herself");
            await _context.Bans.AddAsync(_mapper.Map<Ban>(model));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var model = await _context.Bans.FindAsync(id);
            if (model is null)
                throw new InvalidOperationException("Ban (block) with such an id was not found");
            _context.Bans.Remove(model);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<BlockModel> GetAll()
        {
            return _mapper.Map<IEnumerable<BlockModel>>(_context.Bans);
        }

        public async Task<BlockModel> GetByIdAsync(int id)
        {
            return _mapper.Map<BlockModel>(await _context.Bans.SingleOrDefaultAsync(v => v.Id == id));
        }

        public async Task UpdateAsync(BlockModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model), "Model cannot be null");
            if (model.DateTo < model.DateFrom)
                throw new ArgumentException($"Ban date to the end cannot be less than date from the beginning",
                    nameof(model));
            if (string.IsNullOrEmpty(model.Hammer))
                throw new ArgumentNullException(nameof(model), "Ban hammer cannot be null or empty");
            if (model.Hammer.Length < _minHammerLength)
                throw new ArgumentException($"Ban hammer should contain at least {_minHammerLength} characters",
                    model.Hammer);
            if (model.Hammer.Length > _maxHammerLength)
                throw new ArgumentException($"Ban hammer should contain at not more {_maxHammerLength} characters",
                    model.Hammer);
            if (string.IsNullOrEmpty(model.UserId))
                throw new ArgumentNullException(nameof(model), "Model's user id cannot be null or empty");
            if (await _context.Users.FindAsync(model.UserId) is null)
                throw new ArgumentNullException(nameof(model), "User with such an id was not found");
            if (!(await _context.Bans.SingleOrDefaultAsync(p => p.UserId == model.UserId) is null))
                throw new ArgumentException("This user is already blocked", nameof(model));
            if (string.IsNullOrEmpty(model.AdminId))
                throw new ArgumentNullException(nameof(model), "Model's admin id cannot be null or empty");
            if (await _context.Users.FindAsync(model.AdminId) is null)
                throw new ArgumentNullException(nameof(model), "Admin with such an id was not found");
            if (model.UserId == model.AdminId)
                throw new ArgumentException(nameof(model), "Admin cannot block himself/herself");
            _context.Bans.Update(_mapper.Map<Ban>(model));
            await _context.SaveChangesAsync();
        }

        public async Task<BlockModel> GetByUserIdAsync(string userId)
        {
            if (userId is null || userId == "")
                throw new ArgumentNullException("Model's user id cannot be null or empty");
            if (await _context.Users.FindAsync(userId) is null)
                throw new ArgumentNullException("User with such an id was not found");
            return _mapper.Map<BlockModel>(await _context.Bans.SingleOrDefaultAsync(p => p.UserId == userId && p.DateTo >= DateTime.Now));
        }

        public async Task DeleteByUserIdAsync(string userId)
        {
            if (userId is null || userId == "")
                throw new ArgumentNullException("Model's user id cannot be null or empty");
            if (await _context.Users.FindAsync(userId) is null)
                throw new ArgumentNullException("User with such an id was not found");
            var blocks = _context.Bans.Where(p => p.UserId == userId && p.DateTo >= DateTime.Now).ToArray();
            foreach (var block in blocks)
            {
                await DeleteByIdAsync(block.Id);
            }
        }

        public async Task<IEnumerable<BlockModel>> GetSortedByAdminIdAsync(string adminId)
        {
            if (adminId is null)
                throw new ArgumentNullException("Admin id cannot be null or empty");
            if (await _context.Users.FindAsync(adminId) is null)
                throw new ArgumentNullException("Admin with such an id was not found");

            var blocks = await Task.Run(() => _context.Bans.Where(v => v.AdminId == adminId)
                                                                     .OrderByDescending(v => v.DateFrom));
            return _mapper.Map<IEnumerable<BlockModel>>(blocks);
        }

        public async Task<IEnumerable<BlockModel>> GetSortedActiveBlocksAsync()
        {
            var blocks = await Task.Run(() => _context.Bans.Where(v => v.DateTo >= DateTime.Now)
                                                                     .OrderByDescending(v => v.DateFrom));
            return _mapper.Map<IEnumerable<BlockModel>>(blocks);
        }      

    }
}
