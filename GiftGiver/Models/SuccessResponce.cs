using System.Runtime.Serialization;

namespace GiftGiver.Models
{
    public class SuccessResponse
    {
        /// <summary>
        ///     Результат
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        ///     Сообщение
        /// </summary>
        public string Message { get; set; }
    }
}
