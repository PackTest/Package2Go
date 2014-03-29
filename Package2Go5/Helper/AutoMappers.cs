using AutoMapper;
using Package2Go5.Constants;
using Package2Go5.Models.EditModels;
using Package2Go5.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Package2Go5.Helper
{
    public static class AutoMapperWebConfiguration
    {
        public static void Configure()
        {
            ConfigureItemsMapping();
            ConfigureRoutesMapping();
            ConfigureUserProfileMapping();
        }

        private static void ConfigureItemsMapping()
        {
            Mapper.CreateMap<Items, ItemsView>();
        }

        private static void ConfigureRoutesMapping()
        {
            Mapper.CreateMap<Routes, RoutesView>();
            Mapper.CreateMap<vw_routes, RoutesView>();
        }

        private static void ConfigureUserProfileMapping()
        {
            Mapper.CreateMap<UserProfileView, UserProfile>();
            Mapper.CreateMap<UserProfile, UserProfileEdit>()
                .ForMember(dest => dest.gender, opt => opt.MapFrom(
                    src => (CProfile.genderType)Enum.Parse(typeof(CProfile.genderType), src.gender)));
                    }
    }
}