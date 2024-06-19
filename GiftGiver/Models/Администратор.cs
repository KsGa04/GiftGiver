namespace GiftGiver
{
    public partial class Администратор
    {
        public int АдминистраторId { get; set; }
        public string Логин { get; set; } = null!;
        public string Пароль { get; set; } = null!;
        public int РолиId { get; set; }

        public virtual Роли Роли { get; set; } = null!;
    }
}
