using TinyUrlApp.DAL.Interface;
using TinyUrlApp.Logic.Interface;
using TinyUrlApp.Model;
using TinyUrlApp.Model.Entities;
using TinyUrlApp.UnitOfWork;
using TinyUrlApp.Utility;

namespace TinyUrlApp.Logic
{
    public class EndPointLogic : IEndPointLogic
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEndPointDA _da;
        public EndPointLogic(IUnitOfWork unitOfWork,IEndPointDA da) {
            _unitOfWork = unitOfWork;
            _da = da;
        }

        public ReturnLink AddLink(LinkAdd linkAdd)
        {
            using (_unitOfWork.GetDbConnection())
            {
                using(_unitOfWork.Begin())
                {
                    _da.DbTransaction = _unitOfWork.Transaction;
                    string ShortLink = linkAdd.originalUrl.GetShortLink();
                    var endPoint = new EndPoint
                    {
                        shortlink = ShortLink,
                        originallink = linkAdd.originalUrl,
                        isprivate = linkAdd.isPrivate,
                        status = "Y",
                        clickcount = 0,
                        createddate = DateTime.Now
                    };
                  long idVal =  _da.EndPointAdd(endPoint);
                    _unitOfWork.Commit();
                    var result = new ReturnLink
                    {
                        id = Convert.ToInt32(idVal),
                        shortlink = ShortLink,
                        originallink = linkAdd.originalUrl,
                        clickcount = 0,
                        createddate = DateTime.Now
                    };
                    return result;
                }
            }
                
        }

        public bool UpdateClickCount(int linkId)
        {
            using (_unitOfWork.GetDbConnection())
            {
                using (_unitOfWork.Begin())
                {
                    _da.DbTransaction = _unitOfWork.Transaction;
                    
                    _da.EndPointClickUpdate(linkId);
                    _unitOfWork.Commit();
                    return true;
                }
            }
        }

        public bool DeleteEndPoint(int linkId)
        {
            using (_unitOfWork.GetDbConnection())
            {
                using (_unitOfWork.Begin())
                {
                    _da.DbTransaction = _unitOfWork.Transaction;

                    _da.EndPointDelete(linkId);
                    _unitOfWork.Commit();
                    return true;
                }
            }
        }
        public List<ReturnLink> GetLink(bool isPrivate,int id)
        {
            using (_unitOfWork.GetDbConnection())
            {
                var idVal = _da.ListEndPoints(isPrivate,id);                    
                return idVal;            
            }

        }
    }
}
