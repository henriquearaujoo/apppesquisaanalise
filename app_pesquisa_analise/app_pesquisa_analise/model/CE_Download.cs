using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_pesquisa_analise.model
{
    [Table("tb_download")]
    public class CE_Download
    {
        [PrimaryKey, AutoIncrement, Column("iddownload")]
        public Int32 iddownload { get; set; }
        public Int32 idpesquisa01 { get; set; }
        public DateTime data { get; set; }
    }
}
