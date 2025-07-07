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
        Task<AiModelResponseDto> PredictCaloriesAsync(AiModelRequestDto request);

    }
}
