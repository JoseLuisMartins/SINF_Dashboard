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
using System.IO;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Text;

namespace FirstREST.Mongo
{
    public static class MongoConnection
    {
        static MongoClient client;
        static IMongoDatabase db;



        static MongoConnection()
        {

            client = new MongoClient("mongodb://localhost:27017");
            client.DropDatabase("SAFTDB");
            db = client.GetDatabase("SAFTDB");

            string[] fileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Assets\\");

            foreach (string filename in fileNames)
                new SaftParser(getYear(filename));


        }

        public static string getYear(string stringXiraa)
        {
            var m = Regex.Match(stringXiraa, @"(\d\d\d\d)");
            return m.Groups[0].Value;
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

            if (list.Count > 0)
            {
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

            if (list.Count > 0)
            {
                var ids = list[0].ToBsonDocument()["customers"].AsBsonArray;
                return GetCollectionsByIds("Customers", "CustomerID", ids);
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

        public static double GetAccountOpeningValue(int accountId, string year)
        {
            var settings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
            JToken balaceSheet = new JObject();

            var coll = db.GetCollection<BsonDocument>("Accounts" + year);

            var filter = "{ AccountID: '" + accountId + "'}";

            var query = coll.FindSync(filter).ToList();

            if (query.Count > 0)
                return query[0].ToBsonDocument()["OpeningDebitBalance"].AsDouble - query[0].ToBsonDocument()["OpeningCreditBalance"].AsDouble;

            return 0;
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
                                                            { "_id", "null" }, 
                                                            { "total_credit", new BsonDocument("$sum", "$transaction.Lines.creditLine.CreditAmount")}
                                         });

            var list = aggregate.ToList();
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
                                                            { "_id", "null" }, 
                                                            { "total_debit", new BsonDocument("$sum", "$transaction.Lines.debitLine.DebitAmount")}
                                         });

            var list = aggregate.ToList();

            if (list.Count > 0)
            {
                return list[0].ToBsonDocument()["total_debit"].AsDouble;
            }

            return 0;

        }



        public static double GetAccountInRange(int account, string begin, string end)
        {
            string year = getYear(end);

            double initialAccount = GetAccountOpeningValue(account, year);
            double periodCredit = GetAccountCredit(account, begin, end);
            double periodDebit = GetAccountDebit(account, begin, end);

            return initialAccount + periodDebit - periodCredit;

        }



        private static JObject getRatioObject(string name, double value, string explanation)
        {
            JObject obj = new JObject();

            obj.Add("name", name);
            obj.Add("value", value);
            obj.Add("explanation", explanation);

            return obj;
        }

        public static string GetNetIncome(string begin, string end)
        {
            JObject netIncome = new JObject();



            double account71 = GetAccountInRange(71, begin, end);
            double account72 = GetAccountInRange(72, begin, end);
            double account73 = GetAccountInRange(73, begin, end);
            double account74 = GetAccountInRange(74, begin, end);
            double account75 = GetAccountInRange(75, begin, end);
            double account78 = GetAccountInRange(78, begin, end);
            double account79 = GetAccountInRange(79, begin, end);
            double account761 = GetAccountInRange(761, begin, end);
            double account762 = GetAccountInRange(762, begin, end);
            double account763 = GetAccountInRange(763, begin, end);
            double account61 = GetAccountInRange(61, begin, end);
            double account62 = GetAccountInRange(62, begin, end);
            double account63 = GetAccountInRange(63, begin, end);
            double account64 = GetAccountInRange(64, begin, end);
            double account65 = GetAccountInRange(65, begin, end);
            double account67 = GetAccountInRange(67, begin, end);
            double account68 = GetAccountInRange(68, begin, end);
            double account69 = GetAccountInRange(69, begin, end);
            double account812 = GetAccountInRange(812, begin, end);


            double netIncomeVal = Math.Abs(account71 + account72) + account75 + account73 + account74 - account61 - account62 -
                account63 + account762 - account65 + account763 - account67 + account78 - account68 + account761 - account64 + account79 - account69 + account812;

            netIncome.Add("name", "Net Income");
            netIncome.Add("value", netIncomeVal);

            return netIncome.ToString();

        }

        public static string GetFinancialRatios(string begin, string end)
        {
            JArray ratios = new JArray();

            //*************** Aux Values ********************
            //Income statement--------------- 
            double account71 = GetAccountInRange(71, begin, end);
            double account72 = GetAccountInRange(72, begin, end);
            double account73 = GetAccountInRange(73, begin, end);
            double account74 = GetAccountInRange(74, begin, end);
            double account75 = GetAccountInRange(75, begin, end);
            double account78 = GetAccountInRange(78, begin, end);
            double account79 = GetAccountInRange(79, begin, end);
            double account761 = GetAccountInRange(761, begin, end);
            double account762 = GetAccountInRange(762, begin, end);
            double account763 = GetAccountInRange(763, begin, end);
            double account61 = GetAccountInRange(61, begin, end);
            double account62 = GetAccountInRange(62, begin, end);
            double account63 = GetAccountInRange(63, begin, end);
            double account64 = GetAccountInRange(64, begin, end);
            double account65 = GetAccountInRange(65, begin, end);
            double account67 = GetAccountInRange(67, begin, end);
            double account68 = GetAccountInRange(68, begin, end);
            double account69 = GetAccountInRange(69, begin, end);
            double account812 = GetAccountInRange(812, begin, end);
            //-----------------------------------------

            // balance sheet-------------------------
            // non current assets
            double class4 = GetAccountInRange(41, begin, end) +
                            GetAccountInRange(42, begin, end) +
                            GetAccountInRange(43, begin, end) +
                            GetAccountInRange(44, begin, end) +
                            GetAccountInRange(45, begin, end) +
                            GetAccountInRange(46, begin, end);

            // current assets             

            double class3 = GetAccountInRange(32, begin, end) +
                            GetAccountInRange(33, begin, end) +
                            GetAccountInRange(34, begin, end) +
                            GetAccountInRange(35, begin, end) +
                            GetAccountInRange(36, begin, end) +
                            GetAccountInRange(39, begin, end);

            double account21 = GetAccountInRange(21, begin, end);
            double class1 = GetAccountInRange(11, begin, end) +
                            GetAccountInRange(12, begin, end) +
                            GetAccountInRange(13, begin, end) +
                            GetAccountInRange(14, begin, end);

            // non current liabilities
            double account25 = Math.Abs(GetAccountInRange(25, begin, end));


            // current liabilities
            double account22 = Math.Abs(GetAccountInRange(22, begin, end));
            double account24 = Math.Abs(GetAccountInRange(24, begin, end));
            double account26 = Math.Abs(GetAccountInRange(26, begin, end));


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
            double sales = Math.Abs(GetAccountInRange(71, begin, end));
            double accountsPayable = account22;
            double purchases = GetAccountInRange(31, begin, end);
            double cash = GetAccountInRange(11, begin, end);





            //------------------------------------------------


            //Analysis of return
            JObject analysisOfReturn = new JObject();

            analysisOfReturn.Add("name", "Analysis of return");

            JArray analysisOfReturnValues = new JArray();
            analysisOfReturnValues.Add(getRatioObject("Return on sales", (netEarnings / netSales) * 100, ""));
            analysisOfReturnValues.Add(getRatioObject("Return on Assets", (netEarnings / totalAssets) * 100, ""));
            analysisOfReturnValues.Add(getRatioObject("Return on Equity", (netEarnings / equity) * 100, ""));

            analysisOfReturn.Add("values", analysisOfReturnValues);

            //Efficiency

            JObject efficiency = new JObject();

            efficiency.Add("name", "Efficiency");

            JArray efficiencyValues = new JArray();
            efficiencyValues.Add(getRatioObject("Asset turnover", (sales / totalAssets) * 100, ""));
            efficiencyValues.Add(getRatioObject("Average inventory period", (inventory / costsOfGoodSold) * 365, ""));
            efficiencyValues.Add(getRatioObject("Inventory turnover", (costsOfGoodSold / inventory) * 100, ""));
            efficiencyValues.Add(getRatioObject("Average collection period", (accountsReceivable / sales) * 365, ""));
            efficiencyValues.Add(getRatioObject("Average payment period", (accountsPayable / purchases) * 365, ""));


            efficiency.Add("values", efficiencyValues);

            //Liquidity

            JObject liquidity = new JObject();

            liquidity.Add("name", "Liquidity");

            JArray liquidityValues = new JArray();
            liquidityValues.Add(getRatioObject("Current ratio", totalAssets / currentLiabilities, ""));
            liquidityValues.Add(getRatioObject("Quick ratio", (totalAssets - inventory) / currentLiabilities, ""));
            liquidityValues.Add(getRatioObject("Cash ratio", class1 / currentLiabilities, ""));
            liquidityValues.Add(getRatioObject("Working Capital", totalAssets - currentLiabilities, ""));

            liquidity.Add("values", liquidityValues);

            //Financial stability and Leverage

            JObject financialStabilityAndLeverage = new JObject();

            financialStabilityAndLeverage.Add("name", "Financial stability and Leverage");

            JArray financialStabilityAndLeverageValues = new JArray();
            financialStabilityAndLeverageValues.Add(getRatioObject("Equity to assets ratio", equity / totalAssets, ""));
            //financialStabilityAndLeverageValues.Add(getRatioObject("Debt to Equity", 0, ""));
            //financialStabilityAndLeverageValues.Add(getRatioObject("Coverage of fixed investments", 0, ""));
            financialStabilityAndLeverageValues.Add(getRatioObject("Interest Coverage", interest != 0 ? ebit / interest : 0, ""));

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

        public static string GetIncomeStatement(string begin, string end)
        {
            JArray obj = new JArray();

            double account71 = GetAccountInRange(71, begin, end);
            double account72 = GetAccountInRange(72, begin, end);
            double account73 = GetAccountInRange(73, begin, end);
            double account74 = GetAccountInRange(74, begin, end);
            double account75 = GetAccountInRange(75, begin, end);
            double account78 = GetAccountInRange(78, begin, end);
            double account79 = GetAccountInRange(79, begin, end);
            double account761 = GetAccountInRange(761, begin, end);
            double account762 = GetAccountInRange(762, begin, end);
            double account763 = GetAccountInRange(763, begin, end);
            double account61 = GetAccountInRange(61, begin, end);
            double account62 = GetAccountInRange(62, begin, end);
            double account63 = GetAccountInRange(63, begin, end);
            double account64 = GetAccountInRange(64, begin, end);
            double account65 = GetAccountInRange(65, begin, end);
            double account67 = GetAccountInRange(67, begin, end);
            double account68 = GetAccountInRange(68, begin, end);
            double account69 = GetAccountInRange(69, begin, end);
            double account812 = GetAccountInRange(812, begin, end);


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

        public static string GetBalanceSheet(string begin, string end)
        {
            JObject obj = new JObject();

            //ASSETS--------------------------------------------------------------------------------------------
            JObject assets = new JObject();

            //Non current assets----        

            // sub topics
            double class4 = GetAccountInRange(41, begin, end) +
                            GetAccountInRange(42, begin, end) +
                            GetAccountInRange(43, begin, end) +
                            GetAccountInRange(44, begin, end) +
                            GetAccountInRange(45, begin, end) +
                            GetAccountInRange(46, begin, end);

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
            double class3 = GetAccountInRange(32, begin, end) +
                            GetAccountInRange(33, begin, end) +
                            GetAccountInRange(34, begin, end) +
                            GetAccountInRange(35, begin, end) +
                            GetAccountInRange(36, begin, end) +
                            GetAccountInRange(39, begin, end);

            double account21 = GetAccountInRange(21, begin, end);

            double class1 = GetAccountInRange(11, begin, end) +
                            GetAccountInRange(12, begin, end) +
                            GetAccountInRange(13, begin, end) +
                            GetAccountInRange(14, begin, end);

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
            double account25 = Math.Abs(GetAccountInRange(25, begin, end));


            JArray nonCurrentLiabilitiesValues = new JArray();

            JObject funding = getFieldObject("Obtained funding (loans and others)", account25);

            nonCurrentLiabilitiesValues.Add(funding);

            // object
            JObject nonCurrentLiabilities = new JObject();
            nonCurrentLiabilities.Add("name", "Non Current Liabilities");
            nonCurrentLiabilities.Add("total", account25);
            nonCurrentLiabilities.Add("values", nonCurrentLiabilitiesValues);


            // current liabilities
            double account22 = Math.Abs(GetAccountInRange(22, begin, end));
            double account24 = Math.Abs(GetAccountInRange(24, begin, end));
            double account26 = Math.Abs(GetAccountInRange(26, begin, end));

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
        }


        public static string GetReceivableVSPayable(string arg1, string arg2)
        {
            string[] date1 = arg1.Split(new char[]{'-','/'});
            string[] date2 = arg2.Split(new char[]{'-','/'});
            
            int mounthStart = int.Parse(date1[1]);
            int mounthEnd = int.Parse(date2[1]);

            int yearStart = int.Parse(date1[0]);
            int yearEnd = int.Parse(date2[0]);

            JObject data = new JObject();

            JArray receivables = new JArray();
            JArray payables = new JArray();



            string beginDate = arg1;

            do
            {
                JObject payable = new JObject();
                JObject receivable = new JObject();


                mounthStart++;

                if(mounthStart > 12){
                    mounthStart = 1;
                    yearStart++;
                }

                string endDate = arg2;

                if(mounthStart != mounthEnd || yearStart != yearEnd)
                    endDate = String.Format("{0}-{1}-01", yearStart, mounthStart);

                receivable.Add("xasdas", beginDate);
                double accountValue = GetAccountInRange(21, beginDate, endDate); 
                receivable.Add("yasdsad", accountValue);

                payable.Add("x", "ola".ToJson());
                accountValue = GetAccountInRange(22, beginDate, endDate);
                payable.Add("y", accountValue);


                receivables.Add(receivable);
                payables.Add(payable);

            } while (mounthStart != mounthEnd || yearStart != yearEnd);

            data.Add("receivables", receivables);
            data.Add("payables", payables);

            return data.ToJson();

        }
    }
}
