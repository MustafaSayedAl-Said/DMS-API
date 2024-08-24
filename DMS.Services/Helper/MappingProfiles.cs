﻿using AutoMapper;
using DMS.Core.Dto;
using DMS.Core.Entities;

namespace DMS.Services.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // User Mappings
            CreateMap<User, LoginDto>().ReverseMap();

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
