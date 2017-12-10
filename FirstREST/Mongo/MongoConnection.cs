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

        public static double GetAccountClosingValue(int accountId)
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

        public static double GetAccountCredit(int account, string begin, string end)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>("Journals");

            var aggregate = coll.Aggregate()
                                         .Unwind(x => x["transaction"])
                                         .Match(new BsonDocument 
                                                    {{ "transaction.TransactionDate", new BsonDocument {
                                                        {"$gte", begin},
                                                        {"$lt", end}
                                                    }}}
                                          )
                                          .Unwind(x => x["transaction.Lines.creditLine"])
                                          .Match(new BsonDocument 
                                                    {{ "transaction.Lines.creditLine.AccountID", new BsonRegularExpression("/^" + account +  ".*/")                                                       
                                                    }}
                                          )
                                         .Group(new BsonDocument { 
                                                            { "_id", "$transaction.Lines.creditLine.AccountID" }, 
                                                            { "total_credit", new BsonDocument("$sum", "$transaction.Lines.creditLine.CreditAmount")}
                                         });

            var list = aggregate.ToList();
            System.Diagnostics.Debug.WriteLine(list.ToJson()[0]);
            if (list.Count > 0)
            {
                return list[0].ToBsonDocument()["total_credit"].AsDouble;
            }

            return 0;

        }

        public static double GetAccountDebit(int account, string begin, string end)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var coll = db.GetCollection<BsonDocument>("Journals");

            var aggregate = coll.Aggregate()
                                         .Unwind(x => x["transaction"])
                                         .Match(new BsonDocument 
                                                    {{ "transaction.TransactionDate", new BsonDocument {
                                                        {"$gte", begin},
                                                        {"$lt", end}
                                                    }}}
                                          )
                                          .Unwind(x => x["transaction.Lines.debitLine"])
                                          .Match(new BsonDocument 
                                                    {{ "transaction.Lines.debitLine.AccountID", new BsonRegularExpression("/^" + account +  ".*/")                                                       
                                                    }}
                                          )
                                         .Group(new BsonDocument { 
                                                            { "_id", "$transaction.Lines.debitLine.AccountID" }, 
                                                            { "total_debit", new BsonDocument("$sum", "$transaction.Lines.debitLine.DebitAmount")}
                                         });

            var list = aggregate.ToList();

            if (list.Count > 0)
            {
                return list[0].ToBsonDocument()["total_debit"].AsDouble;
            }

            return 0;

        }

        public static double GetAccountOpeningValue(int accountId)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            JToken balaceSheet = new JObject();

            var coll = db.GetCollection<BsonDocument>("Accounts");

            var filter = "{ AccountID: '" + accountId + "'}";

            var query = coll.FindSync(filter).ToList();

            if (query.Count > 0)
                return query[0].ToBsonDocument()["OpeningDebitBalance"].AsDouble - query[0].ToBsonDocument()["OpeningCreditBalance"].AsDouble;

            return 0;
        }


        public static string GetAccountInRange(int account, string begin, string end)
        {
            double initialAccount = GetAccountOpeningValue(account);
            double periodCredit = GetAccountCredit(account, begin, end);
            double periodDebit = GetAccountDebit(account, begin, end);

            JObject res = new JObject();

            res.Add("account", account);
            res.Add("value", initialAccount + periodDebit - periodCredit);


            return res.ToString();

        }



        private static JObject getRatioObject(string name, double value, string explanation)
        {
            JObject obj = new JObject();

            obj.Add("name", name);
            obj.Add("value", value);
            obj.Add("explanation", explanation);

            return obj;
        }

        public static string GetNetIncome()        {
            JObject netIncome = new JObject();


            double account71 = GetAccountClosingValue(71);
            double account72 = GetAccountClosingValue(72);
            double account73 = GetAccountClosingValue(73);
            double account74 = GetAccountClosingValue(74);
            double account75 = GetAccountClosingValue(75);
            double account78 = GetAccountClosingValue(78);
            double account79 = GetAccountClosingValue(79);
            double account761 = GetAccountClosingValue(761);
            double account762 = GetAccountClosingValue(762);
            double account763 = GetAccountClosingValue(763);
            double account61 = GetAccountClosingValue(61);
            double account62 = GetAccountClosingValue(62);
            double account63 = GetAccountClosingValue(63);
            double account64 = GetAccountClosingValue(64);
            double account65 = GetAccountClosingValue(65);
            double account67 = GetAccountClosingValue(67);
            double account68 = GetAccountClosingValue(68);
            double account69 = GetAccountClosingValue(69);
            double account812 = GetAccountClosingValue(812);


            double netIncomeVal = Math.Abs(account71 + account72) + account75 + account73 + account74 - account61 - account62 -
                account63 + account762 - account65 + account763 - account67 + account78 - account68 + account761 - account64 + account79 - account69 + account812;

            netIncome.Add("name", "Net Income");
            netIncome.Add("value", netIncomeVal);

            return netIncome.ToString();

        }

        public static string GetFinancialRatios()
        {
            JArray ratios = new JArray();

            //*************** Aux Values ********************
            //Income statement--------------- 
            double account71 = GetAccountClosingValue(71);
            double account72 = GetAccountClosingValue(72);
            double account73 = GetAccountClosingValue(73);
            double account74 = GetAccountClosingValue(74);
            double account75 = GetAccountClosingValue(75);
            double account78 = GetAccountClosingValue(78);
            double account79 = GetAccountClosingValue(79);
            double account761 = GetAccountClosingValue(761);
            double account762 = GetAccountClosingValue(762);
            double account763 = GetAccountClosingValue(763);         
            double account61 = GetAccountClosingValue(61);
            double account62 = GetAccountClosingValue(62);
            double account63 = GetAccountClosingValue(63);
            double account64 = GetAccountClosingValue(64);
            double account65 = GetAccountClosingValue(65);
            double account67 = GetAccountClosingValue(67);
            double account68 = GetAccountClosingValue(68);
            double account69 = GetAccountClosingValue(69);
            double account812 = GetAccountClosingValue(812);
            //-----------------------------------------
            
            // balance sheet-------------------------
            // non current assets
            double class4 = GetAccountClosingValue(41) +
                            GetAccountClosingValue(42) +
                            GetAccountClosingValue(43) +
                            GetAccountClosingValue(44) +
                            GetAccountClosingValue(45) +
                            GetAccountClosingValue(46);

            // current assets             
           
            double class3 = GetAccountClosingValue(32) +
                            GetAccountClosingValue(33) +
                            GetAccountClosingValue(34) +
                            GetAccountClosingValue(35) +
                            GetAccountClosingValue(36) +
                            GetAccountClosingValue(39);

            double account21 = GetAccountClosingValue(21);
            double class1 = GetAccountClosingValue(11) +
                            GetAccountClosingValue(12) +
                            GetAccountClosingValue(13) +
                            GetAccountClosingValue(14);

            // non current liabilities
            double account25 = Math.Abs(GetAccountClosingValue(25));


            // current liabilities
            double account22 = Math.Abs(GetAccountClosingValue(22));
            double account24 = Math.Abs(GetAccountClosingValue(24));
            double account26 = Math.Abs(GetAccountClosingValue(26));

          
            //----------------------------------
            
            
            double netSales = Math.Abs(account71 + account72) + account75 + account73 + account74 - account61 - account62 - 
                account63 + account762 - account65 + account763 - account67 + account78 - account68;
            double ebit = netSales + (account761 - account64);
            double interest = account79 - account69;
            double netEarnings = ebit + interest + account812;
            double totalAssets = class4 + class3 + account21 + class1;
            double currentLiabilities = account22 + account24 + account26;
            double nonCurrentLiabilities = account25;
            double equity = totalAssets - currentLiabilities - nonCurrentLiabilities;            
            double inventory = class3;
            double costsOfGoodSold = account61;
            double accountsReceivable = Math.Abs(account21);
            double sales = Math.Abs(GetAccountClosingValue(71));
            double accountsPayable = account22;
            double purchases = GetAccountClosingValue(31);
            double cash = GetAccountClosingValue(11);





            //------------------------------------------------


            //Analysis of return
            JObject analysisOfReturn = new JObject();

            analysisOfReturn.Add("name", "Analysis of return");
            
            JArray analysisOfReturnValues = new JArray();
            analysisOfReturnValues.Add(getRatioObject("Return on sales", (netEarnings/netSales) * 100, ""));
            analysisOfReturnValues.Add(getRatioObject("Return on Assets", (netEarnings/totalAssets) * 100, ""));
            analysisOfReturnValues.Add(getRatioObject("Return on Equity", (netEarnings/equity) * 100, ""));

            analysisOfReturn.Add("values", analysisOfReturnValues);

            //Efficiency

            JObject efficiency = new JObject();

            efficiency.Add("name", "Efficiency");

            JArray efficiencyValues = new JArray();
            efficiencyValues.Add(getRatioObject("Asset turnover", (sales/totalAssets) * 100, ""));
            efficiencyValues.Add(getRatioObject("Average inventory period", (inventory/costsOfGoodSold) * 365, ""));
            efficiencyValues.Add(getRatioObject("Inventory turnover", (costsOfGoodSold/inventory) * 100, ""));
            efficiencyValues.Add(getRatioObject("Average collection period", (accountsReceivable/sales) * 365, ""));
            efficiencyValues.Add(getRatioObject("Average payment period", (accountsPayable/purchases) * 365, ""));


            efficiency.Add("values", efficiencyValues);

            //Liquidity

            JObject liquidity = new JObject();

            liquidity.Add("name", "Liquidity");

            JArray liquidityValues = new JArray();
            liquidityValues.Add(getRatioObject("Current ratio", totalAssets/currentLiabilities, ""));
            liquidityValues.Add(getRatioObject("Quick ratio", (totalAssets - inventory)/currentLiabilities, ""));
            liquidityValues.Add(getRatioObject("Cash ratio", class1 / currentLiabilities, ""));
            liquidityValues.Add(getRatioObject("Working Capital", totalAssets - currentLiabilities, ""));

            liquidity.Add("values", liquidityValues);

            //Financial stability and Leverage

            JObject financialStabilityAndLeverage = new JObject();

            financialStabilityAndLeverage.Add("name", "Financial stability and Leverage");

            JArray financialStabilityAndLeverageValues = new JArray();
            financialStabilityAndLeverageValues.Add(getRatioObject("Equity to assets ratio", equity/totalAssets, ""));
            //financialStabilityAndLeverageValues.Add(getRatioObject("Debt to Equity", 0, ""));
            //financialStabilityAndLeverageValues.Add(getRatioObject("Coverage of fixed investments", 0, ""));
            financialStabilityAndLeverageValues.Add(getRatioObject("Interest Coverage", interest != 0 ? ebit/interest : 0, ""));

            financialStabilityAndLeverage.Add("values", financialStabilityAndLeverageValues);

            //Growth ratios
            /*
            JObject growthRatios = new JObject();

            growthRatios.Add("name", "Growth Ratios");

            JArray growthRatiosValues = new JArray();
            growthRatiosValues.Add(getRatioObject("Sales", 0, ""));
            growthRatiosValues.Add(getRatioObject("Profit", 0, ""));
            growthRatiosValues.Add(getRatioObject("Assets", 0, ""));
            growthRatiosValues.Add(getRatioObject("Inventory", 0, ""));

            growthRatios.Add("values", growthRatiosValues);
            */

            //--------------------- Ratios object ------------------
            ratios.Add(analysisOfReturn);
            ratios.Add(efficiency);
            ratios.Add(liquidity);
            ratios.Add(financialStabilityAndLeverage);
            //ratios.Add(growthRatios);
            

            return ratios.ToString();
        }

        private static JObject getIncomeStatementObject(string name, double value, Boolean result)
        {
            JObject obj = new JObject();

            obj.Add("name", name);
            obj.Add("value", value);
            obj.Add("result", result);

            return obj;
        }

        public static string GetIncomeStatement()
        {
            JArray obj = new JArray();

            double account71 = GetAccountClosingValue(71);
            double account72 = GetAccountClosingValue(72);
            double account73 = GetAccountClosingValue(73);
            double account74 = GetAccountClosingValue(74);
            double account75 = GetAccountClosingValue(75);
            double account78 = GetAccountClosingValue(78);
            double account79 = GetAccountClosingValue(79);
            double account761 = GetAccountClosingValue(761);
            double account762 = GetAccountClosingValue(762);
            double account763 = GetAccountClosingValue(763);         
            double account61 = GetAccountClosingValue(61);
            double account62 = GetAccountClosingValue(62);
            double account63 = GetAccountClosingValue(63);
            double account64 = GetAccountClosingValue(64);
            double account65 = GetAccountClosingValue(65);
            double account67 = GetAccountClosingValue(67);
            double account68 = GetAccountClosingValue(68);
            double account69 = GetAccountClosingValue(69);
            double account812 = GetAccountClosingValue(812);


            obj.Add(getIncomeStatementObject("Net Sales", Math.Abs(account71 + account72), false));
            obj.Add(getIncomeStatementObject("Exploration Subsidy", account75, false));
            obj.Add(getIncomeStatementObject("Variation in production inventories", account73, false));
            obj.Add(getIncomeStatementObject("Work for the entity itself", account74, false));
            obj.Add(getIncomeStatementObject("Costs of goods sold", -account61, false));
            obj.Add(getIncomeStatementObject("Supplies and external services", -account62, false));
            obj.Add(getIncomeStatementObject("Expenses with people", -account63, false));
            obj.Add(getIncomeStatementObject("Impairments (losses / reversals)", account762 - account65, false));
            obj.Add(getIncomeStatementObject("Provisions (increases / reductions)", account763 - account67, false));
            obj.Add(getIncomeStatementObject("Other income and gains", account78, false));
            obj.Add(getIncomeStatementObject("Other expenses and losses", -account68, false));

            double total = Math.Abs(account71 + account72) + account75 + account73 + account74 - account61 - account62 - 
                account63 + account762 - account65 + account763 - account67 + account78 - account68;

            obj.Add(getIncomeStatementObject("Result before depreciation, financing expenses and taxes", total, true));
            //---------------           

            obj.Add(getIncomeStatementObject("Depreciation and amortization expenses/reversals", account761 - account64, false));
            total += account761 - account64;

            obj.Add(getIncomeStatementObject("Operating income (before financing expenses and taxes)", total, true));
        
            //-----------
            obj.Add(getIncomeStatementObject("Net financing expense", account79 - account69, false));
            total += account79 - account69;
            obj.Add(getIncomeStatementObject("Income before income taxes", total, true));

            //---------
            obj.Add(getIncomeStatementObject("Income taxes for the period", account812, false));
            total += account812;
            obj.Add(getIncomeStatementObject("Net income", total, true));

            
            return obj.ToString();
        }
        
        private static JObject getFieldObject(string name, double value)
        {
            JObject obj = new JObject();

            obj.Add("name", name);
            obj.Add("value", value);

            return obj;
        }

        public static string GetBalanceSheet()
        {
            JObject obj = new JObject();

            //ASSETS--------------------------------------------------------------------------------------------
            JObject assets =  new JObject();
            
            //Non current assets----        

            // sub topics
            double class4 = GetAccountClosingValue(41) +
                            GetAccountClosingValue(42) +
                            GetAccountClosingValue(43) +
                            GetAccountClosingValue(44) +
                            GetAccountClosingValue(45) +
                            GetAccountClosingValue(46);

            JArray nonCurrentAssetsValues = new JArray();

            JObject investment = getFieldObject("Investments/Tangible Assets", class4);

            nonCurrentAssetsValues.Add(investment);

            // object
            JObject nonCurrentAssets = new JObject();
                nonCurrentAssets.Add("name", "Non Current Assets");
                nonCurrentAssets.Add("total", class4);
                nonCurrentAssets.Add("values", nonCurrentAssetsValues);
       

            // current assets
           
            // sub topics
            double class3 = GetAccountClosingValue(32) +
                            GetAccountClosingValue(33) +
                            GetAccountClosingValue(34) +
                            GetAccountClosingValue(35) +
                            GetAccountClosingValue(36) +
                            GetAccountClosingValue(39);

            double account21 = GetAccountClosingValue(21);

            double class1 = GetAccountClosingValue(11) +
                            GetAccountClosingValue(12) +
                            GetAccountClosingValue(13) +
                            GetAccountClosingValue(14);
           
            JArray currentAssetsValues = new JArray();

            JObject inventory = getFieldObject("Inventory", class3);     
            JObject accountsReceivable = getFieldObject("Accounts Receivable", account21);
            JObject liquidAssets = getFieldObject("Liquid Assets (Cash, Bank Deposits and other)", class1);  
              

            currentAssetsValues.Add(inventory);
            currentAssetsValues.Add(accountsReceivable);
            currentAssetsValues.Add(liquidAssets);

         
            // object
            JObject currentAssets = new JObject();
            currentAssets.Add("name", "Current Assets");
            currentAssets.Add("total", class3 + account21 + class1);
            currentAssets.Add("values", currentAssetsValues);
                   
            
            //assets obj           
            JArray assetsFields = new JArray();

            assetsFields.Add(nonCurrentAssets);
            assetsFields.Add(currentAssets);

            assets.Add("total", class4 + class3 + account21 + class1);
            assets.Add("fields", assetsFields);


            //LIABILITIES-------------------------------------------------------------------------------------
            JObject liabilities = new JObject();

            //Non current liabilities

            // sub topics
            double account25 = Math.Abs(GetAccountClosingValue(25));


            JArray nonCurrentLiabilitiesValues = new JArray();

            JObject funding = getFieldObject("Obtained funding (loans and others)", account25);

            nonCurrentLiabilitiesValues.Add(funding);

            // object
            JObject nonCurrentLiabilities = new JObject();
            nonCurrentLiabilities.Add("name", "Non Current Liabilities");
            nonCurrentLiabilities.Add("total", account25);
            nonCurrentLiabilities.Add("values", nonCurrentLiabilitiesValues);


            // current liabilities
            double account22 = Math.Abs(GetAccountClosingValue(22));
            double account24 = Math.Abs(GetAccountClosingValue(24));
            double account26 = Math.Abs(GetAccountClosingValue(26));

            JArray currentLiabilitiesValues = new JArray();

            JObject accountsPayable = getFieldObject("Accounts Payable", account22);
            JObject state = getFieldObject("State and other public entities", account24);
            JObject shareholders = getFieldObject("Shareholders", account26);

            currentLiabilitiesValues.Add(accountsPayable);
            currentLiabilitiesValues.Add(state);
            currentLiabilitiesValues.Add(shareholders);
            
     
            // object
            JObject currentLiabilities = new JObject();
            currentLiabilities.Add("name", "Current Liabilities");
            currentLiabilities.Add("total", account22 + account24 + account26);
            currentLiabilities.Add("values", currentLiabilitiesValues);

            //liabilities obj     

            JArray liabilitiesFields = new JArray();

            liabilitiesFields.Add(nonCurrentLiabilities);
            liabilitiesFields.Add(currentLiabilities);

            liabilities.Add("total", account22 + account24 + account26 + account25);
            liabilities.Add("fields", liabilitiesFields);


            // balance sheet
            obj.Add("Assets", assets);
            obj.Add("Liabilities", liabilities);
           


            return obj.ToString();
            // GFK
            /*
            JObject ativos_nao_correntes = new JObject();
            ativos_nao_correntes.Add("Ativos fixos tangíveis", GetAccountClosingValue(43) + GetAccountClosingValue(453));
            ativos_nao_correntes.Add("Propriedades de investimento", GetAccountClosingValue(42));
            ativos_nao_correntes.Add("Activos intangíveis", GetAccountClosingValue(44) - GetAccountClosingValue(441) + GetAccountClosingValue(454));
            ativos_nao_correntes.Add("Activos biológicos", GetAccountClosingValue(37));
            ativos_nao_correntes.Add("Participações financeiras - método eq. patrimonial", GetAccountClosingValue(411) + GetAccountClosingValue(2) + GetAccountClosingValue(3));
            ativos_nao_correntes.Add("Participações financeiras - outros métodos", GetAccountClosingValue(414) + GetAccountClosingValue(9));
            ativos_nao_correntes.Add("Accionistas / sócios", GetAccountClosingValue(26));
            ativos_nao_correntes.Add("Outros activos financeiros", GetAccountClosingValue(1421) + GetAccountClosingValue(1431) + GetAccountClosingValue(415) + GetAccountClosingValue(416) + GetAccountClosingValue(417) + GetAccountClosingValue(418) + GetAccountClosingValue(419));
            ativos_nao_correntes.Add("Activos por impostos diferidos", GetAccountClosingValue(274));

            JObject ativos_correntes = new JObject();
            ativos_correntes.Add("Inventários", GetAccountClosingValue(32) + GetAccountClosingValue(33) + GetAccountClosingValue(34) + GetAccountClosingValue(35) + GetAccountClosingValue(36) + GetAccountClosingValue(39));
            ativos_correntes.Add("Activos Biológicos", GetAccountClosingValue(37));
            ativos_correntes.Add("Clientes", GetAccountClosingValue(21) - GetAccountClosingValue(218));
            ativos_correntes.Add("Adiantamentos a fornecedores", GetAccountClosingValue(228) + GetAccountClosingValue(229));
            ativos_correntes.Add("Estado e outros entres públicos", GetAccountClosingValue(24));
            ativos_correntes.Add("Accionistas / Sócios", GetAccountClosingValue(26) - GetAccountClosingValue(261) - GetAccountClosingValue(262));
            ativos_correntes.Add("Outras contas a receber", GetAccountClosingValue(22) + GetAccountClosingValue(23) + GetAccountClosingValue(27) + GetAccountClosingValue(29));
            ativos_correntes.Add("Diferimentos", GetAccountClosingValue(28 ));
            ativos_correntes.Add("Activos financeiros detidos para negociação", GetAccountClosingValue(1421));
            ativos_correntes.Add("Outros activos financeiros", GetAccountClosingValue(1431));
            ativos_correntes.Add("Activos não correntes detidos para venda", GetAccountClosingValue(46) + GetAccountClosingValue(49));
            ativos_correntes.Add("Caixa e depósitos bancários", GetAccountClosingValue(11) + GetAccountClosingValue(12) + GetAccountClosingValue(13));

            JObject capitais_proprios = new JObject();
            capitais_proprios.Add("Capital realizado", GetAccountClosingValue(51) + GetAccountClosingValue(261) + GetAccountClosingValue(262));
            capitais_proprios.Add("Ações (quotas) próprias", GetAccountClosingValue(52));
            capitais_proprios.Add("Outros instrumentos de capital próprio", GetAccountClosingValue(53));
            capitais_proprios.Add("Prémios de emissão", GetAccountClosingValue(54));
            capitais_proprios.Add("Reservas legais", GetAccountClosingValue(551));
            capitais_proprios.Add("Outras reservas", GetAccountClosingValue(552));
            capitais_proprios.Add("Resultados transitados", GetAccountClosingValue(56));
            capitais_proprios.Add("Ajustamentos em ativos financeiros", GetAccountClosingValue(57));
            capitais_proprios.Add("Excedentes de revalorização", GetAccountClosingValue(58));
            capitais_proprios.Add("Outras variações no capital próprio", GetAccountClosingValue(59));
            capitais_proprios.Add("Resultado líquido do exercício", GetAccountClosingValue(818));

            JObject passivos_nao_correntes = new JObject();
            passivos_nao_correntes.Add("Provisões", GetAccountClosingValue(29));
            passivos_nao_correntes.Add("Financiamentos obtidos", GetAccountClosingValue(25));
            passivos_nao_correntes.Add("Responsabilidades por benefícios pós-emprego", GetAccountClosingValue(273));
            passivos_nao_correntes.Add("Passivos por impostas diferidos", GetAccountClosingValue(2742));
            passivos_nao_correntes.Add("Outras contas a pagar", GetAccountClosingValue(21) + GetAccountClosingValue(23) + GetAccountClosingValue(26) + GetAccountClosingValue(27));

            JObject passivos_correntes = new JObject();
            passivos_correntes.Add("Fornecedores", GetAccountClosingValue(22));
            passivos_correntes.Add("Adiantamento de clientes", GetAccountClosingValue(218));
            passivos_correntes.Add("Estado e outros entes públicos", GetAccountClosingValue(24));
            passivos_correntes.Add("Accionistas/Sócios", GetAccountClosingValue(26) - GetAccountClosingValue(261) - GetAccountClosingValue(262));
            passivos_correntes.Add("Financiamentos obtidos", GetAccountClosingValue(25));
            passivos_correntes.Add("Outras contas a pagar", GetAccountClosingValue(21) + GetAccountClosingValue(23) + GetAccountClosingValue(27));
            passivos_correntes.Add("Diferimentos", GetAccountClosingValue(28));
            passivos_correntes.Add("Passivos financeiros detidos para negociação", GetAccountClosingValue(1422));
            passivos_correntes.Add("Outros passivos financeiros", GetAccountClosingValue(1432));
            
            obj.Add("Ativos não correntes", ativos_nao_correntes);
            obj.Add("Ativos correntes", ativos_correntes);
            obj.Add("Capitais Próprios", capitais_proprios);
            obj.Add("Passivo não corrente", passivos_nao_correntes);
            obj.Add("Passivo corrente", passivos_correntes);
            
            return obj.ToString();
            
            */
        }

    }
}