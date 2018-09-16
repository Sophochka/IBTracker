using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using IBTracker.Common;

namespace IBTracker.Data
{

    public class IBSchoolDb : IDisposable
    {
        private SQLiteConnection database;
        private List<TableMapping> mappings;

        public IBSchoolDb(string databasePath)
        {
            database = new SQLiteConnection(databasePath);
            mappings = database.TableMappings.ToList();
        }

        public void Dispose()
        {
            database.Close();
        }

        public void CreateTable(IEnumerable<IBSchool> schools)
        {
            ClearTables();
            database.CreateTable<School>();
            schools.ToList().ForEach(s => database.Insert(new School(s)));
        }

        private void ClearTables()
        {
            mappings.ForEach(m => database.DropTable(m));
        }
    }
}