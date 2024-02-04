using System;
using System.Collections.Generic;

namespace tests
{
    public partial class Лента
    {
        public int ЛентаId { get; set; }
        public int ПользовательId { get; set; }
        public int ПодаркиId { get; set; }
        public DateTime ВремяЗапроса { get; set; }

        public virtual Подарки Подарки { get; set; } = null!;
        public virtual Пользователь Пользователь { get; set; } = null!;
    }
}
