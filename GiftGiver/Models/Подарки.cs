namespace GiftGiver
{
    public partial class Подарки
    {
        public Подарки()
        {
            Желаемоеs = new HashSet<Желаемое>();
            Лентаs = new HashSet<Лента>();
        }

        public int ПодаркиId { get; set; }
        public byte[]? Изображение { get; set; }
        public string Наименование { get; set; } = null!;
        public decimal Цена { get; set; }
        public int? МинВозраст { get; set; }
        public string? Получатель { get; set; }
        public string? Жанр { get; set; }
        public string Ссылка { get; set; } = null!;

        public virtual ICollection<Желаемое> Желаемоеs { get; set; }
        public virtual ICollection<Лента> Лентаs { get; set; }

    }
}
