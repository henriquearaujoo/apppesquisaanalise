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
    public class DAO_Download
    {
        private SQLiteConnection conn;
        private static DAO_Download instance;

        public DAO_Download()
        {
            conn = DependencyService.Get<ISQLite>().GetConnection();
            //conn.DropTable<CE_Filtro>();
            conn.CreateTable<CE_Download>();
        }

        public static DAO_Download Instance
        {
            get
            {
                if (instance == null)
                    instance = new DAO_Download();

                return instance;
            }
        }
        
        public CE_Download ObterUltimoDownload(Int32 idpesquisa01)
        {
            return conn.Query<CE_Download>("SELECT * FROM [tb_download] WHERE [idpesquisa01] = " + idpesquisa01).LastOrDefault();
        }

        public void InserirDownload(CE_Download download)
        {
            conn.Insert(download);
        }

        public void AtualizarDownload(CE_Download download)
        {
            conn.Update(download);
        }

        public void SalvarFiltro(CE_Download download)
        {
            if (download.iddownload == 0)
                InserirDownload(download);
            else
                AtualizarDownload(download);
        }

        public Int32 DeleteFiltro(Int32 idfiltro)
        {
            return conn.Delete<CE_Download>(idfiltro);
        }        

        public void DeleteAll()
        {
            conn.DeleteAll<CE_Download>();
        }
    }
}
