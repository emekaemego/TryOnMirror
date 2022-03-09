using AutoMapper;
using SymaCord.TryOnMirror.Entities;
using SymaCord.TryOnMirror.UI.Web.ViewModels;

namespace SymaCord.TryOnMirror.UI.Web.App_Start
{
    public class AutoMapperBootstrapper
    {
        public static void Initialize()
        {
            Mapper.CreateMap<HairCategory, HairCategoryModel>();
            Mapper.CreateMap<HairCategoryModel, HairCategory>();

            Mapper.CreateMap<Hairstyle, HairstyleModel>();
            Mapper.CreateMap<HairstyleModel, Hairstyle>();

            Mapper.CreateMap<Glass, GlassModel>();
            Mapper.CreateMap<GlassModel, Glass>();

            Mapper.CreateMap<ContactLens, ContactLensModel>();
            Mapper.CreateMap<ContactLensModel, ContactLens>();

            Mapper.CreateMap<NewSalonModel, Salon>();
            Mapper.CreateMap<Salon, NewSalonModel>();

            Mapper.CreateMap<EditSalonModel, Salon>();
            Mapper.CreateMap<Salon, EditSalonModel>();
            
            Mapper.CreateMap<AddressModel, Address>();
            Mapper.CreateMap<Address, AddressModel>();

            Mapper.CreateMap<PhoneAndWebContactModel, Salon>();
            Mapper.CreateMap<Salon, PhoneAndWebContactModel>();

            Mapper.CreateMap<NewHairstyleBookingModel, HairstyleBooking>();
            Mapper.CreateMap<HairstyleBooking, NewHairstyleBookingModel>();

            Mapper.CreateMap<Brand, BrandModel>();
        }
    }
}