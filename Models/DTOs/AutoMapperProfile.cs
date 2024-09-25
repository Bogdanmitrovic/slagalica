using AutoMapper;

namespace Slagalica.Models.DTOs;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<PlayerAnswer, PlayerAnswerDTO>();
        CreateMap<GameState, GameStateDTO>()
            .ForMember(dest => dest.Answers, opt => opt.Condition(src => src.RoundEnded || src.GameEnded))
            .ForMember(dest => dest.PlayerAnswers, opt => opt.MapFrom(src => src.PlayerAnswers.Select(pa => pa==null?null:new PlayerAnswerDTO
            {
                Username = src.PlayerUsernames[Array.IndexOf(src.PlayerIds, pa.PlayerId)],
                Answer = pa.Answer
            }).ToArray()));
    }
}