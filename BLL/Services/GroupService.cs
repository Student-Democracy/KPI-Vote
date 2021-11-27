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
    public class GroupService : IGroupService
    {
        private readonly ApplicationContext _context;

        private readonly IMapper _mapper;

        public GroupService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }

        public async Task AddAsync(GroupModel model)
        {
            if (model.Number < 0 && model.Number >= 10)
                throw new ArgumentNullException(nameof(model), "Number of group < 0 or >=10");
            model.CreationDate = DateTime.Now;
            if(await _context.Flows.FindAsync(model.FlowId) is null)
                throw new ArgumentNullException(nameof(model), "Flow with such an id was not found");

            await _context.Groups.AddAsync(_mapper.Map<Group>(model));
            await _context.SaveChangesAsync();
        }
        public async Task DeleteByIdAsync(int id)
        {
            var model = await _context.Groups.FindAsync(id);
            if (model is null)
                throw new ArgumentNullException(nameof(id), "Group with such an id was not found");
            _context.Groups.Remove(model);
            await _context.SaveChangesAsync();
        }
        public IEnumerable<GroupModel> GetAll()
        {
            return _mapper.Map<IEnumerable<GroupModel>>(_context.Groups);
        }

        public async Task<GroupModel> GetByIdAsync(int id)
        {
            return _mapper.Map<GroupModel>(await _context.Groups.SingleOrDefaultAsync(v => v.Id == id));
        }

        public async Task UpdateAsync(GroupModel model)
        {
            if (model.Number < 0 && model.Number >= 10)
                throw new ArgumentNullException(nameof(model), "Number of group < 0 or >=10");
            if (await _context.Flows.FindAsync(model.FlowId) is null)
                throw new ArgumentNullException(nameof(model), "Flow with such an id was not found");
            _context.Groups.Update(_mapper.Map<Group>(model));
            await _context.SaveChangesAsync();
        }
    }
}
