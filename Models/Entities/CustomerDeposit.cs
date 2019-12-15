using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.Entities
{
    public class CustomerDeposit
    {
        [Required]
        public double AmountPaid { get; set; }

        [MaxLength(60)]
        public string BankName { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; }

        [MaxLength(10)]
        public string FiscalYear { get; set; }

        [MaxLength(20)]
        public string CustId { get; set; }

        [MaxLength(100)]
        public string CustName { get; set; }

        [MaxLength(60)]
        public string EmployeeName { get; set; }

        [MaxLength(25)]
        public string EmployeeId { get; set; }

        [Required]
        public string Month { get; set; }

        [Required]
        public string PaymentDate { get; set; } //YYY-MM-DD

        [Required]
        public string PmtMethod { get; set; }

        [MaxLength(60)]
        public string DepositorName { get; set; }

        public string ProcessingStatus { get; set; }

        public double WHTDeducted { get; set; }

        // public Int64 EmployeeRecId { get; set; }
    }

    public class CustomerDepositForSave
    {
        public double AmountPaid { get; set; }

        public string BankName { get; set; }

        public string Currency { get; set; }

        public string FiscalYear { get; set; }

        public string CustId { get; set; }

        public string CustName { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeId { get; set; }

        public string Month { get; set; }

        public string PaymentDate { get; set; } //YYY-MM-DD

        public string PmtMethod { get; set; }

        public string DepositorName { get; set; }

        public string ProcessingStatus { get; set; }

        public double WHTDeducted { get; set; }

        // public Int64 EmployeeRecId { get; set; }

        public string UniqueId { get; set; }
    }
}
