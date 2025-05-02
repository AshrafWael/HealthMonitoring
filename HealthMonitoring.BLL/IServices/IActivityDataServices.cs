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
        Task Add(ActivityDataCreateDto activitycreatdto);
        Task<ActivityDataUpdateDto> Update(ActivityDataUpdateDto activityupdatedto,int id);
        Task<List<ActivityDataReadDto>>  GetByUserId(string userid);
        Task Delete(int userid);
       
    }
}
