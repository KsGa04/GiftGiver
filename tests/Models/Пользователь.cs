using System;
using System.Collections.Generic;

namespace tests
{
    public partial class Пользователь
    {
        public Пользователь()
        {
            Желаемоеs = new HashSet<Желаемое>();
            Лентаs = new HashSet<Лента>();
        }

        public int ПользовательId { get; set; }
        public string Логин { get; set; } = null!;
        public string Пароль { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Фио { get; set; }
        public DateTime? Возраст { get; set; }
        public DateTime? ДатаПосещения { get; set; }
        public int РолиId { get; set; }

        public virtual Роли Роли { get; set; } = null!;
        public virtual ICollection<Желаемое> Желаемоеs { get; set; }
        public virtual ICollection<Лента> Лентаs { get; set; }
    }
}
