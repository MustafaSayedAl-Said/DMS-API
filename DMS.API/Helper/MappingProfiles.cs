using AutoMapper;
using DMS.API.Helper;
using DMS.Core.Dto;
using DMS.Core.Entities;

namespace Document_Management_System.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // User Mappings
            CreateMap<User, UserDto>().ReverseMap();

            // Document Mappings
            CreateMap<Document, DocumentGetDto>()
                .ForMember(d => d.DocumentContent, o => o.MapFrom<DocumentUrlResolver>());
            CreateMap<DocumentDto, Document>().ReverseMap();

            // Directory Mappings
            CreateMap<MyDirectory, MyDirectoryDto>().ReverseMap();

            // Workspace Mappings
            CreateMap<Workspace, WorkspaceDto>().ReverseMap();
        }
    }
}
