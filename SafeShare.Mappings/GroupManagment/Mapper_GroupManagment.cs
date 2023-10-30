﻿using AutoMapper;
using SafeShare.DataAccessLayer.Models;
using SafeShare.DataTransormObject.GroupManagment;

namespace SafeShare.Mappings.GroupManagment;
public class Mapper_GroupManagment : Profile
{
    public Mapper_GroupManagment()
    {
        CreateMap<DTO_CreateGroup, Group>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.GroupName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.GroupDescription))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false));
    }
}