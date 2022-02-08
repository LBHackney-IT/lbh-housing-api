using System;

namespace HousingRegisterApi.V1.Domain.Sns
{
    public class NovaletSns
    {
        //User that invoked generate novalet export
        public string FileName { get; set; }

        public DateTime InvokeDateTime { get; set; }
    }
}
