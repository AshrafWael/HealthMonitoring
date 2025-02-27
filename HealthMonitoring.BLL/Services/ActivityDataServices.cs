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
                _unitOfWork.SaveChanges();

                _logger.LogInformation("Added new activity for user {UserId}", activity.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding new activity");
                throw new ApplicationException("Error adding new activity", ex);
            }
        }

        public Task Delete(int Id)
        {
            throw new NotImplementedException();
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

        public async Task<ActivityDataReadDto> GetById(int id)
        {
            throw new NotImplementedException();
        }

 

        public Task Update(ActivityDataUpdateDto activityupdatedto)
        {
            throw new NotImplementedException();
        }
    }
}
