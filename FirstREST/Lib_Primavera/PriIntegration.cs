using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Interop.ErpBS900;
using Interop.StdPlatBS900;
using Interop.StdBE900;
using Interop.GcpBE900;
using ADODB;

namespace FirstREST.Lib_Primavera
{
    public class PriIntegration
    {
        

        # region Cliente

        public static List<Model.Cliente> ListaClientes()
        {
            
            
            StdBELista objList;

            List<Model.Cliente> listClientes = new List<Model.Cliente>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                //objList = PriEngine.Engine.Comercial.Clientes.LstClientes();

                objList = PriEngine.Engine.Consulta("SELECT Cliente, Nome, Moeda, NumContrib as NumContribuinte, Fac_Mor AS campo_exemplo, CDU_Email as Email FROM  CLIENTES");

                //StdBECampos objeto = PriEngine.Engine.Comercial.Compras.ValidaCamposSAFT();
                //StdBECampos objeto = PriEngine.Engine.Comercial.Compras.;
                //PriEngine.Engine.Comercial.TabCompras.LstTodosDocCompras();
                //PriEngine.Engine.Comercial.TabCompras.LstDocCompras();
                //PriEngine.Engine.Comercial.Fornecedores.
                //PriEngine.Engine.Comercial.Stocks.DaCamposUtil
                //PriEngine.Engine.Comercial.ArtigosFornecedores.DaPrecoFornecedor();
                //PriEngine.Engine.Comercial.TabCompras.LstDocCompras();
                //PriEngine.Engine.Comercial.TabStocks.LstDocStocks();
                //PriEngine.Engine.Comercial.Vendedores.LstVendedores();
                //GcpBEFornecedor fornecedor = PriEngine.Engine.Comercial.Fornecedores.Consulta("sdioadioausd");

                while (!objList.NoFim())
                {
                    listClientes.Add(new Model.Cliente
                    {
                        CodCliente = objList.Valor("Cliente"),
                        NomeCliente = objList.Valor("Nome"),
                        Moeda = objList.Valor("Moeda"),
                        NumContribuinte = objList.Valor("NumContribuinte"),
                        Morada = objList.Valor("campo_exemplo"),
                        Email = objList.Valor("Email")
                    });
                    objList.Seguinte();

                }

                return listClientes;
            }
            else
                return null;
        }

        public static Lib_Primavera.Model.Cliente GetCliente(string codCliente)
        {
            

            GcpBECliente objCli = new GcpBECliente();


            Model.Cliente myCli = new Model.Cliente();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == true)
                {
                    
                    objCli = PriEngine.Engine.Comercial.Clientes.Edita(codCliente);
                    myCli.CodCliente = objCli.get_Cliente();
                    myCli.NomeCliente = objCli.get_Nome();
                    myCli.Moeda = objCli.get_Moeda();
                    myCli.NumContribuinte = objCli.get_NumContribuinte();
                    myCli.Morada = objCli.get_Morada();
                    myCli.Email = PriEngine.Engine.Comercial.Clientes.DaValorAtributo(codCliente, "CDU_Email");
                    
                    return myCli;
                }
                else
                {
                    return null;
                }
            }
            else
                return null;
        }

        public static Lib_Primavera.Model.RespostaErro UpdCliente(Lib_Primavera.Model.Cliente cliente)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
           

            GcpBECliente objCli = new GcpBECliente();

            try
            {

                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {

                    if (PriEngine.Engine.Comercial.Clientes.Existe(cliente.CodCliente) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O cliente não existe";
                        return erro;
                    }
                    else
                    {

                        objCli = PriEngine.Engine.Comercial.Clientes.Edita(cliente.CodCliente);
                        objCli.set_EmModoEdicao(true);

                        objCli.set_Nome(cliente.NomeCliente);
                        objCli.set_NumContribuinte(cliente.NumContribuinte);
                        objCli.set_Moeda(cliente.Moeda);
                        objCli.set_Morada(cliente.Morada);
                        
                        PriEngine.Engine.Comercial.Clientes.Actualiza(objCli);

                        erro.Erro = 0;
                        erro.Descricao = "Sucesso";
                        return erro;
                    }
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir a empresa";
                    return erro;

                }

            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }


        public static Lib_Primavera.Model.RespostaErro DelCliente(string codCliente)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBECliente objCli = new GcpBECliente();


            try
            {

                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    if (PriEngine.Engine.Comercial.Clientes.Existe(codCliente) == false)
                    {
                        erro.Erro = 1;
                        erro.Descricao = "O cliente não existe";
                        return erro;
                    }
                    else
                    {

                        PriEngine.Engine.Comercial.Clientes.Remove(codCliente);
                        erro.Erro = 0;
                        erro.Descricao = "Sucesso";
                        return erro;
                    }
                }

                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir a empresa";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }

        }



        public static Lib_Primavera.Model.RespostaErro InsereClienteObj(Model.Cliente cli)
        {

            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            

            GcpBECliente myCli = new GcpBECliente();

            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {

                    myCli.set_Cliente(cli.CodCliente);
                    myCli.set_Nome(cli.NomeCliente);
                    myCli.set_NumContribuinte(cli.NumContribuinte);
                    myCli.set_Moeda(cli.Moeda);
                    myCli.set_Morada(cli.Morada);

                    PriEngine.Engine.Comercial.Clientes.Actualiza(myCli);

                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;
                }
            }

            catch (Exception ex)
            {
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }


        }

       

        #endregion Cliente;   // -----------------------------  END   CLIENTE    -----------------------


        #region Artigo

        public static Lib_Primavera.Model.Artigo GetArtigo(string codArtigo)
        {
            
            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.Artigo myArt = new Model.Artigo();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Artigos.Existe(codArtigo) == false)
                {
                    return null;
                }
                else
                {
                    objArtigo = PriEngine.Engine.Comercial.Artigos.Edita(codArtigo);
                    myArt.CodArtigo = objArtigo.get_Artigo();
                    myArt.DescArtigo = objArtigo.get_Descricao();
                    myArt.STKAtual = objArtigo.get_StkActual(); 

                    return myArt;
                }
                
            }
            else
            {
                return null;
            }
        }

        public static List<Model.Artigo> ListaArtigos()
        {
                        
            StdBELista objList;

            Model.Artigo art = new Model.Artigo();
            List<Model.Artigo> listArts = new List<Model.Artigo>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();
                while (!objList.NoFim())
                {
                    art = new Model.Artigo();
                    art.CodArtigo = objList.Valor("artigo");
                    art.DescArtigo = objList.Valor("descricao");
  //                  art.STKAtual = objList.Valor("stkatual");
                  
                    
                    listArts.Add(art);
                    objList.Seguinte();
                }

                return listArts;

            }
            else
            {
                return null;

            }

        }

        #endregion Artigo

        #region Fornecedor

        public static List<Model.Artigo> GetArtigos_Fornecedor(string idFornecedor)
        {

            StdBELista objListCab;

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                List<Model.Artigo> result = new List<Model.Artigo>();

                string query = String.Format(
                    "SELECT Artigo, Descricao From Artigo where Fornecedor = '{0}'", idFornecedor);

                objListCab = PriEngine.Engine.Consulta(query);

                while (!objListCab.NoFim())
                {
                    Model.Artigo fc = new Model.Artigo();
                    fc.CodArtigo = objListCab.Valor("Artigo");
                    fc.DescArtigo = objListCab.Valor("Descricao");
                    result.Add(fc);
                    objListCab.Seguinte();
                }

                return result;
            }

            return null;
        }

        public static List<Model.Fornecedor> ListaFornecedores()
        {
            StdBELista objListCab;

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                List<Model.Fornecedor> result = new List<Model.Fornecedor>();

                string query = String.Format(
                    "SELECT Fornecedor, Nome, NumContrib, Tel, NomeFiscal From Fornecedores");

                objListCab = PriEngine.Engine.Consulta(query);

                while (!objListCab.NoFim())
                {
                    Model.Fornecedor fc = new Model.Fornecedor();
                    fc.CodFornecedor = objListCab.Valor("Fornecedor");
                    fc.NomeFiscal = objListCab.Valor("NomeFiscal");
                    fc.NomeFornecedor = objListCab.Valor("Nome");
                    fc.Telefone = objListCab.Valor("Tel");
                    fc.NumContribuinte = objListCab.Valor("NumContrib");
                    result.Add(fc);
                    objListCab.Seguinte();
                }

                return result;
            }

            return null;
            

        }

        public static Model.Fornecedor GetFornecedor(string id)
        {
            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                Model.Fornecedor nf = new Model.Fornecedor();

                nf.Telefone = PriEngine.Engine.Comercial.Fornecedores.DaValorAtributo(id,"Tel");
                nf.NomeFornecedor = PriEngine.Engine.Comercial.Fornecedores.DaValorAtributo(id, "Nome");
                nf.NomeFiscal = PriEngine.Engine.Comercial.Fornecedores.DaValorAtributo(id, "NomeFiscal");
                nf.NumContribuinte = PriEngine.Engine.Comercial.Fornecedores.DaValorAtributo(id, "NumContrib");
                nf.CodFornecedor = id;

                return nf;
            }
            else
            {
                return null;
            }
        }

        #endregion Fornecedor

        #region Inventory
        public static Lib_Primavera.Model.Inventory GetInventory(string codArtigo)
        {

            GcpBEArtigo objArtigo = new GcpBEArtigo();
            Model.Inventory myArt = new Model.Inventory();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                if (PriEngine.Engine.Comercial.Artigos.Existe(codArtigo) == false)
                {
                    return null;
                }
                else
                {
                    objArtigo = PriEngine.Engine.Comercial.Artigos.Edita(codArtigo);
                    myArt.CodArtigo = objArtigo.get_Artigo();
                    myArt.Description = objArtigo.get_Descricao();
                    myArt.Stock = objArtigo.get_StkActual();

                    return myArt;
                }

            }
            else
            {
                return null;
            }

        }

        public static List<Model.Inventory> ListInventory()
        {

            StdBELista objList;

            Model.Inventory art = new Model.Inventory();
            List<Model.Inventory> listInventory = new List<Model.Inventory>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                //objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();
                objList = PriEngine.Engine.Consulta("SELECT Artigo.Artigo, Artigo.Descricao, Artigo.STKActual FROM Artigo");

                while (!objList.NoFim())
                {
                    art = new Model.Inventory();
                    art.CodArtigo = objList.Valor("artigo");
                    art.Description = objList.Valor("descricao");
                    art.Stock = objList.Valor("stkactual");


                    listInventory.Add(art);
                    objList.Seguinte();
                }

                return listInventory;

            }
            else
            {
                return null;

            }

        }

        public static List<Model.ArtStock> ListInventoryByDate(string date)
        {
            StdBELista objList;

            Model.ArtStock art = null;
            List<Model.ArtStock> listArts = new List<Model.ArtStock>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();



                DateTime dateObj;
                
                DateTime.TryParse(date, out dateObj);

                StdBELista armsList = PriEngine.Engine.Comercial.Armazens.LstArmazens();
            

                string arms = "";

                while (!armsList.NoFim())
                {
                    arms += "[" + armsList.Valor("Armazem") + "]";
                    armsList.Seguinte();
                }

                string table = "tempdb.dbo.##RecalculoStk";
                string error = "";

                short res = PriEngine.Engine.Comercial.Stocks.RecalculoStocks(
                    enumTipoRecalculoCusteio.trcRecalculoData,
                    strExtArms: arms, 
                    dtData: dateObj, 
                    blnArtNecRecalcPCM: false,
                    blnRecalcQtdReservada: false,
                    blnExtRecalculo: false);

                string query = @"
                    SELECT DISTINCT
                        Artigo.Familia Familia,
                        Familias.Descricao NomeFamilia,
                        Artigo.Artigo Artigo,
                        Artigo.Descricao Descricao,
                        Artigo.UnidadeBase UnidadeBase,
                        Recalculo.PCMedio PCMedio,
                        Artigo.SubFamilia SubFamilia,
                        Artigo.STKActual STKActual,
                        Recalculo.QuantidadeArm Actual,
                        Artigo.TratamentoDim,
                        Recalculo.Artigo rArtigo
                    FROM (Artigo Artigo LEFT OUTER JOIN tempdb.dbo.##RecalculoStk Recalculo ON Artigo.Artigo=Recalculo.Artigo)
                        LEFT OUTER JOIN Familias Familias ON Artigo.Familia=Familias.Familia
                    WHERE Artigo.TratamentoDIM<>1
                    ORDER BY Artigo.Artigo, Artigo.SubFamilia";

                objList = PriEngine.Engine.Consulta(query);

                while (!objList.NoFim())
                {
                    art = new Model.ArtStock();
                    art.ProductID = objList.Valor("Artigo");
                    art.ProductDesc = objList.Valor("Descricao");
                    Double.TryParse(objList.Valor("Actual") != null ? objList.Valor("Actual") : "0.0", out art.ActualSTK);
                    art.Family = objList.Valor("NomeFamilia");
                    art.SubFamily = objList.Valor("SubFamilia");
                    Double.TryParse(objList.Valor("PCMedio") != null ? objList.Valor("PCMedio") : "0.0", out art.PCM);
                    art.TotalValue = art.PCM * art.ActualSTK;
                    art.ProductID = objList.Valor("PCMedio");

                    listArts.Add(art);
                    objList.Seguinte();
                }
                return listArts;
            }
            else
                return null;
        }

        public static List<Model.Inventory> ListInventoryByDate(string begin, string end)
        {
            StdBELista objList;

            Model.Inventory art = null;
            List<Model.Inventory> listArts = new List<Model.Inventory>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                objList = PriEngine.Engine.Comercial.Artigos.LstArtigos();

                string query = String.Format(
                    "SELECT Artigo, EntradaSaida, Unidade , SUM(Quantidade) as Quantidade , SUM(Quantidade * PrecUnit - DescontoComercial + DespesaAdicionalCompra) as Valor From LinhasSTK where NOT TipoDoc = 'GR' AND NOT TipoDoc='SOF' AND Artigo IS NOT NULL AND Data between '{0}' and '{1}' group by Artigo, Unidade, EntradaSaida",
                    begin, end);

                
                objList = PriEngine.Engine.Consulta(query);

                while (!objList.NoFim())
                {
                    art = new Model.Inventory();
                    art.CodArtigo = objList.Valor("Artigo");
                    art.Stock = objList.Valor("Quantidade");
                    art.InOut = objList.Valor("EntradaSaida");
                    art.Value = objList.Valor("Valor");
                    listArts.Add(art);
                    objList.Seguinte();
                }
                return listArts;
            }
            else
                return null;
        }

        #endregion Inventory


        #region DocCompra

        public static List<Model.DocCompra> ListPurchasesInvoices(string begin, string end)
        {
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocCompra dc = new Model.DocCompra();
            List<Model.DocCompra> listdc = new List<Model.DocCompra>();
            Model.LinhaDocCompra lindc = new Model.LinhaDocCompra();
            List<Model.LinhaDocCompra> listlindc = new List<Model.LinhaDocCompra>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                string query = String.Format(
                    "SELECT id, NumDocExterno, Entidade, DataDoc, NumDoc, TotalMerc, Serie, TipoDoc From CabecCompras where (TipoDoc='VFA' or TipoDoc='VNC') and (DataDoc between '{0}' and '{1}' )",
                     begin, end);

                //objListCab = PriEngine.Engine.Comercial.TabCompras.LstDocCompras();
                objListCab = PriEngine.Engine.Consulta(query);
                while (!objListCab.NoFim())
                {
                    dc = new Model.DocCompra();
                    dc.id = objListCab.Valor("id");
                    dc.NumDocExterno = objListCab.Valor("NumDocExterno");
                    dc.Entidade = objListCab.Valor("Entidade");
                    dc.NumDoc = objListCab.Valor("NumDoc");
                    dc.Data = objListCab.Valor("DataDoc");
                    dc.TotalMerc = objListCab.Valor("TotalMerc");
                    dc.Serie = objListCab.Valor("Serie");
                    dc.TipoDoc = objListCab.Valor("TipoDoc");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecCompras, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido, Armazem, Lote from LinhasCompras where IdCabecCompras='" + dc.id + "' order By NumLinha");
                    listlindc = new List<Model.LinhaDocCompra>();

                    while (!objListLin.NoFim())
                    {
                        lindc = new Model.LinhaDocCompra();
                        lindc.IdCabecDoc = objListLin.Valor("idCabecCompras");
                        lindc.CodArtigo = objListLin.Valor("Artigo");
                        lindc.DescArtigo = objListLin.Valor("Descricao");
                        lindc.Quantidade = objListLin.Valor("Quantidade");
                        lindc.Unidade = objListLin.Valor("Unidade");
                        lindc.Desconto = objListLin.Valor("Desconto1");
                        lindc.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindc.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindc.TotalLiquido = objListLin.Valor("PrecoLiquido");
                        lindc.Armazem = objListLin.Valor("Armazem");
                        lindc.Lote = objListLin.Valor("Lote");

                        listlindc.Add(lindc);
                        objListLin.Seguinte();
                    }

                    dc.LinhasDoc = listlindc;
                    
                    listdc.Add(dc);
                    objListCab.Seguinte();
                }
            }
            return listdc;
        }

                
        public static Model.RespostaErro VGR_New(Model.DocCompra dc)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            

            GcpBEDocumentoCompra myGR = new GcpBEDocumentoCompra();
            GcpBELinhaDocumentoCompra myLin = new GcpBELinhaDocumentoCompra();
            GcpBELinhasDocumentoCompra myLinhas = new GcpBELinhasDocumentoCompra();

            PreencheRelacaoCompras rl = new PreencheRelacaoCompras();
            List<Model.LinhaDocCompra> lstlindv = new List<Model.LinhaDocCompra>();

            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myGR.set_Entidade(dc.Entidade);
                    myGR.set_NumDocExterno(dc.NumDocExterno);
                    myGR.set_Serie(dc.Serie);
                    myGR.set_Tipodoc("VGR");
                    myGR.set_TipoEntidade("F");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dc.LinhasDoc;
                    //PriEngine.Engine.Comercial.Compras.PreencheDadosRelacionados(myGR,rl);
                    PriEngine.Engine.Comercial.Compras.PreencheDadosRelacionados(myGR);
                    foreach (Model.LinhaDocCompra lin in lstlindv)
                    {
                        PriEngine.Engine.Comercial.Compras.AdicionaLinha(myGR, lin.CodArtigo, lin.Quantidade, lin.Armazem, "", lin.PrecoUnitario, lin.Desconto);
                    }


                    PriEngine.Engine.IniciaTransaccao();
                    PriEngine.Engine.Comercial.Compras.Actualiza(myGR, "Teste");
                    PriEngine.Engine.TerminaTransaccao();
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;

                }

            }
            catch (Exception ex)
            {
                PriEngine.Engine.DesfazTransaccao();
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }


        #endregion DocCompra


        #region DocsVenda

        public static Model.RespostaErro Encomendas_New(Model.DocVenda dv)
        {
            Lib_Primavera.Model.RespostaErro erro = new Model.RespostaErro();
            GcpBEDocumentoVenda myEnc = new GcpBEDocumentoVenda();
             
            GcpBELinhaDocumentoVenda myLin = new GcpBELinhaDocumentoVenda();

            GcpBELinhasDocumentoVenda myLinhas = new GcpBELinhasDocumentoVenda();
             
            PreencheRelacaoVendas rl = new PreencheRelacaoVendas();
            List<Model.LinhaDocVenda> lstlindv = new List<Model.LinhaDocVenda>();
            
            try
            {
                if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
                {
                    // Atribui valores ao cabecalho do doc
                    //myEnc.set_DataDoc(dv.Data);
                    myEnc.set_Entidade(dv.Entidade);
                    myEnc.set_Serie(dv.Serie);
                    myEnc.set_Tipodoc("ECL");
                    myEnc.set_TipoEntidade("C");
                    // Linhas do documento para a lista de linhas
                    lstlindv = dv.LinhasDoc;
                    //PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc, rl);
                    PriEngine.Engine.Comercial.Vendas.PreencheDadosRelacionados(myEnc);
                    foreach (Model.LinhaDocVenda lin in lstlindv)
                    {
                        PriEngine.Engine.Comercial.Vendas.AdicionaLinha(myEnc, lin.CodArtigo, lin.Quantidade, "", "", lin.PrecoUnitario, lin.Desconto);
                    }


                   // PriEngine.Engine.Comercial.Compras.TransformaDocumento(

                    PriEngine.Engine.IniciaTransaccao();
                    //PriEngine.Engine.Comercial.Vendas.Edita Actualiza(myEnc, "Teste");
                    PriEngine.Engine.Comercial.Vendas.Actualiza(myEnc, "Teste");
                    PriEngine.Engine.TerminaTransaccao();
                    erro.Erro = 0;
                    erro.Descricao = "Sucesso";
                    return erro;
                }
                else
                {
                    erro.Erro = 1;
                    erro.Descricao = "Erro ao abrir empresa";
                    return erro;

                }

            }
            catch (Exception ex)
            {
                PriEngine.Engine.DesfazTransaccao();
                erro.Erro = 1;
                erro.Descricao = ex.Message;
                return erro;
            }
        }

     

        public static List<Model.DocVenda> Encomendas_List(string begin, string end)
        {
            
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            List<Model.DocVenda> listdv = new List<Model.DocVenda>();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new
            List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                string query = String.Format(
                   "SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL' and (Data between '{0}' and '{1}' )",
                     begin, end);

                objListCab = PriEngine.Engine.Consulta(query);
                
                while (!objListCab.NoFim())
                {
                    dv = new Model.DocVenda();
                    dv.id = objListCab.Valor("id");
                    dv.Entidade = objListCab.Valor("Entidade");
                    dv.NumDoc = objListCab.Valor("NumDoc");
                    dv.Data = objListCab.Valor("Data");
                    dv.TotalMerc = objListCab.Valor("TotalMerc");
                    dv.Serie = objListCab.Valor("Serie");
                    objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                    listlindv = new List<Model.LinhaDocVenda>();

                    while (!objListLin.NoFim())
                    {
                        lindv = new Model.LinhaDocVenda();
                        lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                        lindv.CodArtigo = objListLin.Valor("Artigo");
                        lindv.DescArtigo = objListLin.Valor("Descricao");
                        lindv.Quantidade = objListLin.Valor("Quantidade");
                        lindv.Unidade = objListLin.Valor("Unidade");
                        lindv.Desconto = objListLin.Valor("Desconto1");
                        lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                        lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                        lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");

                        listlindv.Add(lindv);
                        objListLin.Seguinte();
                    }

                    dv.LinhasDoc = listlindv;
                    listdv.Add(dv);
                    objListCab.Seguinte();
                }
            }
            return listdv;
        }


       

        public static Model.DocVenda Encomenda_Get(string numdoc)
        {
            
            
            StdBELista objListCab;
            StdBELista objListLin;
            Model.DocVenda dv = new Model.DocVenda();
            Model.LinhaDocVenda lindv = new Model.LinhaDocVenda();
            List<Model.LinhaDocVenda> listlindv = new List<Model.LinhaDocVenda>();

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                

                string st = "SELECT id, Entidade, Data, NumDoc, TotalMerc, Serie From CabecDoc where TipoDoc='ECL' and NumDoc='" + numdoc + "'";
                objListCab = PriEngine.Engine.Consulta(st);
                dv = new Model.DocVenda();
                dv.id = objListCab.Valor("id");
                dv.Entidade = objListCab.Valor("Entidade");
                dv.NumDoc = objListCab.Valor("NumDoc");
                dv.Data = objListCab.Valor("Data");
                dv.TotalMerc = objListCab.Valor("TotalMerc");
                dv.Serie = objListCab.Valor("Serie");
                objListLin = PriEngine.Engine.Consulta("SELECT idCabecDoc, Artigo, Descricao, Quantidade, Unidade, PrecUnit, Desconto1, TotalILiquido, PrecoLiquido from LinhasDoc where IdCabecDoc='" + dv.id + "' order By NumLinha");
                listlindv = new List<Model.LinhaDocVenda>();

                while (!objListLin.NoFim())
                {
                    lindv = new Model.LinhaDocVenda();
                    lindv.IdCabecDoc = objListLin.Valor("idCabecDoc");
                    lindv.CodArtigo = objListLin.Valor("Artigo");
                    lindv.DescArtigo = objListLin.Valor("Descricao");
                    lindv.Quantidade = objListLin.Valor("Quantidade");
                    lindv.Unidade = objListLin.Valor("Unidade");
                    lindv.Desconto = objListLin.Valor("Desconto1");
                    lindv.PrecoUnitario = objListLin.Valor("PrecUnit");
                    lindv.TotalILiquido = objListLin.Valor("TotalILiquido");
                    lindv.TotalLiquido = objListLin.Valor("PrecoLiquido");
                    listlindv.Add(lindv);
                    objListLin.Seguinte();
                }

                dv.LinhasDoc = listlindv;
                return dv;
            }
            return null;
        }

        #endregion DocsVenda

        #region Compras

        public static double getDatedPurchases(string begin, string end)
        {

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {

                string query = String.Format("SELECT SUM(CabecCompras.TotalMerc) - SUM(CabecCompras.TotalDesc) as total FROM CabecCompras WHERE (CabecCompras.DataDoc between '{0}' and '{1}' ) and not (TipoDoc = 'COT' or TipoDoc = 'PCO' or TipoDoc='VFS' or TipoDoc='VGT' or TipoDoc='ECF')", begin, end);

                return (double)PriEngine.Engine.Consulta(query).Valor("total");
            }

            return 0;
        }

        public static double getDatedPurchasesByFornecedor(string begin, string end, string idF)
        {

            if (PriEngine.InitializeCompany(FirstREST.Properties.Settings.Default.Company.Trim(), FirstREST.Properties.Settings.Default.User.Trim(), FirstREST.Properties.Settings.Default.Password.Trim()) == true)
            {
                string query = String.Format("SELECT SUM(CabecCompras.TotalMerc) - SUM(CabecCompras.TotalDesc) as total FROM CabecCompras WHERE (CabecCompras.DataDoc between '{0}' and '{1}' ) and not (TipoDoc = 'COT' or TipoDoc = 'PCO' or TipoDoc='VFS' or TipoDoc='VGT' or TipoDoc='ECF') and Entidade = '{2}'", begin, end,idF);

                return (double)PriEngine.Engine.Consulta(query).Valor("total");

            }

            return 0;
        }

        #endregion Compras
    }
}