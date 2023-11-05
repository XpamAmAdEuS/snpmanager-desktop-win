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
            cfg.CreateMap<Snp.V1.Customer, Customer>().ReverseMap();
            cfg.CreateMap<Snp.V1.Site, Site>().ReverseMap();
            cfg.CreateMap<Snp.V1.User, User>().ReverseMap();
            
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