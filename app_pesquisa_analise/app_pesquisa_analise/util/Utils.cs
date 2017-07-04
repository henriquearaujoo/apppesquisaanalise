using app_pesquisa_analise.dao;
using app_pesquisa_analise.interfaces;
using app_pesquisa_analise.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace app_pesquisa_analise.util
{
    public class Utils
    {
        public static String ObterImei()
        {
            String imei = DependencyService.Get<IUtils>().ObterIMEI();

            return imei;
        }

        public static bool IsOnline()
        {
            bool isOnline = DependencyService.Get<IUtils>().IsOnline();

            return isOnline;
        }

        public static CE_Pesquisa08 ObterPesquisadorLogado()
        {
            DAO_Pesquisa08 dao08 = DAO_Pesquisa08.Instance;
            return dao08.ObterPesquisadorLogado();
        }

        public static void SalvarArquivo(string filename, string text)
        {
            DependencyService.Get<IUtils>().SalvarArquivo(filename, text);
        }

        public static String CarregarArquivo(string filename)
        {
            return DependencyService.Get<IUtils>().CarregarArquivo(filename);
        }
        
    }
}
