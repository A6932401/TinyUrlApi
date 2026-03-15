using TinyUrlApp.Model;

namespace TinyUrlApp.Logic.Interface
{
    public interface IEndPointLogic
    {
        public ReturnLink AddLink(LinkAdd linkAdd);
        public bool UpdateClickCount(int linkId);
        public bool DeleteEndPoint(int linkId);
        public List<ReturnLink> GetLink(bool isPrivate, int id);
    }
}
