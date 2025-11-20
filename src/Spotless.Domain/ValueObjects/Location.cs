namespace Spotless.Domain.ValueObjects
{
    public record Location
    {
        public decimal? Latitude { get; init; }
        public decimal? Longitude { get; init; }


        protected Location() { }

        public Location(decimal? latitude, decimal? longitude)
        {

            if (latitude.HasValue && (latitude.Value < -90 || latitude.Value > 90))
            {
                throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90 degrees.");
            }
            if (longitude.HasValue && (longitude.Value < -180 || longitude.Value > 180))
            {
                throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180 degrees.");
            }

            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
