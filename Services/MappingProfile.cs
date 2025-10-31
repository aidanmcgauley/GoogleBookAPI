using AutoMapper;
using GoogleBookAPI.Models.DTOs;
using GoogleBookAPI.Models.Entities;

namespace GoogleBookAPI.Services
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Author, AuthorDTO>();
            CreateMap<Category, CategoryDTO>();
            CreateMap<IndustryIdentifier, IndustryIdentifierDTO>();

            // Book → CustomerBookDTO
            CreateMap<Book, CustomerBookDTO>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors));


            // Book → ManagerBookDTO
            CreateMap<Book, ManagerBookDTO>()
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.IndustryIdentifiers, opt => opt.MapFrom(src => src.IndustryIdentifiers));

            // DTO → Entity mappings (for POST/PUT)
            CreateMap<ManagerBookDTO, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // avoid overwriting PK
                .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories))
                .ForMember(dest => dest.IndustryIdentifiers, opt => opt.MapFrom(src => src.IndustryIdentifiers));

            CreateMap<AuthorDTO, Author>();
            CreateMap<CategoryDTO, Category>();
            CreateMap<IndustryIdentifierDTO, IndustryIdentifier>();
        }
    }
}
