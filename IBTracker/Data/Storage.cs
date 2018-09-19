using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace IBTracker.Data
{
    public class QueryRecord
    {
        public string Name { get; set; }
    }

    public class Storage : IDisposable
    {
        private const string IndexParameterName = "Index";
        private const string DateParameterName = "Date";

        private string searchIndex;        

        private SQLiteConnection connection;

        public Storage(string databasePath, string index)
        {
            searchIndex = index;
            connection = new SQLiteConnection(databasePath);
            if (searchIndex != ReadParameter(IndexParameterName))
            {
                Clear();
            }
        }

        public void Clear()
        {
            var command = connection.CreateCommand("SELECT NAME FROM SQLITE_MASTER WHERE TYPE = 'table'");
            var records = command.ExecuteQuery<QueryRecord>();
            foreach(var rec in records.Where(r => !r.Name.StartsWith("sqlite")))
            {
                command = connection.CreateCommand($"DROP TABLE '{rec.Name}'");
                command.ExecuteNonQuery();
            }

            WriteParameter(IndexParameterName, searchIndex);
            WriteParameter(DateParameterName, DateTime.Now.ToString());
        }

        public void Dispose()
        {
            connection.Close();
        }

        public IEnumerable<T> Read<T>() where T : new()
        {
            if (Exists<T>())
                return connection.Table<T>();
            else      
                return null;            
        }        

        public void Write<T>(IEnumerable<T> records)
        {
            connection.DropTable<T>();
            connection.CreateTable<T>();
            records.ToList().ForEach(r => connection.Insert(r));
        }

        private string GetTableName<T>()
        {
            var type = typeof(T);
            var tableNameAttr = type.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() as TableAttribute;
            return tableNameAttr?.Name ?? type.Name;
        }

        private bool Exists<T>()
        {
            var command = connection.CreateCommand("SELECT COUNT(1) FROM SQLITE_MASTER WHERE TYPE = @TYPE AND NAME = @NAME");
            command.Bind("@TYPE", "table");
            command.Bind("@NAME", GetTableName<T>());
            
            int result = command.ExecuteScalar<int>();
        
            return (result > 0);
        }

        private string ReadParameter(string name)
        {
            if (Exists<Parameter>())
            {
                var parameter = connection.Find<Parameter>(p => p.Name == name);
                return parameter?.Value;
            }

            return null;
        }

        private void WriteParameter(string name, string value)
        {
            connection.CreateTable<Parameter>();
            var parameter = new Parameter
            {
                Name = name,
                Value = value
            };

            if(connection.Update(parameter) == 0)
            {
                connection.Insert(parameter);
            }
        }
    }
}