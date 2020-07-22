using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NalogaProAim.Classes
{
    public static class Utility
    {
        public static ObservableCollection<Kos> kosData;
        public static ObservableCollection<DelovniNalog> delovniNalogData;

        public static string ConnVal(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
