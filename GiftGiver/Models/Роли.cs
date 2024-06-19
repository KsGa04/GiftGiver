namespace GiftGiver
{
    public partial class Роли
    {
        public Роли()
        {
            Администраторs = new HashSet<Администратор>();
            Пользовательs = new HashSet<Пользователь>();
        }

        public int РолиId { get; set; }
        public string Наименование { get; set; } = null!;

        public virtual ICollection<Администратор> Администраторs { get; set; }
        public virtual ICollection<Пользователь> Пользовательs { get; set; }
    }
}
