namespace Churchee.Module.ChurchSuite.Helpers
{
    internal class Grouping
    {
        public int? Sequence { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }
        public decimal? LocationLatitude { get; set; }
        public decimal? LocationLongitude { get; set; }
        public string ImageSmallUrl { get; set; }
        public string ImageMediumUrl { get; set; }
        public string ImageLargeUrl { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Grouping grouping &&
                   Sequence == grouping.Sequence &&
                   Name == grouping.Name &&
                   Description == grouping.Description &&
                   LocationName == grouping.LocationName &&
                   LocationAddress == grouping.LocationAddress &&
                   LocationLatitude == grouping.LocationLatitude &&
                   LocationLongitude == grouping.LocationLongitude &&
                   ImageSmallUrl == grouping.ImageSmallUrl &&
                   ImageMediumUrl == grouping.ImageMediumUrl &&
                   ImageLargeUrl == grouping.ImageLargeUrl;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Sequence);
            hash.Add(Name);
            hash.Add(Description);
            hash.Add(LocationName);
            hash.Add(LocationAddress);
            hash.Add(LocationLatitude);
            hash.Add(LocationLongitude);
            hash.Add(ImageSmallUrl);
            hash.Add(ImageMediumUrl);
            hash.Add(ImageLargeUrl);
            return hash.ToHashCode();
        }
    }
}
