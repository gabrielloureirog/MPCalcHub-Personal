using AutoMapper;
using DTO = MPCalcHub.Application.DataTransferObjects;
using MPCalcHub.Domain.Entities;
using MSG = MPCalcHub.Application.DataTransferObjects.MessageBrokers;

namespace MPCalcHub.Application.Mappings;

public class ContactMapper : Profile
{
    public ContactMapper()
    {
        CreateMap<Contact, DTO.Contact>()
            .ConstructUsing(src => new DTO.Contact())
            .ReverseMap()
            .ConstructUsing(src => new Contact())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Removed, opt => opt.Ignore());

        CreateMap<DTO.BasicContact, Contact>()
            .ConstructUsing(src => new Contact());

        CreateMap<Contact, MSG.Contact>()
            .ReverseMap()
            .ConstructUsing(src => new Contact())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedAt, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Removed, opt => opt.Ignore());

        CreateMap<MSG.BasicContact, Contact>()
            .ConstructUsing(src => new Contact());
    }
}