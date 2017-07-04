using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.model
{
    [Table("tb_pesquisa03")]
    public class CE_Pesquisa03
    {
        [PrimaryKey, Column("idpesquisa03")]
        public Int32 idpesquisa03 { get; set; }
        public String descricao { get; set; }
        public String retornopesquisa { get; set; }
        public Int32 ordem { get; set; }
        public Int32 cor { get; set; }

        [ForeignKey(typeof(CE_Pesquisa02))]
        public Int32 idpesquisa02 { get; set; }

        [Ignore]
        public CE_Pesquisa02 pesquisa02 { get; set; }
        [Ignore]
        public Boolean selecionado { get; set; }

        public override String ToString()
        {
            return descricao;
        }
    }
}
