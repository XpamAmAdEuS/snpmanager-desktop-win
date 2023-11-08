using System;
using AutoMapper;
using Snp.Models;

namespace Snp.Repository;

public class MapperConfig
{
    public static Mapper InitializeAutomapper()
    {
        //Provide all the Mapping Configuration
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<V1.ProtoCustomerRepo.Types.ProtoCustomer, Customer>().ReverseMap();
            cfg.CreateMap<V1.ProtoSiteRepo.Types.ProtoSite, Site>()
                // //Monday
                // .ForMember(dest => dest.OpenMon, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.OpenMon) ))
                //
                // .ForMember(dest => dest.CloseMon, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.CloseMon)))
                // //Tuesday
                // .ForMember(dest => dest.OpenTue, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.OpenTue)))
                //
                // .ForMember(dest => dest.CloseTue, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.CloseTue)))
                //
                // //wednesday
                // .ForMember(dest => dest.OpenWed, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.OpenWed)))
                // .ForMember(dest => dest.CloseWed, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.CloseWed)))
                //
                // .ForMember(dest => dest.OpenThu, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.OpenThu)))
                // .ForMember(dest => dest.CloseThu, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.CloseThu)))
                //
                //
                // .ForMember(dest => dest.OpenFri, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.OpenFri)))
                // .ForMember(dest => dest.CloseFri, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.CloseFri)))
                //
                // .ForMember(dest => dest.OpenSat, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.OpenSat)))
                // .ForMember(dest => dest.CloseSat, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.CloseSat)))
                //
                // .ForMember(dest => dest.OpenSun, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.OpenSun)))
                // .ForMember(dest => dest.CloseSun, 
                //     act => act.MapFrom(
                //         src => TimeOnly.Parse(src.CloseSun)))
               
                
                
                .ReverseMap();
            cfg.CreateMap<V1.ProtoUserRepo.Types.ProtoUser, User>().ReverseMap();
            
            // //Configuring Employee and EmployeeDTO
            // cfg.CreateMap<Employee, EmployeeDTO>()
            //     //Provide Mapping Configuration of FullName and Name Property
            //     .ForMember(dest => dest.FullName, act => act.MapFrom(src => src.Name))
            //     
            //     //Provide Mapping Dept of FullName and Department Property
            //     .ForMember(dest => dest.Dept, act => act.MapFrom(src => src.Department));
            
        });
        //Create an Instance of Mapper and return that Instance
        var mapper = new Mapper(config);
        return mapper;
    }
}