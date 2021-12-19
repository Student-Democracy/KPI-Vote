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
            CreateMap<Group, GroupModel>()
                .ForMember(p => p.UserIds, c => c.MapFrom(group => group.Users.Select(user => user.Id)))
                .ForMember(p => p.VotingIds, c => c.MapFrom(group => group.Votings.Select(voting => voting.Id)))
                .ReverseMap();
            CreateMap<Flow, FlowModel>()
                .ForMember(p => p.GroupIds, c => c.MapFrom(flow => flow.Groups.Select(group => group.Id)))
                .ForMember(p => p.VotingIds, c => c.MapFrom(flow => flow.Votings.Select(voting => voting.Id)))
                .ReverseMap();
            CreateMap<Faculty, FacultyModel>()
                .ForMember(p => p.FlowIds, c => c.MapFrom(flow => flow.Flows.Select(flow => flow.Id)))
                .ForMember(p => p.VotingIds, c => c.MapFrom(flow => flow.Votings.Select(voting => voting.Id)))
                .ReverseMap();
            CreateMap<Ban, BlockModel>().ReverseMap();
        }
    }
}
