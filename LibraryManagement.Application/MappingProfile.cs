using AutoMapper;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Entities;
using System.Linq;   

namespace LibraryManagement.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(d => d.PublishedDate,
                    o => o.MapFrom(s => s.PublishedYear.HasValue
                        ? new DateTime(s.PublishedYear.Value, 1, 1)
                        : (DateTime?)null))
                .ForMember(d => d.Quantity, o => o.MapFrom(_ => 0))
                .ForMember(d => d.Categories, o => o.MapFrom(s => s.BookCategories.Select(bc => bc.Category)));

            CreateMap<Category, CategoryDto>();
        }
    }
}