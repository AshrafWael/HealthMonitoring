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
using HealthMonitoring.BLL.Dtos.BloodPressureDto;
using HealthMonitoring.BLL.Dtos.EmergencyContactDtos;
using HealthMonitoring.BLL.Dtos.HealthInformationDtos;
using HealthMonitoring.BLL.Dtos.HeartRateDataDtos;
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

            CreateMap<EmergencyContact, EmergencyContactReadDto>().ReverseMap();
            CreateMap<EmergencyContactCreateDto, EmergencyContact>().ReverseMap();
            CreateMap<EmergencyContactUpdateDto, EmergencyContact>().ReverseMap();

            CreateMap<HeartRateData, HeartRateReadingDto>().ReverseMap();
            CreateMap<HeartRateData, HeartRateDataReadDto>().ReverseMap();
            CreateMap<HeartRateData, HeartRateRequstDto>().ReverseMap();
            CreateMap<ECGReading, HeartRateRequstDto>()
           .ForMember(dest => dest.ECG, opt => opt.MapFrom(src => new List<double> { src.ECG }))
           .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<HeartDisease, HeartDiseaseRequstDto>().ReverseMap();
            CreateMap<ECGReading, HeartDiseaseRequstDto>()
           .ForMember(dest => dest.ECG, opt => opt.MapFrom(src => new List<double> { src.ECG }))
           .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => src.Timestamp))
           .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<BloodPressureReading, BloodPressurReadDto>()
         .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString())).ReverseMap();

            CreateMap<HeartRateData, HeartRateReadingDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString())).ReverseMap();

            CreateMap<HeartDisease, HeartDiseasesReadingDto>().ReverseMap();

        }
    }
}
