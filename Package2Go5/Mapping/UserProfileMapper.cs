using AutoMapper;
using Package2Go5.Models.EditModels;

namespace Package2Go5.Mapping
{
    public static class UserProfileMapper
    {
           public static void Configure()
           {
               Mapper.CreateMap<UserProfile, UserProfileEdit>();
           }
    }
}