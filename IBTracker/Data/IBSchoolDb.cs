using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using IBTracker.Common;
using IBTracker.Data.Tables;

namespace IBTracker.Data
{

    public class IBSchoolDb : IDisposable
    {
        private const string IndexParameterName = "Index";
        private const string DateParameterName = "Date";

        private SQLiteConnection database;

        public IBSchoolDb(string databasePath, string index)
        {
            database = new SQLiteConnection(databasePath);
            if (index != ReadParameter(IndexParameterName))
            {
                ClearTables(false);
                WriteParameter(IndexParameterName, index);
            }

            WriteParameter(DateParameterName, DateTime.Now.ToString());
        }

        public void Dispose()
        {
            database.Close();
        }

        public void CreateTable(IEnumerable<IBSchool> schools)
        {
            ClearTables(true);
            database.CreateTable<School>();
            schools.ToList().ForEach(s => database.Insert(new School(s)));
        }

        public IEnumerable<IBSchool> GetSchools()
        {
            if (TableExists<School>())
                return database.Table<School>().Select(s => s.ToObject());
            else      
                return Enumerable.Empty<IBSchool>();            
        }        

        public bool TableExists<T>()
        {
            SQLiteCommand command = database.CreateCommand("SELECT COUNT(1) FROM SQLITE_MASTER WHERE TYPE = @TYPE AND NAME = @NAME");
            command.Bind("@TYPE", "table");
            command.Bind("@NAME", typeof(T).Name);
            
            int result = command.ExecuteScalar<int>();
        
            return (result > 0);
        }

        private void ClearTables(bool keepParameters)
        {
            database.DropTable<School>();
            if (!keepParameters)
            {
                database.DropTable<Parameter>();
            }
        }

        private string ReadParameter(string name)
        {
            if (TableExists<Parameter>())
            {
                var parameter = database.Table<Parameter>().FirstOrDefault(p => p.Name == name);
                return parameter?.Value;
            }

            return null;
        }

        private void WriteParameter(string name, string value)
        {
            database.CreateTable<Parameter>();
            var parameter = new Parameter
            {
                Name = name,
                Value = value
            };

            if(database.Update(parameter) == 0)
            {
                database.Insert(parameter);
            }
        }
    }
}