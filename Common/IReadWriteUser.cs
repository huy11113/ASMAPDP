namespace ASMAPDP.Common
{
    public interface IReadWriteUser
    {
        public List<User> ReadUser();
        public void WriteUser(List<User> users);
    }
}
