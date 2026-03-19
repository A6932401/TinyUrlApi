using System.Data;
using System.Data.Common;
using TinyUrlApp.Model;
using TinyUrlApp.Model.Entities;

namespace TinyUrlApp.DAL.Interface
{
    public interface IEndPointDA
    {
        public DbTransaction DbTransaction { get; set; }
        public long EndPointAdd(EndPoint model);
        public int EndPointClickUpdate(int linkId);
        public int EndPointDeleteById(int linkId);
        public List<ReturnLink> ListAllEndPoints();
        public List<ReturnLink> ListEndPointsByRule(bool isPrivate);
        public ReturnLink ListEndPointsById(int id);
        public int EndPointDeleteByAll();
    }
}
