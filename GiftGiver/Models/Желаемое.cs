namespace GiftGiver
{
    public partial class Желаемое
    {
        public int ЖелаемоеId { get; set; }
        public int ПользовательId { get; set; }
        public int ПодаркиId { get; set; }

        public virtual Подарки Подарки { get; set; } = null!;
        public virtual Пользователь Пользователь { get; set; } = null!;
    }
}
