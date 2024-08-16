using AutoMapper;
using DMS.Core.Entities;
using DocumentManagementSystem.Dto;
namespace Document_Management_System.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<Document, DocumentDto>();
            CreateMap<MyDirectory, MyDirectoryDto>();
            CreateMap<Workspace, WorkspaceDto>();

            CreateMap<UserDto, User>();
            CreateMap<DocumentDto, Document>();
            CreateMap<MyDirectoryDto, MyDirectory>();
            CreateMap<WorkspaceDto, Workspace>();
        }
    }
}
