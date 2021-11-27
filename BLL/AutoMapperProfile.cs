using AutoMapper;
using DAL.Entities;
using BLL.Models;
using System.Linq;

namespace BLL
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Voting, VotingModel>()
                .ForMember(p => p.Votes, 
                c => c.MapFrom(voting => voting.Votes.
                Select(x => new VoteModel() { Result = x.Result, UserId = x.UserId, VotingId = x.VotingId})))
                .ReverseMap();
            CreateMap<Vote, VoteModel>().ReverseMap();
            CreateMap<Appeal, AppealModel>().ReverseMap();
            CreateMap<Group, GroupModel>().ReverseMap();
        }
    }
}
