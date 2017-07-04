using app_pesquisa_analise.dao;
using app_pesquisa_analise.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.util
{
    public class DadosPesquisaUtil
    {
        private DAO_Pesquisa06 dao06;
        private DAO_Pesquisa01 dao01;
        private DAO_Pesquisa04 dao04;
        private DAO_Pesquisa02 dao02;
        private DAO_Pesquisa03 dao03;
        private DAO_Pesquisa07 dao07;
        private DAO_Filtro daoFiltro;

        private List<CE_Pesquisa06> listPesquisas;
        private List<CE_Pesquisa04> listPerguntas;

        private WSUtil ws;

        public DadosPesquisaUtil()
        {
            dao01 = DAO_Pesquisa01.Instance;
            dao06 = DAO_Pesquisa06.Instance;
            dao02 = DAO_Pesquisa02.Instance;
            dao03 = DAO_Pesquisa03.Instance;
            dao04 = DAO_Pesquisa04.Instance;
            daoFiltro = DAO_Filtro.Instance;
            
            listPesquisas = new List<CE_Pesquisa06>();
            listPerguntas = new List<CE_Pesquisa04>();

            ws = WSUtil.Instance;
        }

        public async Task ObterDadosPerguntas(Int32 idpesquisa01)
        {
            JObject obj = new JObject();
            obj["idpesquisa01"] = idpesquisa01;
            obj["data"] = String.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now);

            HttpResponseMessage resposta = await ws.Post("obterItensFormularioAnalise", obj);

            String message = await resposta.Content.ReadAsStringAsync();

            if (resposta.IsSuccessStatusCode)
            {
                ListPesquisa04 listPesquisa04 = JsonConvert.DeserializeObject<ListPesquisa04>(message);

                this.listPerguntas.AddRange(listPesquisa04.itensFormulario);

                listPesquisa04 = null;
            }
            else
            {
                throw new Exception(message);
            }

        }

        public async Task DownloadRespostas(Int32 idpesquisa06, Int32 idpesquisa01)
        {
            try
            {
                JObject obj = new JObject();
                obj["idpesquisa06"] = idpesquisa06;

                List<CE_Filtro> filtros = daoFiltro.ObterFiltrosPorPesquisa(idpesquisa01);

                var groupPergunta = filtros.GroupBy(o => o.idpesquisa04);

                JArray jFiltros = new JArray();

                foreach (var item in groupPergunta)
                {
                    List<CE_Filtro> filtrosPergunta = filtros.Where(o => o.idpesquisa04 == item.Key).ToList();

                    JObject objPergunta = new JObject();
                    objPergunta["idpesquisa04"] = item.Key;
                    objPergunta["temopcoes"] = filtrosPergunta[0].idpesquisa03 != 0;

                    JArray jValores = new JArray();

                    if (filtrosPergunta[0].idpesquisa03 != 0)
                    {
                        foreach (var fp in filtrosPergunta)
                        {
                            JObject objValor = new JObject();
                            objValor["vlresposta"] = fp.vlresposta;

                            jValores.Add(objValor);
                        }
                    }
                    else
                    {
                        JObject objValor = new JObject();
                        objValor["vlresposta"] = filtrosPergunta[0].vlrde;

                        jValores.Add(objValor);

                        objValor = new JObject();
                        objValor["vlresposta"] = filtrosPergunta[0].vlrate;

                        jValores.Add(objValor);
                    }

                    objPergunta["valores"] = jValores;

                    jFiltros.Add(objPergunta);
                }

                obj["filtros"] = jFiltros;

                HttpResponseMessage resposta = await ws.Post("obterRespostasAgrupadasPorOnda", obj);

                String message = await resposta.Content.ReadAsStringAsync();

                if (resposta.IsSuccessStatusCode)
                {
                    DAO_Download daoDownload = DAO_Download.Instance;
                    daoDownload.InserirDownload(new CE_Download() { data = DateTime.Now, idpesquisa01 = idpesquisa01 });
                    Utils.SalvarArquivo("onda_" + idpesquisa06 + ".json", message);
                }
                else
                {
                    throw new Exception(message);
                }
            }
            catch (Exception)
            {
                throw new Exception("Não foi possível efetuar o download dos dados.");
            }
        }

        public async Task DownloadRespostas(List<Int32> ids, Int32 idpesquisa01)
        {
            try
            {
                String idsPesquisas06 = "";
                String strIdsPesquisa06 = "";
                JObject obj = new JObject();
                

                if (ids.Count == 1)
                {
                    idsPesquisas06 = "(" + ids[0] + ")";
                }
                else
                {
                    idsPesquisas06 = "(";

                    for (int i = 0; i < ids.Count; i++)
                    {
                        if (i != ids.Count - 1)
                        {
                            idsPesquisas06 += ids[i].ToString() + ",";
                            strIdsPesquisa06 += ids[i].ToString() + "_";
                        }
                        else
                        {
                            idsPesquisas06 += ids[i].ToString();
                            strIdsPesquisa06 += ids[i].ToString();
                        }
                            
                    }

                    idsPesquisas06 += ")";
                }

                obj["idsPesquisas06"] = idsPesquisas06;

                List<CE_Filtro> filtros = daoFiltro.ObterFiltrosPorPesquisa(idpesquisa01);

                var groupPergunta = filtros.GroupBy(o => o.idpesquisa04);

                JArray jFiltros = new JArray();

                foreach (var item in groupPergunta)
                {
                    List<CE_Filtro> filtrosPergunta = filtros.Where(o => o.idpesquisa04 == item.Key).ToList();

                    JObject objPergunta = new JObject();
                    objPergunta["idpesquisa04"] = item.Key;
                    objPergunta["temopcoes"] = filtrosPergunta[0].idpesquisa03 != 0;

                    JArray jValores = new JArray();

                    if (filtrosPergunta[0].idpesquisa03 != 0)
                    {
                        foreach (var fp in filtrosPergunta)
                        {
                            JObject objValor = new JObject();
                            objValor["vlresposta"] = fp.vlresposta;

                            jValores.Add(objValor);
                        }
                    }
                    else
                    {
                        JObject objValor = new JObject();
                        objValor["vlresposta"] = filtrosPergunta[0].vlrde;

                        jValores.Add(objValor);

                        objValor = new JObject();
                        objValor["vlresposta"] = filtrosPergunta[0].vlrate;

                        jValores.Add(objValor);
                    }

                    objPergunta["valores"] = jValores;

                    jFiltros.Add(objPergunta);
                }

                obj["filtros"] = jFiltros;

                HttpResponseMessage resposta = await ws.Post("obterRespostasAgrupadasEntreOndas", obj);

                String message = await resposta.Content.ReadAsStringAsync();

                if (resposta.IsSuccessStatusCode)
                {
                    DAO_Download daoDownload = DAO_Download.Instance;
                    daoDownload.InserirDownload(new CE_Download() { data = DateTime.Now, idpesquisa01 = idpesquisa01 });
                    Utils.SalvarArquivo("ondas_" + strIdsPesquisa06 + ".json", message);
                }
                else
                {
                    throw new Exception(message);
                }
            }
            catch (Exception)
            {
                throw new Exception("Não foi possível efetuar o download dos dados.");
            }
        }

        public async Task Download(String dataInicio, String dataFim)
        {
            try
            {
                JObject obj = new JObject();
                obj["dataInicio"] = dataInicio;
                obj["dataFim"] = dataFim;

                var pesquisador = Utils.ObterPesquisadorLogado();

                obj["idcliente"] = pesquisador.idcliente;

                HttpResponseMessage resposta = await ws.Post("obterPesquisasPorPeriodo", obj);

                String message = await resposta.Content.ReadAsStringAsync();

                if (resposta.IsSuccessStatusCode)
                {
                    ListPesquisas listPesquisas = JsonConvert.DeserializeObject<ListPesquisas>(message);

                    this.listPesquisas.AddRange(listPesquisas.pesquisas);

                    var groupPesquisas = listPesquisas.pesquisas.GroupBy(o => o.pesquisa01.idpesquisa01);

                    foreach (var grpPesq in groupPesquisas)
                    {
                        var pesquisa = this.listPesquisas.FirstOrDefault(o => o.pesquisa01.idpesquisa01 == grpPesq.Key);
                        
                        await ObterDadosPerguntas(pesquisa.pesquisa01.idpesquisa01);
                        
                    }

                    dao06.DeleteAll();
                    dao01.DeleteAll();
                    dao03.DeleteAll();
                    dao04.DeleteAll();
                    dao02.DeleteAll();

                    var group01 = this.listPesquisas.GroupBy(o => o.pesquisa01.idpesquisa01);

                    foreach (var grp1 in group01)
                    {
                        var pq01 = this.listPesquisas.FirstOrDefault(o => o.pesquisa01.idpesquisa01 == grp1.Key).pesquisa01;

                        dao01.InserirPesquisa(pq01);
                    }

                    var group06 = listPesquisas.pesquisas.GroupBy(o => o.idpesquisa06);

                    foreach (var grp6 in group06)
                    {
                        var onda = this.listPesquisas.FirstOrDefault(o => o.idpesquisa06 == grp6.Key);
                        onda.idpesquisa01 = onda.pesquisa01.idpesquisa01;
                        dao06.InserirOnda(onda);
                    }

                    var group02 = this.listPerguntas.Where(w => w.pesquisa02.idpesquisa02 != 0).GroupBy(o => o.pesquisa02.idpesquisa02);

                    foreach (var grp2 in group02)
                    {
                        var tipo = this.listPerguntas.Where(w => w.pesquisa02.idpesquisa02 != 0).FirstOrDefault(o => o.pesquisa02.idpesquisa02 == grp2.Key).pesquisa02;
                        dao02.InserirTipo(tipo);
                    }

                    var group03 = this.listPerguntas.Where(w => w.pesquisa03.idpesquisa03 != 0).GroupBy(o => o.pesquisa03.idpesquisa03);

                    
                    foreach (var grp3 in group03)
                    {
                        var valor = this.listPerguntas.Where(w => w.pesquisa02.idpesquisa02 != 0 && w.pesquisa03.idpesquisa03 != 0).FirstOrDefault(o => o.pesquisa03.idpesquisa03 == grp3.Key);
                        valor.pesquisa03.idpesquisa02 = valor.pesquisa02.idpesquisa02;
                        dao03.InserirValor(valor.pesquisa03);
                    }

                    var group04 = this.listPerguntas.Where(w => w.idpesquisa04 != 0).GroupBy(o => o.idpesquisa04);

                    foreach (var grp4 in group04)
                    {
                        var pergunta = this.listPerguntas.FirstOrDefault(o => o.idpesquisa04 == grp4.Key);
                        pergunta.idpesquisa02 = pergunta.pesquisa02.idpesquisa02;
                        dao04.InserirPergunta(pergunta);
                    }

                    listPesquisas = null;
                    this.listPesquisas = null;
                    this.listPerguntas = null;
                }
                else
                {
                    throw new Exception(message);
                }

            }
            catch (Exception)
            {
                throw new Exception("Não foi possível efetuar o download dos dados.");
            }

        }
    }
}
