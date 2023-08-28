namespace MOFAAPI.Models
{
    public class EntityModel
    {
        public string entityCode { get; set; }
        public string requesterName { get; set; }
        public string requestDate { get; set; }
        public string idType { get; set; }
        public string idNumber { get; set; }
        public string nationality { get; set; }
        public string emailId { get; set; }
        public string contactNo { get; set; }
        public string reqAppNumber { get; set; }
        public string attestfee { get; set; }
        public string payTransNo { get; set; }
        public string attestDocType { get; set; }
        public string attestDocName { get; set; }
        public string attestFromCountryCode { get; set; }
        public string attestToCountryCode { get; set; }
        public string attestDocCoords { get; set; }
        public string attestFile { get; set; }//base 64 string
    }
    public class response
    {
        public string entityCode { get; set; }
        public string reqAppNumber { get; set; }
        public string requesterName { get; set; }
        public string attestNo { get; set; }//generated from DB
        public string docOwnerEmail { get; set; }
        public string attestDocName { get; set; }
        public string attestedFile { get; set; }
        public int statusCode { get; set; }
    }
}
