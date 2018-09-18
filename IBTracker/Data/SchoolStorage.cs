using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using IBTracker.Common;
using IBTracker.Data.Tables;
using IBTracker.Parsing;

namespace IBTracker.Data
{

    public class SchoolStorage : IDisposable
    {
        private const string IndexParameterName = "Index";
        private const string DateParameterName = "Date";

        private SQLiteConnection connection;

        public SchoolStorage(string databasePath, string index)
        {
            connection = new SQLiteConnection(databasePath); //, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
            if (index != ReadParameter(IndexParameterName))
            {
                Clear(false);
                WriteParameter(IndexParameterName, index);
            }

            WriteParameter(DateParameterName, DateTime.Now.ToString());
        }

        public void Dispose()
        {
            connection.Close();
        }

        public void Write<T>(IEnumerable<T> records, bool clearAll = false)
        {
            if (clearAll)
            {
                Clear(true);
            }
            else
            {
                connection.DropTable<T>();
            }

            connection.CreateTable<T>();
            records.ToList().ForEach(r => connection.Insert(r));
        }

        public IEnumerable<T> Read<T>() where T : new()
        {
            if (Exists<T>())
                return connection.Table<T>();
            else      
                return Enumerable.Empty<T>();            
        }        

        private bool Exists<T>()
        {
            SQLiteCommand command = connection.CreateCommand("SELECT COUNT(1) FROM SQLITE_MASTER WHERE TYPE = @TYPE AND NAME = @NAME");
            command.Bind("@TYPE", "table");
            command.Bind("@NAME", typeof(T).Name);
            
            int result = command.ExecuteScalar<int>();
        
            return (result > 0);
        }

        private void Clear(bool keepParameters)
        {
            connection.DropTable<School>();
            if (!keepParameters)
            {
                connection.DropTable<Parameter>();
            }
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