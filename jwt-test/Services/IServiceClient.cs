using System.Threading.Tasks;
using jwt_test.Contract;

namespace jwt_test.Services
{
    public interface IServiceClient
    {
        Task<ResponseModel> DoGet(RequestModel request);
        
        Task<ResponseModel> DoPost(RequestModel request);
    }
}