using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvaFrontier.Lib.EasyStorage
{
    public static class Global
    {
        public static List<SaveGameDescription> SaveGameDescriptions { get; set; }

        static Global()
        {
            SaveGameDescriptions = new List<SaveGameDescription>();
        }
    }
}
