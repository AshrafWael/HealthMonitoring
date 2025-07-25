﻿using AutoMapper;
using HealthMonitoring.BLL.APIRequst;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vonage.Users;

namespace HealthMonitoring.BLL.AutoMapper
{
    public class MppingProfille : Profile
    {
        public MppingProfille()
        {
            //       CreateMap<ActivityData, ActivityDataReadDto>()
            //.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));


            CreateMap<ActivityData, DailyActivityDataDto>().ReverseMap();

            CreateMap<ApplicationUser, UserActivityJsonDto>().ReverseMap();


            CreateMap<CaloriesPrediction, CaloriesPredictionResponseDto>();
               // .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.ActivityData.Day));
              // .ForMember(dest => dest.UpdatedWeight, opt => opt.MapFrom(src => src.ActivityData.WeightKg))
              //  .ForMember(dest => dest.ActivityData, opt => opt.MapFrom(src => src.ActivityData));

            CreateMap<ApplicationUser, ApplicationUserReadDto>().ReverseMap();
            CreateMap<ApplicationUser, ApplicationUserUpdateDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDataReadDto>().ReverseMap();

            CreateMap<ApplicationUser, ApplicationUserReadDto>()
            .ForMember(dest => dest.Role, opt => opt.Ignore());

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

            CreateMap<EmergencyContact, EmergencyContactDto>();
            CreateMap<EmergencyContact, EmergencyContactWithUsersDto>()
            .ForMember(dest => dest.ConnectedUsers,
              opt => opt.MapFrom(src => src.ApplicationUsers.Select(u => u.FirstName).ToList()))
            .ForMember(dest => dest.ConnectedUsers,
              opt => opt.MapFrom(src => src.ApplicationUsers.Select(u => u.Email).ToList()));
            CreateMap<CreateEmergencyContactDto, EmergencyContact>().ReverseMap();
            CreateMap<EmergencyContact, EmergencyContactUpdateDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.EmergencyContacts,
                opt => opt.MapFrom(src => src.EmergencyContacts));
            CreateMap<CreateEmergencyContactDto, EmergencyContact>()
       .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
       .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<EmergencyContact, EmergencyContactResponseDto>()
                .ForMember(dest => dest.ConnectedUserIds,
                    opt => opt.MapFrom(src => src.ApplicationUsers.Select(u => u.Id).ToList()));
            CreateMap<ApplicationUser, ConectedUserDto>().ReverseMap();



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
