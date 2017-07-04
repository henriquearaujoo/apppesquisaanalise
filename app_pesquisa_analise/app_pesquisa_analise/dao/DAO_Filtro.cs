using app_pesquisa_analise.interfaces;
using app_pesquisa_analise.model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.dao
{
    public class DAO_Filtro
    {
        private SQLiteConnection conn;
        private static DAO_Filtro instance;

        public DAO_Filtro()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            //conn.DropTable<CE_Filtro>();
            conn.CreateTable<CE_Filtro>();
        }

        public static DAO_Filtro Instance
        {
            get
            {
                if (instance == null)
                    instance = new DAO_Filtro();

                return instance;
            }
        }

        public List<CE_Filtro> ObterFiltrosPorPergunta(Int32 idpesquisa04)
        {
            return conn.Query<CE_Filtro>("SELECT * FROM [tb_filtro] WHERE [idpesquisa04] = " + idpesquisa04);
        }

        public List<CE_Filtro> ObterFiltrosPorPesquisa(Int32 idpesquisa01)
        {
            return conn.Query<CE_Filtro>("SELECT * FROM [tb_filtro] WHERE [idpesquisa01] = " + idpesquisa01);
        }

        public List<CE_Filtro> ObterFiltrosPorPesquisaPergunta(Int32 idpesquisa01, Int32 idpesquisa04)
        {
            return conn.Query<CE_Filtro>("SELECT * FROM [tb_filtro] WHERE [idpesquisa01] = " + idpesquisa01 + " AND [idpesquisa04] = " + idpesquisa04);
        }

        public bool TemFiltro(Int32 idpesquisa04)
        {
            int count = conn.ExecuteScalar<int>("SELECT COUNT(idfiltro) FROM [tb_filtro] where [idpesquisa04] = " + idpesquisa04);

            return count > 0;
        }

        public bool TemFiltro(Int32 idpesquisa04, Int32 idpesquisa03)
        {
            int count = conn.ExecuteScalar<int>("SELECT COUNT(idfiltro) FROM [tb_filtro] where [idpesquisa04] = " + idpesquisa04 + " AND [idpesquisa03] = " + idpesquisa03);

            return count > 0;
        }

        public CE_Filtro ObterUltimoFiltro(Int32 idpesquisa01)
        {
            return conn.Query<CE_Filtro>("SELECT * FROM [tb_filtro] WHERE [idpesquisa01] = " + idpesquisa01).LastOrDefault();
        }

        public void InserirFiltro(CE_Filtro filtro)
        {
            conn.Insert(filtro);
        }

        public void AtualizarFiltro(CE_Filtro filtro)
        {
            conn.Update(filtro);
        }

        public void SalvarFiltro(CE_Filtro filtro)
        {
            if (filtro.idfiltro == 0)
                InserirFiltro(filtro);
            else
                AtualizarFiltro(filtro);
        }

        public Int32 DeleteFiltro(Int32 idfiltro)
        {
            return conn.Delete<CE_Filtro>(idfiltro);
        }

        public void DeleteFiltroPorPergunta(Int32 idpesquisa04)
        {
            SQLiteCommand command = conn.CreateCommand("DELETE FROM [tb_filtro] WHERE [idpesquisa04] = " + idpesquisa04);

            command.ExecuteNonQuery();
        }

        public void DeleteAll()
        {
            conn.DeleteAll<CE_Filtro>();
        }
    }
}
