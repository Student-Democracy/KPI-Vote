using AutoMapper;
using BLL.Interfaces;
using BLL.Models;
using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AppealService : IAppealService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public AppealService(
            ApplicationContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAsync(AppealModel model)
        {
            if (string.IsNullOrEmpty(model.Message))
                throw new ArgumentNullException(nameof(model), "Model's message cannot be null or empty");
            model.Date = DateTime.Now;
            if (model.Importance <= 0)
                throw new ArgumentException($"Model's importance term cannot be less than or equal 0",
                    nameof(model));
            if (string.IsNullOrEmpty(model.UserId))
                throw new ArgumentNullException(nameof(model), "Model's user id cannot be null or empty");
            if (await _context.Users.FindAsync(model.UserId) is null)
                throw new ArgumentNullException(nameof(model), "Author with such an id was not found");
            await _context.AddAsync(_mapper.Map<Appeal>(model));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var model = await _context.Appeals.FindAsync(id) ??
                throw new ArgumentNullException(nameof(id), "Appeal with such an id was not found");
            _context.Appeals.Remove(model);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<AppealModel> GetAll()
        {
            return _mapper.Map<IEnumerable<AppealModel>>(_context.Appeals);
        }

        public async Task<AppealModel> GetByIdAsync(int id)
        {
            return _mapper.Map<AppealModel>(await _context.Appeals.FindAsync(id));
        }

        public async Task UpdateAsync(AppealModel model)
        {
            if (string.IsNullOrEmpty(model.Message))
                throw new ArgumentNullException(nameof(model), "Model's message cannot be null or empty");
            if (model.Importance <= 0)
                throw new ArgumentException($"Model's importance term cannot be less than or equal 0",
                    nameof(model));
            if (string.IsNullOrEmpty(model.UserId))
                throw new ArgumentNullException(nameof(model), "Model's user id cannot be null or empty");
            if (await _context.Users.FindAsync(model.UserId) is null)
                throw new ArgumentNullException(nameof(model), "Author with such an id was not found");
            var existingModel = await _context.Appeals.FindAsync(model.Id);
            if (!(existingModel is null) && model.Date != existingModel.Date)
                throw new ArgumentException("The creation date cannot be changed", nameof(model));
            if (!(existingModel is null) && model.UserId != existingModel.UserId)
                throw new ArgumentException("Author cannot be changed", nameof(model));
            existingModel = _mapper.Map(model, existingModel);
            _context.Appeals.Update(existingModel);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AppealModel>> GetUserAppealsAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null)
                throw new ArgumentNullException(nameof(userId), "No such a user");
            var appeals = await _context.Appeals
                .Where(a => a.UserId == userId)
                .OrderByDescending(a=>a.Date)
                .ToListAsync();
            return _mapper.Map<IEnumerable<AppealModel>>(appeals);
        }

        public async Task ResponseAppealAsync(AppealModel model)
        {
            if (string.IsNullOrEmpty(model.Response))
                throw new ArgumentNullException(nameof(model), "Model's response cannot be null or empty");
            if (string.IsNullOrEmpty(model.AdminId))
                throw new ArgumentNullException(nameof(model), "Model's admin id cannot be null or empty");
            await UpdateAsync(model);
        }
    }
}
