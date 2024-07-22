using MauiAssignment.Models;
using Microsoft.Maui.Graphics.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MauiAssignment.Interfaces;


namespace MauiAssignment.Services
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoDBService(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<BsonDocument>(collectionName);
        }

        public async Task SaveJsonToMongoAsync(string json)
        {

            var deleteResult = await _collection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);

            Console.WriteLine($"{deleteResult.DeletedCount} documents deleted.");

            var document = BsonDocument.Parse(json);
            await _collection.InsertOneAsync(document);
        }

        public async Task UpdateJsonToMongoAsync(StemData stemData)
        {

            var filter = Builders<BsonDocument>.Filter.Eq("HarvestedProduction.Machine.MachineKey", stemData.MachineKey) &
             Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("HarvestedProduction.Machine.Stem",
             Builders<BsonDocument>.Filter.Eq("StemKey", stemData.StemKey));

            // Define the update statement
            var update = Builders<BsonDocument>.Update
                .Set("HarvestedProduction.Machine.Stem.$.StemCoordinates.Latitude.#text", stemData.Latitude)
                .Set("HarvestedProduction.Machine.Stem.$.StemCoordinates.Longitude.#text", stemData.Longitude)
                .Set("HarvestedProduction.Machine.Stem.$.StemCoordinates.Altitude", stemData.Altitude)
                .Set("HarvestedProduction.Machine.Stem.$.StemCoordinates.@receiverPosition", stemData.receiverPosition)
                .Set("HarvestedProduction.Machine.Stem.$.StemCoordinates.@coordinateReferenceSystem", stemData.coordinateReferenceSystem)
                .Set("HarvestedProduction.Machine.Stem.$.StemNumber", stemData.StemNumber);
                

            // Execute the update command
            var result = await _collection.UpdateOneAsync(filter, update);

            // Check if the update was successful
            if (result.MatchedCount > 0 && result.ModifiedCount > 0)
            {
                Console.WriteLine("Update successful.");
            }
            else
            {
                Console.WriteLine("No documents were updated.");
            }


        }


        public async Task DeleteStemAsync(StemData stemData)
    {
        // Define the filter to find the document and the specific Stem element to remove
        var filter = Builders<BsonDocument>.Filter.Eq("HarvestedProduction.Machine.MachineKey", stemData.MachineKey) &
                     Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("HarvestedProduction.Machine.Stem",
                     Builders<BsonDocument>.Filter.Eq("StemKey", stemData.StemKey));

        // Define the update statement to pull (remove) the specific Stem element from the array
        var update = Builders<BsonDocument>.Update.PullFilter(
            "HarvestedProduction.Machine.Stem",
            Builders<BsonDocument>.Filter.Eq("StemKey", stemData.StemKey));

        // Execute the update command
        var result = await _collection.UpdateOneAsync(filter, update);

        // Check if the update was successful
        if (result.MatchedCount > 0 && result.ModifiedCount > 0)
        {
            Console.WriteLine("Deletion successful.");
        }
        else
        {
            Console.WriteLine("No documents were updated.");
        }
    }




    public async Task<List<StemData>> GetAllDocumentsAsync()
        {
            List<StemData> stemDataList = new List<StemData>();

            var latitude = "";
            var longitude = "";
            var altitude = "";
            var MachineKey = "";
            var stemKey = "";
            var stemNumber = "";
            var receiverPosition = "";
            var coordinateReferenceSystem = "";


            // Define filter to find documents where "HarvestedProduction.Stem" exists
            var filter = Builders<BsonDocument>.Filter.Exists("HarvestedProduction.Machine");

            // Define projection to include only specific fields within "HarvestedProduction.Stem"
            var projection = Builders<BsonDocument>.Projection
                .Include("HarvestedProduction.Machine")
                .Include("HarvestedProduction.Machine");

            // Fetch the documents with the filter and projection
            var documents = await _collection.Find(filter).Project(projection).ToListAsync();

            // Iterate through the documents and extract required fields
            foreach (var document in documents)
            {


                MachineKey = document["HarvestedProduction"]["Machine"]["MachineKey"].AsString;
                var stemArray = document["HarvestedProduction"]["Machine"]["Stem"].AsBsonArray;

                foreach (var stem in stemArray)
                {
                    stemKey = stem["StemKey"].AsString;
                    stemNumber = stem["StemNumber"].AsString;
                    var stemCoordinates = stem["StemCoordinates"].AsBsonDocument;

                    var latitudeObject = stemCoordinates["Latitude"].AsBsonDocument;
                    var longitudeObject = stemCoordinates["Longitude"].AsBsonDocument;
                    var AltitudeObject = stemCoordinates["Altitude"].AsString;

                    latitude = latitudeObject["#text"].AsString;
                    longitude = longitudeObject["#text"].AsString;
                    altitude = AltitudeObject.ToString();
                    receiverPosition = stemCoordinates["@receiverPosition"].AsString;
                    coordinateReferenceSystem = stemCoordinates["@coordinateReferenceSystem"].AsString;


                    Console.WriteLine($"StemKey: {stemKey}, Latitude: {latitude}, Longitude: {longitude}");

                    var stemData = new StemData
                    {
                        MachineKey = MachineKey,
                        StemKey = stemKey,
                        StemNumber = stemNumber,
                        receiverPosition = receiverPosition,
                        coordinateReferenceSystem = coordinateReferenceSystem,
                        Latitude = Convert.ToDecimal(latitude.ToString()),
                        Longitude = Convert.ToDecimal(longitude.ToString()),
                        Altitude = Convert.ToDecimal(altitude.ToString()),
                    };




                    stemDataList.Add(stemData);

                    // Create a new StemData object

                }

            }

            return stemDataList;
        }

    }

}
