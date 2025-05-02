using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;

namespace HealthMonitoring.BLL.IServices
{
    public interface IEmergencyContactService
    {
        Task<IEnumerable<EmergencyContactReadDto>> GetAllAsync();
        Task<EmergencyContactReadDto> GetByIdAsync(int id);
        Task AddAsync(EmergencyContactCreateDto dto);
        Task UpdateAsync(EmergencyContactUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
