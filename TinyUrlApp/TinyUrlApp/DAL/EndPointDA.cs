using Dapper;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Data.Common;
using TinyUrlApp.DAL.Interface;
using TinyUrlApp.Model;
using TinyUrlApp.Model.Entities;

namespace TinyUrlApp.DAL
{
    public class EndPointDA : IEndPointDA
    {
        private readonly DbConnection _connection;
        public DbTransaction DbTransaction { get; set; }

        public EndPointDA(DbConnection connection)
        {
            _connection = connection;
        }

        public long EndPointAdd(EndPoint model)
        {
            var rtn = _connection.Insert<EndPoint>(model, transaction: DbTransaction);
            return rtn;
        }
        public int EndPointClickUpdate(int linkId)
        {
            var p = new DynamicParameters();
            p.Add("linkId", linkId);

            string query = "update EndPoint set clickcount = clickcount + 1 where id = @linkId ";
            
            return _connection.Execute(query, p, DbTransaction);
        }
        public int EndPointDeleteById(int linkId)
        {
            var p = new DynamicParameters();
            p.Add("linkId", linkId);

            string query = "delete from EndPoint where id = @linkId";
           
            return _connection.Execute(query, p, DbTransaction);
        }
        public int EndPointDeleteByAll()
        {
            var p = new DynamicParameters();

            string query = "delete from EndPoint";

            return _connection.Execute(query,p, DbTransaction);
        }
        public ReturnLink ListEndPointsById(int id)
        {
            var p = new DynamicParameters();
            p.Add("id", id);

            string query = "select id, shortlink,originallink,isprivate,status,createddate,clickcount from EndPoint where id =@id";            

            var rtn = _connection.Query<ReturnLink>(query, p, transaction: DbTransaction).FirstOrDefault();
            return rtn;

        }
        public List<ReturnLink> ListEndPointsByRule(bool isPrivate)
        {
            var p = new DynamicParameters();
            p.Add("isPrivate", isPrivate ? 1 : 0);

            string query = @"select id,shortlink,originallink,isprivate,status,createddate,clickcount from EndPoint where isprivate =@isPrivate ";
            

            var rtn = _connection.Query<ReturnLink>(query, p, transaction: DbTransaction).ToList();
            return rtn;

        }
        public List<ReturnLink> ListAllEndPoints()
        {

            string query = @"select id,shortlink,originallink,isprivate,status,createddate,clickcount from EndPoint ";           

            var rtn = _connection.Query<ReturnLink>(query, transaction: DbTransaction).ToList();
            return rtn;

        }
    }
}
