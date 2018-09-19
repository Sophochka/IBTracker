using SQLite;

namespace IBTracker.Data
{
    [Table("Parameters")]
    public class Parameter
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}