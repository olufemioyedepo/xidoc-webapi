using GeofencingWebApi.Models.DTOs;
using GeofencingWebApi.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeofencingWebApi.Models.ODataResponse
{
    public class CustomerDepositResponse
    {
        [JsonProperty("@odata.context")]
        public string odatacontext { get; set; }
        [JsonProperty("value")]
        public List<CustomerDepositDTO> value { get; set; }
    }

    public class CustomerDepositCreateResponse
    {
        public double AmountPaid { get; set; }

        public string BankName { get; set; }

        public string Currency { get; set; }

        public string FiscalYear { get; set; }

        public string CustId { get; set; }

        public string CustName { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeId { get; set; }

        public Enum Month { get; set; }

        public string PaymentDate { get; set; } //YYY-MM-DD

        public Enum PmtMethod { get; set; }

        public string DepositorName { get; set; }

        public Enum ProcessingStatus { get; set; }

        public double WHTDeducted { get; set; }

        public Int64 EmployeeRecId { get; set; }
    }

    public class CustomerDepositItem
    {

    }
}
