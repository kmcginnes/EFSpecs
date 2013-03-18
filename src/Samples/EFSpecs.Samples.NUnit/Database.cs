using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace EFSpecs.Samples.EF5ConsoleSample.Tests
{
    public class Database
    {
        private const string ConnectionString = "Server=(localdb)\\v11.0;database=master;trusted_connection=true";

        public void Execute(string query)
        {
            var scriptFile = GetResourceTextFile(query);
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                foreach (var queryCommand in scriptFile.Split(
                    new[] {"GO"}, StringSplitOptions.RemoveEmptyEntries))
                {
                    using (var command = new SqlCommand(queryCommand, conn))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public string GetResourceTextFile(string filename)
        {
            var result = string.Empty;

            //using (var stream = this.GetType().Assembly.
            //           GetManifestResourceStream("assembly.folder." + filename))
            using (var stream = Assembly.GetExecutingAssembly()
                       .GetManifestResourceStream(this.GetType().Namespace + "." + filename))
            {
                using (var sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}