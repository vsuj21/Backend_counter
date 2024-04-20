using UnityEngine;
using MongoDB.Bson;
using UnityEngine.UI;
using MongoDB.Driver;
using TMPro;

public class CounterManager : MonoBehaviour
{
    public TMP_Dropdown territoryDropdown;
    public TMP_Dropdown idDropdown;
    public TMP_Text countText;

    private IMongoCollection<BsonDocument> collection;

    private void Start()
    {
        
        string connectionString = "mongodb+srv://sujayv21:vXcs5jz6vC7JFaCO@cluster0.9wnardq.mongodb.net/";
        MongoClient client = new MongoClient(connectionString);
        IMongoDatabase database = client.GetDatabase("CounterDB");
        collection = database.GetCollection<BsonDocument>("CounterCollection");

        InitializeCounters();
    }

   
    private void InitializeCounters()
    {
       
        for (int territoryIndex = 0; territoryIndex < 4; territoryIndex++)
        {
            for (int idIndex = 0; idIndex < 4; idIndex++)
            {
                
                BsonDocument document = new BsonDocument
                {
                    { "territory", territoryIndex },
                    { "id", idIndex },
                    { "count", 0 }
                };

                
                collection.InsertOne(document);
            }
        }
    }



    private void UpdateCountDisplay(int count)
    {
       
        countText.SetText(count.ToString());
        
        
    }

    public void IncrementCount()
    {
        int territoryIndex = territoryDropdown.value;
        int idIndex = idDropdown.value;

        
        var filter = Builders<BsonDocument>.Filter.Eq("territory", territoryIndex) &
                     Builders<BsonDocument>.Filter.Eq("id", idIndex);

        var update = Builders<BsonDocument>.Update.Inc("count", 1);

        
        collection.UpdateOne(filter, update);

        int updatedCount = QueryCount(territoryIndex, idIndex);
        UpdateCountDisplay(updatedCount);
    }

    
    public void DecrementCount()
    {
        int territoryIndex = territoryDropdown.value;
        int idIndex = idDropdown.value;

       
        var filter = Builders<BsonDocument>.Filter.Eq("territory", territoryIndex) &
                     Builders<BsonDocument>.Filter.Eq("id", idIndex);

        
        var update = Builders<BsonDocument>.Update.Inc("count", -1);

        
        collection.UpdateOne(filter, update);

   
        int updatedCount = QueryCount(territoryIndex, idIndex);
        UpdateCountDisplay(updatedCount);
    }

    
    private int QueryCount(int territoryIndex, int idIndex)
    {
        
        var filter = Builders<BsonDocument>.Filter.Eq("territory", territoryIndex) &
                     Builders<BsonDocument>.Filter.Eq("id", idIndex);

        var document = collection.Find(filter).FirstOrDefault();

        
        if (document != null && document.Contains("count"))
        {
            return document["count"].AsInt32;
        }
        else
        {
            return 0;
        }
    }
}
