using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;

namespace HealthMonitoring.BLL.IServices
{
    public interface IActivityDataServices
    {
       Task< IEnumerable<ActivityDataReadDto>> GetAll();
        Task<ActivityDataReadDto> GetById(int id);
        Task  Add(ActivityDataCreateDto activitycreatdto);
        Task Update(ActivityDataUpdateDto activityupdatedto);
        Task Delete(int Id);
       
    }
}
