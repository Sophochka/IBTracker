using SQLite;

namespace IBTracker.Data.Tables
{
    public class Parameter
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}