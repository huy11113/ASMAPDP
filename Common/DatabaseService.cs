namespace ASMAPDP.Common
{
    public class DatabaseService
    {
        protected string _filePath;
        protected static DatabaseService service;
        protected DatabaseService(string _filePath)
        { 
            this._filePath = _filePath;
        }
        public DatabaseService getInstance()
        {
            if (service == null)
            {
                service = new DatabaseService(_filePath);
            }
            return service;
        }
    }
}
