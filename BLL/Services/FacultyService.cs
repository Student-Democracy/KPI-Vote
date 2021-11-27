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
    public class FacultyService : IFacultyService
    {
        private readonly ApplicationContext _context;

        private readonly IMapper _mapper;

        public IEnumerable<FacultyModel> GetAll()
        {
            return _mapper.Map<IEnumerable<FacultyModel>>(_context.Faculties);
        }

        public async Task<FacultyModel> GetByIdAsync(int id)
        {
            return _mapper.Map<FacultyModel>(await _context.Faculties.SingleOrDefaultAsync(v => v.Id == id));
        }

        public FacultyService(ApplicationContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context), "Context cannot be null");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper), "Mapper cannot be null");
        }

        public async Task AddAsync(FacultyModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");

            await _context.Faculties.AddAsync(_mapper.Map<Faculty>(model));
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FacultyModel model)
        {
            if (string.IsNullOrEmpty(model.Name))
                throw new ArgumentNullException(nameof(model), "Model's name cannot be null or empty");

            _context.Faculties.Update(_mapper.Map<Faculty>(model));
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var model = await _context.Faculties.FindAsync(id);
            if (model is null)
                throw new ArgumentNullException(nameof(id), "Flow with such an id was not found");
            _context.Faculties.Remove(model);
            await _context.SaveChangesAsync();
        }
    }
}
