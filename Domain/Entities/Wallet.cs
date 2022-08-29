using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Wallet
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// شماره حساب
        /// </summary>
        [Required]
        [Range(100000, 999999)]
        public int AccountNumber { get; set; }

        /// <summary>
        /// نام و نام خانوادگی صاحب حساب
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// موجودی قابل برداشت
        /// </summary>
        [Required]
        public long WithdrawalBalance { get; set; }

        /// <summary>
        /// موجودی بلاک شده
        /// </summary>
        [Required]
        public long BlockedInventory { get; set; }

        /// <summary>
        /// موجودی کل
        /// </summary>S
        [Required]
        public long TotalInventory { get; set; }
    }
}
