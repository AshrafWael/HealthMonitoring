using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.IServices;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.IRepository;
using HealthMonitoring.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace HealthMonitoring.BLL.Services
{
    public class ActivityDataServices : IActivityDataServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityData> _logger;

        public ActivityDataServices(IUnitOfWork unitOfWork, IMapper mapper,ILogger<ActivityData> logger )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task Add(ActivityDataCreateDto activitycreatdto)
        {

            try
            {
                var activity = _mapper.Map<ActivityData>(activitycreatdto);
                activity.RecordedAt = DateTime.UtcNow;

               await _unitOfWork.ActivityDatas.CreateAsync(activity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Added new activity for user {UserId}", activity.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding new activity");
                throw new ApplicationException("Error adding new activity", ex);
            }
           
        }
        public async Task <IEnumerable<ActivityDataReadDto>> GetAll()
        {
            try
            {
                var activities =  await _unitOfWork.ActivityDatas.GetAllAsync();
                var mappedactivity = _mapper.Map<IEnumerable<ActivityDataReadDto>>(activities);
                return mappedactivity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all activities");
                throw new ApplicationException("Error retrieving activities", ex);
            }
        }

        public async Task<List<ActivityDataReadDto>> GetByUserId(string userid)
        {
            if (userid !=  null)
            {
                var activity = await _unitOfWork.ActivityDatas.FindAllAsync(a => a.UserId == userid,null,null);
                if (activity == null)
                {
                    throw new KeyNotFoundException($"No activity found for user {userid}");
                }
                var mappedactivity = _mapper.Map<List<ActivityDataReadDto>>(activity);
                return mappedactivity;
            }
            else
            {
                throw new ArgumentNullException(nameof(userid), "User ID cannot be null");
            }
        }

        public async Task Delete(int id)
        {
            var activity = await _unitOfWork.ActivityDatas.FindAsync(a => a.Id == id);
            if (activity == null)
            {
                throw new KeyNotFoundException($"No activity found for user {id}");
            }else
            {
              await  _unitOfWork.ActivityDatas.RemoveAsync(activity);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Deleted activity for user {UserId}", id);
            }
        }

        public async Task<ActivityDataUpdateDto> Update(ActivityDataUpdateDto activityupdatedto,int id)
        {
            var activity = await _unitOfWork.ActivityDatas.FindAsync(a => a.Id == id);
            if (activity == null)
            {
                throw new KeyNotFoundException($"No activity found with ID {id}");
            }
            else
            {
                var mappedactvity =_mapper.Map<ActivityDataUpdateDto,ActivityData >(activityupdatedto, activity);
               await _unitOfWork.ActivityDatas.UpdateAsync(activity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated activity for user {UserId}", activityupdatedto.UserId);
            }
            return activityupdatedto;
        }
    }
}
