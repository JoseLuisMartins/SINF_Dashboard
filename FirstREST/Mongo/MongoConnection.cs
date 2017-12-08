using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using FirstREST.Lib_Primavera.Model;
using Newtonsoft.Json.Linq;

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
                var ids = list[0].ToBsonDocument()["products"].AsBsonArray;
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
                                                       
            var list = aggregate.ToList();

            if (list.Count > 0) {
                var ids = list[0].ToBsonDocument()["customers"].AsBsonArray;
                return  GetCollectionsByIds("Customers", "CustomerID", ids);
            }


            return "";  
        }

        public static string GetCollectionsByIds(string collection, string field, BsonArray ids)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>(collection);

            var query = new BsonDocument(field, new BsonDocument("$in", ids));

            return coll.FindSync(query).ToList().ToJson(settings);
        }

        public static double GetAccountValue(int accountId)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            JToken balaceSheet = new JObject();

            var coll = db.GetCollection<BsonDocument>("Accounts");

            var filter = "{ AccountID: '" +  accountId + "'}";

            var query = coll.FindSync(filter).ToList();

            if (query.Count > 0)
                return query[0].ToBsonDocument()["ClosingDebitBalance"].AsDouble - query[0].ToBsonDocument()["ClosingCreditBalance"].AsDouble;
            
            return 0;
        }

        public static string GetTop10Products(string begin, string end)
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
                                         .Project(new BsonDocument { 
                                                                    { "line.ProductCode", 1 }, 
                                                                    { "line.ProductDescription", 1 }, 
                                                                    { "total", 
                                                                        new BsonDocument("$multiply", new BsonArray { "$line.Quantity", "$line.UnitPrice" }) }
                                                                    })
                                         .Group(new BsonDocument { 
                                                            { "_id", "$line.ProductCode" }, 
                                                            { "total_sold", new BsonDocument("$sum", "$total") }, 
                                                            { "product_description", new BsonDocument("$first", "$line.ProductDescription") } 
                                                            })
                                         .Sort(new BsonDocument { { "total_sold", -1 } })
                                         .Limit(10);


            return aggregate.ToList().ToJson(settings);
        }

        public static string GetTop10Customers(string begin, string end)
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
                                         .Sort(new BsonDocument { { "total_spent", -1 } })
                                         .Limit(10);

            return aggregate.ToList().ToJson(settings);
        }
        

        public static string GetBalanceSheet()
        {
            JObject obj = new JObject();

            JObject ativos_nao_correntes = new JObject();
            ativos_nao_correntes.Add("Ativos fixos tangíveis", GetAccountValue(43) + GetAccountValue(453));
            ativos_nao_correntes.Add("Propriedades de investimento", GetAccountValue(42));
            ativos_nao_correntes.Add("Activos intangíveis", GetAccountValue(44) - GetAccountValue(441) + GetAccountValue(454));
            ativos_nao_correntes.Add("Activos biológicos", GetAccountValue(37));
            ativos_nao_correntes.Add("Participações financeiras - método eq. patrimonial", GetAccountValue(411) + GetAccountValue(2) + GetAccountValue(3));
            ativos_nao_correntes.Add("Participações financeiras - outros métodos", GetAccountValue(414) + GetAccountValue(9));
            ativos_nao_correntes.Add("Accionistas / sócios", GetAccountValue(26));
            ativos_nao_correntes.Add("Outros activos financeiros", GetAccountValue(1421) + GetAccountValue(1431) + GetAccountValue(415) + GetAccountValue(416) + GetAccountValue(417) + GetAccountValue(418) + GetAccountValue(419));
            ativos_nao_correntes.Add("Activos por impostos diferidos", GetAccountValue(274));

            JObject ativos_correntes = new JObject();
            ativos_correntes.Add("Inventários", GetAccountValue(32) + GetAccountValue(33) + GetAccountValue(34) + GetAccountValue(35) + GetAccountValue(36) + GetAccountValue(39));
            ativos_correntes.Add("Activos Biológicos", GetAccountValue(37));
            ativos_correntes.Add("Clientes", GetAccountValue(21) - GetAccountValue(218));
            ativos_correntes.Add("Adiantamentos a fornecedores", GetAccountValue(228) + GetAccountValue(229));
            ativos_correntes.Add("Estado e outros entres públicos", GetAccountValue(24));
            ativos_correntes.Add("Accionistas / Sócios", GetAccountValue(26) - GetAccountValue(261) - GetAccountValue(262));
            ativos_correntes.Add("Outras contas a receber", GetAccountValue(22) + GetAccountValue(23) + GetAccountValue(27) + GetAccountValue(29));
            ativos_correntes.Add("Diferimentos", GetAccountValue(28 ));
            ativos_correntes.Add("Activos financeiros detidos para negociação", GetAccountValue(1421));
            ativos_correntes.Add("Outros activos financeiros", GetAccountValue(1431));
            ativos_correntes.Add("Activos não correntes detidos para venda", GetAccountValue(46) + GetAccountValue(49));
            ativos_correntes.Add("Caixa e depósitos bancários", GetAccountValue(11) + GetAccountValue(12) + GetAccountValue(13));

            JObject capitais_proprios = new JObject();
            capitais_proprios.Add("Capital realizado", GetAccountValue(51) + GetAccountValue(261) + GetAccountValue(262));
            capitais_proprios.Add("Ações (quotas) próprias", GetAccountValue(52));
            capitais_proprios.Add("Outros instrumentos de capital próprio", GetAccountValue(53));
            capitais_proprios.Add("Prémios de emissão", GetAccountValue(54));
            capitais_proprios.Add("Reservas legais", GetAccountValue(551));
            capitais_proprios.Add("Outras reservas", GetAccountValue(552));
            capitais_proprios.Add("Resultados transitados", GetAccountValue(56));
            capitais_proprios.Add("Ajustamentos em ativos financeiros", GetAccountValue(57));
            capitais_proprios.Add("Excedentes de revalorização", GetAccountValue(58));
            capitais_proprios.Add("Outras variações no capital próprio", GetAccountValue(59));
            capitais_proprios.Add("Resultado líquido do exercício", GetAccountValue(818));

            JObject passivos_nao_correntes = new JObject();
            passivos_nao_correntes.Add("Provisões", GetAccountValue(29));
            passivos_nao_correntes.Add("Financiamentos obtidos", GetAccountValue(25));
            passivos_nao_correntes.Add("Responsabilidades por benefícios pós-emprego", GetAccountValue(273));
            passivos_nao_correntes.Add("Passivos por impostas diferidos", GetAccountValue(2742));
            passivos_nao_correntes.Add("Outras contas a pagar", GetAccountValue(21) + GetAccountValue(23) + GetAccountValue(26) + GetAccountValue(27));

            JObject passivos_correntes = new JObject();
            passivos_correntes.Add("Fornecedores", GetAccountValue(22));
            passivos_correntes.Add("Adiantamento de clientes", GetAccountValue(218));
            passivos_correntes.Add("Estado e outros entes públicos", GetAccountValue(24));
            passivos_correntes.Add("Accionistas/Sócios", GetAccountValue(26) - GetAccountValue(261) - GetAccountValue(262));
            passivos_correntes.Add("Financiamentos obtidos", GetAccountValue(25));
            passivos_correntes.Add("Outras contas a pagar", GetAccountValue(21) + GetAccountValue(23) + GetAccountValue(27));
            passivos_correntes.Add("Diferimentos", GetAccountValue(28));
            passivos_correntes.Add("Passivos financeiros detidos para negociação", GetAccountValue(1422));
            passivos_correntes.Add("Outros passivos financeiros", GetAccountValue(1432));
            
            obj.Add("Ativos não correntes", ativos_nao_correntes);
            obj.Add("Ativos correntes", ativos_correntes);
            obj.Add("Capitais Próprios", capitais_proprios);
            obj.Add("Passivo não corrente", passivos_nao_correntes);
            obj.Add("Passivo corrente", passivos_correntes);

            return obj.ToString();
        }

    }
}