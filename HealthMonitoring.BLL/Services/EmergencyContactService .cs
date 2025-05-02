using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.UnitOfWork;

namespace HealthMonitoring.BLL.Services
{
    public class EmergencyContactService :IEmergencyContactService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmergencyContactService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EmergencyContactReadDto>> GetAllAsync()
        {
            var contacts = await _unitOfWork.EmergencyContacts.GetAllAsync();
            return _mapper.Map<IEnumerable<EmergencyContactReadDto>>(contacts);
        }

        public async Task<EmergencyContactReadDto> GetByIdAsync(int id)
        {
            var contact = await _unitOfWork.EmergencyContacts.GetByIdAsync(id);
            return _mapper.Map<EmergencyContactReadDto>(contact);
        }

        public async Task AddAsync(EmergencyContactCreateDto dto)
        {
            var contact = _mapper.Map<EmergencyContact>(dto);
            await _unitOfWork.EmergencyContacts.CreateAsync(contact);
             _unitOfWork.SaveChanges();
        }

        public async Task UpdateAsync(EmergencyContactUpdateDto dto)
        {
            var contact = await _unitOfWork.EmergencyContacts.GetByIdAsync(dto.ContactId);
            if (contact != null)
            {
                _mapper.Map(dto, contact);
               await _unitOfWork.EmergencyContacts.UpdateAsync(contact);
                _unitOfWork.SaveChanges();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await _unitOfWork.EmergencyContacts.GetByIdAsync(id);
            if (contact != null)
            {
               await _unitOfWork.EmergencyContacts.RemoveAsync(contact);
                _unitOfWork.SaveChanges();
            }
        }
    }
}
