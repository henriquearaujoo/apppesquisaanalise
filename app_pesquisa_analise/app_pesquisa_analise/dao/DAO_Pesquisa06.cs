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
    public class DAO_Pesquisa06
    {
        private SQLiteConnection conn;
        private static DAO_Pesquisa06 instance;

        public DAO_Pesquisa06()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            //conn.DropTable<CE_Pesquisa06>();
            conn.CreateTable<CE_Pesquisa06>();
        }

        public static DAO_Pesquisa06 Instance
        {
            get
            {
                if (instance == null)
                    instance = new DAO_Pesquisa06();

                return instance;
            }
        }

        public List<CE_Pesquisa06> ObterOndas()
        {
            return conn.Query<CE_Pesquisa06>("SELECT * FROM [tb_pesquisa06]").Where(o => DateTime.Now >= DateTime.Parse(o.dtiniciopesquisa) && DateTime.Now <= DateTime.Parse(o.dtfimpesquisa)).ToList();
        }

        public List<CE_Pesquisa06> ObterOndasPorPeriodo(Int32 idpesquisa01, DateTime inicio, DateTime fim)
        {
            return conn.Query<CE_Pesquisa06>("SELECT * FROM [tb_pesquisa06] WHERE [idpesquisa01] = " + idpesquisa01).Where(o => (DateTime.Parse(o.dtiniciopesquisa).Month >= inicio.Month && DateTime.Parse(o.dtiniciopesquisa).Year >= inicio.Year) && (DateTime.Parse(o.dtfimpesquisa).Month <= fim.Month && DateTime.Parse(o.dtfimpesquisa).Year <= fim.Year)).ToList();
        }

        public void InserirOnda(CE_Pesquisa06 onda)
        {
            conn.Insert(onda);
        }

        public void AtualizarOnda(CE_Pesquisa06 onda)
        {
            conn.Update(onda);
        }

        public void SalvarOnda(CE_Pesquisa06 onda)
        {
            if (onda.idpesquisa06 == 0)
                conn.Insert(onda);
            else
                conn.Update(onda);
        }

        public Int32 DeleteOnda(Int32 id)
        {
            return conn.Delete<CE_Pesquisa06>(id);
        }

        public void DeleteAll()
        {
            conn.DeleteAll<CE_Pesquisa06>();
        }
    }
}
