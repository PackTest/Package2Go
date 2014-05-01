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
            ConfigureCommentsMapping();
            ConfigureMessagesMapping();
            ConfigureOffersMapping();
        }

        private static void ConfigureItemsMapping()
        {
            Mapper.CreateMap<Items, ItemsView>().ForMember(d => d.currency, o=>o.MapFrom(s=>s.Currencies.code)).ForMember(d => d.status, o => o.MapFrom(s => s.ItemStatus.title));
        }

        private static void ConfigureRoutesMapping()
        {
            Mapper.CreateMap<Routes, RoutesView>();
            Mapper.CreateMap<vw_routes, RoutesView>().ForMember(d => d.waypointsList, o => o.MapFrom(ss => ss.waypoints.Split(new string[] { "->" }, StringSplitOptions.None)));
        }

        private static void ConfigureCommentsMapping()
        {
            Mapper.CreateMap<Comments, CommentsView>();
        }

        private static void ConfigureMessagesMapping()
        {
            Mapper.CreateMap<vw_messages, MessagesView>();
        }

        private static void ConfigureUserProfileMapping()
        {
            Mapper.CreateMap<UserProfileView, UserProfile>();

            Mapper.CreateMap<UserProfile, UserProfileView>().ForMember(dest => dest.gender, opt => opt.MapFrom(
                    src => (CProfile.genderType)Enum.Parse(typeof(CProfile.genderType), src.gender)));
            Mapper.CreateMap<UserProfile, UserProfileEdit>()
                .ForMember(dest => dest.gender, opt => opt.MapFrom(
                    src => (CProfile.genderType)Enum.Parse(typeof(CProfile.genderType), src.gender)));
        }

        private static void ConfigureOffersMapping()
        {
            Mapper.CreateMap<Offers, OffersView>();
        }
    }
}