using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using FirstREST.Lib_Primavera.Model;

namespace FirstREST.Mongo
{
    public static class MongoConnection
    {
        static MongoClient client;
        static IMongoDatabase db;

        static SaftParser saft;

        static MongoConnection()
        {

            client = new MongoClient("mongodb://localhost:27017");
            client.DropDatabase("SAFTDB");
            db = client.GetDatabase("SAFTDB");
            saft = new SaftParser();
        }

        public static void Add(string collection, string json)
        {

            BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(json);

            db.GetCollection<BsonDocument>(collection).InsertOne(document);

        }

        public static void AddMany(string collection, string json)
        {

            BsonArray documents = BsonSerializer.Deserialize<BsonArray>(json);

            db.GetCollection<BsonDocument>(collection).InsertMany(GenerateList(documents.ToList()));

        }

        public static List<BsonDocument> GenerateList(List<BsonValue> values)
        {
            List<BsonDocument> docs = new List<BsonDocument>();
            foreach (BsonValue v in values)
            {
                if ((BsonDocument)v != null)
                    docs.Add((BsonDocument)v);
            }
            return docs;
        }

        public static string GetCollection(string collection)
        {

            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            var coll = db.GetCollection<BsonDocument>(collection);
            var filter = new BsonDocument();

            return coll.Find(filter).ToList().ToJson(settings);
        }

        public static string GetCollectionByDate(string collection, string field, string begin, string end)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>(collection);

            var aggregate = coll.Aggregate()
                                         .Match(new BsonDocument 
                                                    {{ field, new BsonDocument {
                                                        {"$gte", begin},
                                                        {"$lt", end}
                                                    }}}
                                            );
            return aggregate.ToList().ToJson(settings);
        }

        public static string GetTotalNetSales(string begin, string end)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>("Invoices");

            var aggregate = coll.Aggregate()
                                         .Match(new BsonDocument 
                                                    {{ "InvoiceDate", new BsonDocument {
                                                        {"$gte", begin},
                                                        {"$lt", end}
                                                    }}}
                                            )
                                         .Group(new BsonDocument { { "_id", "null" }, { "total", new BsonDocument("$sum", "$DocumentTotals.NetTotal") } });            
            return aggregate.ToList().ToJson(settings);
        }


        public static string GetCollectionById(string collection, string field, string id)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>(collection);

            var filter = "{" + field + ": '" + id + "'}";

            return coll.FindSync(filter).ToList().ToJson(settings);
        }


        public static string GetProductSales(string id, string begin, string end)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>("Invoices");

            var aggregate = coll.Aggregate()
                                         .Match(new BsonDocument 
                                                    {{ "InvoiceDate", new BsonDocument {
                                                        {"$gte", begin},
                                                        {"$lt", end}
                                                    }}}
                                            )
                                         .Unwind(x => x["line"])
                                         .Project(new BsonDocument { { "line.ProductCode", 1 }, { "total", new BsonDocument("$multiply", new BsonArray { "$line.Quantity", "$line.UnitPrice" }) } })
                                         .Group(new BsonDocument { { "_id", "$line.ProductCode" }, { "total_sold", new BsonDocument("$sum", "$total") } })
                                         .Match(new BsonDocument { { "_id", id } });

            
            return aggregate.ToList().ToJson(settings);
        }

        public static string GetCustomerTotalSpent(string id, string begin, string end)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>("Invoices");

            var aggregate = coll.Aggregate()
                                         .Match(new BsonDocument 
                                                    {{ "InvoiceDate", new BsonDocument {
                                                        {"$gte", begin},
                                                        {"$lt", end}
                                                    }}}
                                            )
                                         .Group(new BsonDocument { { "_id", "$CustomerID" }, { "total_spent", new BsonDocument("$sum", "$DocumentTotals.NetTotal") } })
                                         .Match(new BsonDocument { { "_id", id } }); ;
            return aggregate.ToList().ToJson(settings);
        }

        public static string GetCustomerBoughtProducts(string customerId)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>("Invoices");

            var aggregate = coll.Aggregate()
                                         .Unwind(x => x["line"])
                                         .Group(new BsonDocument { { "_id", "$CustomerID" }, { "products", new BsonDocument("$addToSet", "$line.ProductCode") } })
                                         .Match(new BsonDocument { { "_id", customerId } });
            var list = aggregate.ToList();

            if (list.Count > 0) { 
                var ids = aggregate.ToList()[0].ToBsonDocument()["products"].AsBsonArray;
                return GetCollectionsByIds("Products", "ProductCode", ids);
            }
            
           return "";
        }

        public static string GetProductCustomers(string productId)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>("Invoices");

            var aggregate = coll.Aggregate()
                                         .Unwind(x => x["line"])
                                         .Group(new BsonDocument { { "_id", "$line.ProductCode" }, { "customers", new BsonDocument("$addToSet", "$CustomerID") } })
                                         .Match(new BsonDocument { { "_id", productId } }); ;
            
            var ids = aggregate.ToList()[0].ToBsonDocument()["customers"].AsBsonArray;

            return GetCollectionsByIds("Customers", "CustomerID", ids);           
        }

        public static string GetCollectionsByIds(string collection, string field, BsonArray ids)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>(collection);

            var query = new BsonDocument(field, new BsonDocument("$in", ids));

            return coll.FindSync(query).ToList().ToJson(settings);
        }
    }
}