namespace Churchee.Module.Site.Helpers
{
    public static class PageTypes
    {
        public static readonly Guid BlogListingPageTypeId = Guid.Parse("3c5c3620-f8a2-42ef-8a49-3e82912363cf");
        public static readonly Guid BlogDetailPageTypeId = Guid.Parse("f4c623c3-0047-4940-ad01-ca42db5ba54b");
        public static readonly Guid EventListingPageTypeId = Guid.Parse("f9c4c0cf-3908-4993-aa31-c59310ada766");
        public static readonly Guid EventDetailPageTypeId = Guid.Parse("1325d848-a18a-4b09-8d38-bdb1c94f885a");

        public static readonly List<Guid> AllButBlogListingTypes =
        [
            BlogDetailPageTypeId,
            EventListingPageTypeId,
            EventDetailPageTypeId
        ];

        public static readonly List<Guid> AllReservedType =
        [
            BlogListingPageTypeId,
            BlogDetailPageTypeId,
            EventListingPageTypeId,
            EventDetailPageTypeId
        ];
    }
}
