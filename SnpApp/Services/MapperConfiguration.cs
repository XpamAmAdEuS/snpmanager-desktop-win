using System;
using Windows.Media.Core;
using AutoMapper;
using CommunityToolkit.WinUI.UI.Controls;
using Snp.V1;
using SnpApp.Models;

namespace SnpApp.Services;

public class MapperConfig
{
    
    class DirectionFormatter : IValueConverter<DataGridSortDirection, string> {
        
        public string Convert(DataGridSortDirection source, ResolutionContext context)
        {
            string result ="ASC";
            switch (source)
            {
                case DataGridSortDirection.Ascending:
                    result=  "ASC";
                    break;
                case DataGridSortDirection.Descending:
                    result= "DESC";
                    break;
            }
            return result;
        }
    }
    
    public class SizeLimitModelFormatter : IValueConverter<SizeLimitModel, ulong> {
        public ulong Convert(SizeLimitModel source, ResolutionContext context)
        {
            return source.Value;
        }
    }
    
    public class SizeLimitModelFormatterInvert : IValueConverter<ulong, SizeLimitModel> {
        
        public SizeLimitModel Convert(ulong source, ResolutionContext context)
        {
            
            SizeLimitModel result = new SizeLimitModel();
            switch (source)
            {
                case 8000000000:
                    result.Name = "8Gb";
                    result.Value = 8000000000;
                    break;
                case 16000000000:
                    result.Name = "16Gb";
                    result.Value = 16000000000;
                    break;
                case 24000000000:
                    result.Name = "24Gb";
                    result.Value = 24000000000;
                    break;
                case 32000000000:
                    result.Name = "32Gb";
                    result.Value = 32000000000;
                    break;
                
            }

            return result;
        }
    }
    
    public static Mapper InitializeAutomapper()
    {
        
        //Provide all the Mapping Configuration
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<ProtoCustomerRepo.Types.ProtoCustomer, Customer>().ForMember(d => d.SizeLimit,
                opt =>
                    opt.ConvertUsing(new SizeLimitModelFormatterInvert(), src => src.SizeLimit)).ForMember(
                dest => dest.SizeLimitValue,
                act => act.MapFrom(
                    src => src.SizeLimit));
            cfg.CreateMap<Customer,ProtoCustomerRepo.Types.ProtoCustomer>().ForMember(d => d.SizeLimit, 
                opt => 
                    opt.ConvertUsing(new SizeLimitModelFormatter(), src => src.SizeLimit));
            cfg.CreateMap<SearchRequestModel,SearchRequest>()
                .ForMember(d => d.SortDirection, 
                    opt => 
                        opt.ConvertUsing(new DirectionFormatter(), src => src.SortDirection));
            cfg.CreateMap<ProtoSiteRepo.Types.ProtoSite, Site>()
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
            cfg.CreateMap<ProtoUserRepo.Types.ProtoUser, User>().ReverseMap();
            
            // cfg.CreateMap<ProtoImportMusicRepo.Types.ImportMusic, MusicImport>()
            //     .ForMember(dest => dest.Source, act => 
            //         act.MapFrom(src => MediaSource.CreateFromUri(new Uri($"http://192.168.1.36:50051/v1/music/import/file/{src.Hash}.mp3"))));
            
            cfg.CreateMap<ProtoImportMusicRepo.Types.ImportMusic, MusicImport>();
            
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

