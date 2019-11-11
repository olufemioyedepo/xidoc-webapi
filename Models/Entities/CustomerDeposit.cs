using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class CustomerDeposit
    {
        public int SerialNum { get; set; }

        public double WHTDeducted { get; set; }

        [Required]
        public double AmountPaid { get; set; }

        [MaxLength(25)]
        public string EmployeeId { get; set; }

        [MaxLength(10)]
        public string FiscalYear { get; set; }

        [Required]
        public Enum Month { get; set; }

        [MaxLength(20)]
        public string CustId { get; set; }

        [MaxLength(100)]
        public string CustName { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [MaxLength(60)]
        public string BankName { get; set; }

        [MaxLength(60)]
        public string DepositorName { get; set; }

        [MaxLength(60)]
        public string EmployeeName { get; set; }

        [Required]
        public Enum PmtMethod { get; set; }

        public Enum ProcessingStatus { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }

        public Int64 EmployeeRecId { get; set; }

        [MaxLength(10)]
        public string SysBankAccount { get; set; }

    }
}
