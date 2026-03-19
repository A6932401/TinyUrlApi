using TinyUrlApp.Model;

namespace TinyUrlApp.Logic.Interface
{
    public interface IEndPointLogic
    {
        public ReturnLink AddLink(LinkAdd linkAdd);
        public Tuple<bool, string> UpdateClickCount(int linkId);
        public Tuple<bool, string> DeleteEndPoint(int linkId);
        public Tuple<bool, string> DeleteAllEndPoint();
        public ReturnLink GetLinkById(int id);
        public List<ReturnLink> GetLinkByRule(bool isPrivate);
        public List<ReturnLink> GetAllLink();
    }
}
