using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAssignment.Common
{
    public static class DatabaseConnection
    {
        public static string ConnectionString = "mongodb://localhost:27017/";
        public static string DatabaseName = "Mauiassignment";
        public static string CollectionName = "HPRData";
        public static string xmlFilePath = @"D:\Assignment07\data.hpr";

    }
}
