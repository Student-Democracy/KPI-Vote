﻿using AutoMapper;
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
    public class FlowService : IFlowService
    {
        private readonly ApplicationContext _context;

        private readonly IMapper _mapper;

        public IEnumerable<FlowModel> GetAll()
        {
            return _mapper.Map<IEnumerable<FlowModel>>(_context.Flows);
        }

        public async Task<FlowModel> GetByIdAsync(int id)
        {
            return _mapper.Map<FlowModel>(await _context.Flows.SingleOrDefaultAsync(v => v.Id == id));
        }

        public FlowService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }

        public async Task AddAsync(FlowModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");

            await _context.Flows.AddAsync(_mapper.Map<Flow>(model));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FlowModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");

            _context.Flows.Update(_mapper.Map<Flow>(model));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var model = await _context.Flows.FindAsync(id);
            if (model is null)
                throw new ArgumentNullException(nameof(id), "Flow with such an id was not found");
            _context.Flows.Remove(model);
            await _context.SaveChangesAsync();
        }
    }
}
