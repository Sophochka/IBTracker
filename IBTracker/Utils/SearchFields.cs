namespace IBTracker.Utils
{
    public class SearchFields
    {
        public string Region { get; set; }
        public string Country { get; set; }
        public string Keywords { get; set; }
        public string Language { get; set; }
        public string BoardingFacilities { get; set; }
        public string SchoolGender { get; set; }

        public override string ToString()
        {
            return $"{Region}|{Country}|{Keywords}|{Language}|{BoardingFacilities}|{SchoolGender}";
        }
    }
}

