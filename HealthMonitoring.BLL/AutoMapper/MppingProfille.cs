using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthMonitoring.BLL.Dtos.AccountUserDtos;
using HealthMonitoring.BLL.Dtos.ActivityDataDtos;
using HealthMonitoring.BLL.Dtos.AIModelDtos;
using HealthMonitoring.BLL.Dtos.ApplicationUserDtos;
using HealthMonitoring.DAL.Data.Models;
using HealthMonitoring.DAL.Data.Models.AIModels;
using Microsoft.AspNetCore.Identity.Data;

namespace HealthMonitoring.BLL.AutoMapper
{
    public class MppingProfille : Profile
    {
        public MppingProfille()
        {
     //       CreateMap<ActivityData, ActivityDataReadDto>()
     //.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));


            CreateMap<ActivityData, ActivityDataCreateDto>().ReverseMap();
            CreateMap<ActivityData, ActivityDataReadDto>().ReverseMap();
            CreateMap<ActivityData, ActivityDataUpdateDto>().ReverseMap();

            CreateMap<ApplicationUser, ApplicationUserReadDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserUpdateDto>().ReverseMap();

            CreateMap<ApplicationUser, LoginRequest>().ReverseMap();
            CreateMap<ApplicationUser, RegisterRequestDto>().ReverseMap();
            CreateMap<ApplicationUser, LoginResponseDto>().ReverseMap();


            CreateMap<BloodPressureReading, BloodPressureReadingDto>().ReverseMap();
            CreateMap<BloodPressurePrediction, BloodPressureReadingDto>().ReverseMap();
           // CreateMap<SensorDataPoint, DataPointDto>().ReverseMap();
            CreateMap<SensorDataSet, DataSetDto>()
            .ForMember(dest => dest.PPG, opt => opt.MapFrom(src => new List<double> { src.PPG }))
            .ForMember(dest => dest.ABP, opt => opt.MapFrom(src => new List<double> { src.ABP }))
            .ForMember(dest => dest.ECG, opt => opt.MapFrom(src => new List<double> { src.ECG }))
            .ReverseMap();




        }
    }
}
