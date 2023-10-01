using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSWebClient1
{
    internal class InvoiceData
    {
        public string state { get; set; }
        public string amount { get; set; }
        public string discount { get; set; }

        public string lastscanned { get; set; }
        public string grossamount { get; set; }
        public string balance { get; set; }
        public string netamount { get; set; }
        public string vat { get; set; }
        public DataTable lines { get; set; }
        public InvoiceData() { }
    }
}
