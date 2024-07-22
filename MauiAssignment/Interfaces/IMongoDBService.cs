using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiAssignment.Models;

namespace MauiAssignment.Interfaces
{
    public interface IMongoDBService
    {
        Task SaveJsonToMongoAsync(string json);
        Task UpdateJsonToMongoAsync(StemData stemData);
        Task DeleteStemAsync(StemData stemData);
        Task<List<StemData>> GetAllDocumentsAsync();
    }

}
