using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text;

namespace DAL.Converters
{
    internal class ObfuscationConverter : ValueConverter<string, string>
    {
        public ObfuscationConverter() : base(x => ToDatabase(x), x => FromDatabase(x))
        {
        }

        private static string FromDatabase(string x)
        {
            if (x == null)
                return null;
            var bytes = Convert.FromBase64String(x);
            return Encoding.UTF8.GetString(bytes);
        }

        private static string ToDatabase(string x)
        {
            if (x == null)
                return null;
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(x));
            
        }
    }
}
