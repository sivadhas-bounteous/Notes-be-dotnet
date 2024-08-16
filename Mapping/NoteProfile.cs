using AutoMapper;
using Notes.DTO;
using Notes.Models;

namespace Notes.Mapping
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<Note, NoteResponse>();
            CreateMap<NoteInput, Note>();
        }
    }
}
