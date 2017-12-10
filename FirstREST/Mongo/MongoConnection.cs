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


        private static JObject getRatioObject(string name, double value, string explanation)
        {
            JObject obj = new JObject();

            obj.Add("name", name);
            obj.Add("value", value);
            obj.Add("explanation", explanation);

            return obj;
        }
       

        public static string GetFinancialRatios()
        {
            JArray ratios = new JArray();

            //*************** Aux Values ********************
            //Income statement--------------- 
            double account71 = GetAccountValue(71);
            double account72 = GetAccountValue(72);
            double account73 = GetAccountValue(73);
            double account74 = GetAccountValue(74);
            double account75 = GetAccountValue(75);
            double account78 = GetAccountValue(78);
            double account79 = GetAccountValue(79);
            double account761 = GetAccountValue(761);
            double account762 = GetAccountValue(762);
            double account763 = GetAccountValue(763);         
            double account61 = GetAccountValue(61);
            double account62 = GetAccountValue(62);
            double account63 = GetAccountValue(63);
            double account64 = GetAccountValue(64);
            double account65 = GetAccountValue(65);
            double account67 = GetAccountValue(67);
            double account68 = GetAccountValue(68);
            double account69 = GetAccountValue(69);
            double account812 = GetAccountValue(812);
            //-----------------------------------------
            
            // balance sheet-------------------------
            // non current assets
            double class4 = GetAccountValue(41) +
                            GetAccountValue(42) +
                            GetAccountValue(43) +
                            GetAccountValue(44) +
                            GetAccountValue(45) +
                            GetAccountValue(46);

            // current assets             
           
            double class3 = GetAccountValue(32) +
                            GetAccountValue(33) +
                            GetAccountValue(34) +
                            GetAccountValue(35) +
                            GetAccountValue(36) +
                            GetAccountValue(39);

            double account21 = GetAccountValue(21);
            double class1 = GetAccountValue(11) +
                            GetAccountValue(12) +
                            GetAccountValue(13) +
                            GetAccountValue(14);

            // non current liabilities
            double account25 = Math.Abs(GetAccountValue(25));


            // current liabilities
            double account22 = Math.Abs(GetAccountValue(22));
            double account24 = Math.Abs(GetAccountValue(24));
            double account26 = Math.Abs(GetAccountValue(26));

          
            //----------------------------------
            
            
            double netSales = Math.Abs(account71 + account72) + account75 + account73 + account74 - account61 - account62 - 
                account63 + account762 - account65 + account763 - account67 + account78 - account68;
            double netEarnings = netSales + account812 + account79 - account69 + account761 - account64;
            double totalAssets = class4 + class3 + account21 + class1;
            double currentLiabilities = account22 + account24 + account26;
            double nonCurrentLiabilities = account25;
            double equity = totalAssets - currentLiabilities - nonCurrentLiabilities;            
            double inventory = class3;
            double costsOfGoodSold = account61;
            double accountsReceivable = account21;
            double sales = GetAccountValue(71);
            double accountsPayable = account22;
            double purchases = GetAccountValue(31);





            //------------------------------------------------


            //Analysis of return
            JObject analysisOfReturn = new JObject();

            analysisOfReturn.Add("name", "Analysis of return");
            
            JArray analysisOfReturnValues = new JArray();
            analysisOfReturnValues.Add(getRatioObject("Return on sales", netEarnings/netSales, ""));
            analysisOfReturnValues.Add(getRatioObject("Return on Assets", 0, ""));
            analysisOfReturnValues.Add(getRatioObject("Return on Equity", 0, ""));

            analysisOfReturn.Add("values", analysisOfReturnValues);

            //Efficiency

            JObject efficiency = new JObject();

            efficiency.Add("name", "Efficiency");

            JArray efficiencyValues = new JArray();
            efficiencyValues.Add(getRatioObject("Asset turnover", 0, ""));
            efficiencyValues.Add(getRatioObject("Average inventory period", 0, ""));
            efficiencyValues.Add(getRatioObject("Inventory turnover", 0, ""));
            efficiencyValues.Add(getRatioObject("Average collection period", 0, ""));
            efficiencyValues.Add(getRatioObject("Average payment period", 0, ""));


            efficiency.Add("values", efficiencyValues);

            //Liquidity

            JObject liquidity = new JObject();

            liquidity.Add("name", "Liquidity");

            JArray liquidityValues = new JArray();
            liquidityValues.Add(getRatioObject("Current ratio", 0, ""));
            liquidityValues.Add(getRatioObject("Quick ratio (acid test)", 0, ""));
            liquidityValues.Add(getRatioObject("Cash ratio", 0, ""));
            liquidityValues.Add(getRatioObject("Working Capital", 0, ""));

            liquidity.Add("values", liquidityValues);

            //Financial stability and Leverage

            JObject financialStabilityAndLeverage = new JObject();

            financialStabilityAndLeverage.Add("name", "Financial stability and Leverage");

            JArray financialStabilityAndLeverageValues = new JArray();
            financialStabilityAndLeverageValues.Add(getRatioObject("Equity to assets ratio", 0, ""));
            financialStabilityAndLeverageValues.Add(getRatioObject("Debt to Equity", 0, ""));
            financialStabilityAndLeverageValues.Add(getRatioObject("Coverage of fixed investments", 0, ""));
            financialStabilityAndLeverageValues.Add(getRatioObject("Interest Coverage", 0, ""));

            financialStabilityAndLeverage.Add("values", financialStabilityAndLeverageValues);

            //Growth ratios

            JObject growthRatios = new JObject();

            growthRatios.Add("name", "Analysis of return");

            JArray growthRatiosValues = new JArray();
            growthRatiosValues.Add(getRatioObject("Sales", 0, ""));
            growthRatiosValues.Add(getRatioObject("Profit", 0, ""));
            growthRatiosValues.Add(getRatioObject("Assets", 0, ""));
            growthRatiosValues.Add(getRatioObject("Inventory", 0, ""));

            growthRatios.Add("values", growthRatiosValues);


            //--------------------- Ratios object ------------------
            ratios.Add(analysisOfReturn);
            ratios.Add(efficiency);
            ratios.Add(liquidity);
            ratios.Add(financialStabilityAndLeverage);
            ratios.Add(growthRatios);
            

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

            double account71 = GetAccountValue(71);
            double account72 = GetAccountValue(72);
            double account73 = GetAccountValue(73);
            double account74 = GetAccountValue(74);
            double account75 = GetAccountValue(75);
            double account78 = GetAccountValue(78);
            double account79 = GetAccountValue(79);
            double account761 = GetAccountValue(761);
            double account762 = GetAccountValue(762);
            double account763 = GetAccountValue(763);         
            double account61 = GetAccountValue(61);
            double account62 = GetAccountValue(62);
            double account63 = GetAccountValue(63);
            double account64 = GetAccountValue(64);
            double account65 = GetAccountValue(65);
            double account67 = GetAccountValue(67);
            double account68 = GetAccountValue(68);
            double account69 = GetAccountValue(69);
            double account812 = GetAccountValue(812);


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
            double class4 = GetAccountValue(41) +
                            GetAccountValue(42) +
                            GetAccountValue(43) +
                            GetAccountValue(44) +
                            GetAccountValue(45) +
                            GetAccountValue(46);

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
            double class3 = GetAccountValue(32) +
                            GetAccountValue(33) +
                            GetAccountValue(34) +
                            GetAccountValue(35) +
                            GetAccountValue(36) +
                            GetAccountValue(39);

            double account21 = GetAccountValue(21);

            double class1 = GetAccountValue(11) +
                            GetAccountValue(12) +
                            GetAccountValue(13) +
                            GetAccountValue(14);
           
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
            double account25 = Math.Abs(GetAccountValue(25));


            JArray nonCurrentLiabilitiesValues = new JArray();

            JObject funding = getFieldObject("Obtained funding (loans and others)", account25);

            nonCurrentLiabilitiesValues.Add(funding);

            // object
            JObject nonCurrentLiabilities = new JObject();
            nonCurrentLiabilities.Add("name", "Non Current Liabilities");
            nonCurrentLiabilities.Add("total", account25);
            nonCurrentLiabilities.Add("values", nonCurrentLiabilitiesValues);


            // current liabilities
            double account22 = Math.Abs(GetAccountValue(22));
            double account24 = Math.Abs(GetAccountValue(24));
            double account26 = Math.Abs(GetAccountValue(26));

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
            
            */
        }

    }
}