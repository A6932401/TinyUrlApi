using Microsoft.Extensions.Options;
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
        private readonly AppSetting appSetting;
        public EndPointLogic(IUnitOfWork unitOfWork,IEndPointDA da, IOptions<AppSetting> options )
        {
            _unitOfWork = unitOfWork;
            _da = da;
            appSetting = options.Value;
        }

        public ReturnLink AddLink(LinkAdd linkAdd)
        {
            using (_unitOfWork.GetDbConnection())
            {
                using(_unitOfWork.Begin())
                {
                    _da.DbTransaction = _unitOfWork.Transaction;
                    string ShortLink = appSetting.baseUrl.GetShortLink();
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

        public Tuple<bool,string> UpdateClickCount(int linkId)
        {
            Tuple<bool, string> tuple = Tuple.Create(false, "");
            using (_unitOfWork.GetDbConnection())
            {
                using (_unitOfWork.Begin())
                {
                    _da.DbTransaction = _unitOfWork.Transaction;
                    
                    var chkExists = _da.ListEndPointsById(linkId);
                    if (!string.IsNullOrEmpty(chkExists.originallink)) {
                       int upd =  _da.EndPointClickUpdate(linkId);
                        if (upd > 0) {
                            
                            tuple = Tuple.Create(true, chkExists.originallink);
                        }
                        else
                        {
                            tuple = Tuple.Create(false, "Error during Update");
                        }
                    }
                    else
                    {
                        tuple = Tuple.Create(false, "Invalid Id");

                    }
                    _unitOfWork.Commit();
                    return tuple;
                }
            }
        }

        public Tuple<bool, string> DeleteEndPoint(int linkId)
        {
            Tuple<bool, string> tuple = Tuple.Create(false, "");
            using (_unitOfWork.GetDbConnection())
            {
                using (_unitOfWork.Begin())
                {
                    _da.DbTransaction = _unitOfWork.Transaction;

                    var chkExists = _da.ListEndPointsById(linkId);
                    if (!string.IsNullOrEmpty(chkExists.originallink))
                    {
                        int upd = _da.EndPointDeleteById(linkId);
                        if (upd > 0)
                        {

                            tuple = Tuple.Create(true, "Link Deleted Successfully");
                        }
                        else
                        {
                            tuple = Tuple.Create(false, "Error during Delete");
                        }
                    }
                    else
                    {
                        tuple = Tuple.Create(false, "Invalid Id");

                    }
                    _unitOfWork.Commit();
                    return tuple;
                }
            }
        }

        public Tuple<bool, string> DeleteAllEndPoint()
        {
            Tuple<bool, string> tuple = Tuple.Create(false, "");
            using (_unitOfWork.GetDbConnection())
            {
                using (_unitOfWork.Begin())
                {
                    _da.DbTransaction = _unitOfWork.Transaction;

                    int upd = _da.EndPointDeleteByAll();
                    if (upd > 0)
                    {

                        tuple = Tuple.Create(true, "Link Deleted Successfully");
                    }
                    else
                    {
                        tuple = Tuple.Create(false, "Error during Delete");
                    }
                    _unitOfWork.Commit();
                    return tuple;
                }
            }
        }
        public ReturnLink GetLinkById(int id)
        {
            using (_unitOfWork.GetDbConnection())
            {
                var idVal = _da.ListEndPointsById(id);                    
                return idVal;            
            }
        }
        public List<ReturnLink> GetLinkByRule(bool isPrivate)
        {
            using (_unitOfWork.GetDbConnection())
            {
                var idVal = _da.ListEndPointsByRule(isPrivate);
                return idVal;
            }
        }
        public List<ReturnLink> GetAllLink()
        {
            using (_unitOfWork.GetDbConnection())
            {
                var idVal = _da.ListAllEndPoints();
                return idVal;
            }
        }
    }
}
