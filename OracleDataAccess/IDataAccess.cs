namespace MOFAAPI.OracleDataAccess
{
    public interface IDataAccess
    {    
        public Models.UserInfo getUserDetails(dynamic requestParameter);
        public Models.Response addCompany(dynamic requestParameter);
        public Models.Response registerCompany(dynamic requestParameter);
        public Models.Response AddAttestation(dynamic requestParameter);
    }
}
