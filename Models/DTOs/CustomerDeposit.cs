using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.DTOs
{
    public class CustomerDepositDTO
    {
        public string FiscalYear { get; set; }
        public string Month { get; set; }
        public double AmountPaid { get; set; }
        public string PmtMethod { get; set; }
        public string ProcessingStatus { get; set; }
        public string CustName { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string JournalNum { get; set; }
        public double WHTDeducted { get; set; }
        public string BankName { get; set; }
        public string Currency { get; set; }
        public string PostedWithJournalNum { get; set; }
        public string SysBankAccount { get; set; }
        public long RecordId { get; set; }
    }

    public class CustomerDepositRec
    {
        public long RecordId { get; set; }
    }
}
