using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace IBTracker.Data
{
    public class Storage : IDisposable
    {
        private class QueryRecord
        {
            public string Name { get; set; }
        }

        private const string IndexParameterName = "Index";
        private const string DateParameterName = "Date";

        private string searchIndex;        

        private string[] CreationScript = 
        {
            "DROP VIEW VSchools",
            "CREATE VIEW VSchools AS " + 
                "SELECT Schools.*, Details.* " + 
                "FROM Schools " + 
                "INNER JOIN Details ON Details.School = Schools.Id"
        };

        public readonly SQLiteConnection Connection;

        public Storage(string databasePath, string index)
        {
            searchIndex = index;
            Connection = new SQLiteConnection(databasePath);
            if (searchIndex != ReadParameter(IndexParameterName))
            {
                Clear(true);
            }
        }

        public void Clear(bool full)
        {
            var command = Connection.CreateCommand("SELECT NAME FROM SQLITE_MASTER WHERE TYPE = 'table'");
            var records = command.ExecuteQuery<QueryRecord>().Where(
                    r => !r.Name.StartsWith("sqlite") || full && r.Name == GetTableName<PartLink>());

            var script = new List<string>(records.Select(r => $"DROP TABLE '{r.Name}'"));
            script.AddRange(CreationScript);
            ExecuteScript(script);
            WriteParameter(IndexParameterName, searchIndex);
            WriteParameter(DateParameterName, DateTime.Now.ToString());
        }

        public void Dispose()
        {
            Connection.Close();
        }

        public IEnumerable<T> Read<T>() where T : new()
        {
            if (Exists<T>())
                return Connection.Table<T>();
            else      
                return null;            
        }        

        public void Write<T>(IEnumerable<T> records)
        {
            Connection.DropTable<T>();
            Connection.CreateTable<T>();
            records.ToList().ForEach(r => Connection.Insert(r));
        }

        private void ExecuteScript(IEnumerable<string> script)
        {
            foreach(var line in script)
            {
                var command = Connection.CreateCommand(line);
                command.ExecuteNonQuery();
            }
        }

        private bool Exists<T>()
        {
            var command = Connection.CreateCommand("SELECT COUNT(1) FROM SQLITE_MASTER WHERE TYPE = @TYPE AND NAME = @NAME");
            command.Bind("@TYPE", "table");
            command.Bind("@NAME", GetTableName<T>());
            
            int result = command.ExecuteScalar<int>();
        
            return (result > 0);
        }

        private string GetTableName<T>()
        {
            var type = typeof(T);
            var tableNameAttr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
            return tableNameAttr?.Name ?? type.Name;
        }

        private string ReadParameter(string name)
        {
            if (Exists<Parameter>())
            {
                var parameter = Connection.Find<Parameter>(p => p.Name == name);
                return parameter?.Value;
            }

            return null;
        }

        private void WriteParameter(string name, string value)
        {
            Connection.CreateTable<Parameter>();
            var parameter = new Parameter
            {
                Name = name,
                Value = value
            };

            if(Connection.Update(parameter) == 0)
            {
                Connection.Insert(parameter);
            }
        }
    }
}