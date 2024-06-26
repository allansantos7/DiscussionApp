using Azure;
using Azure.Core;
using DataAccess;
using Infrastructure.Models;
using LogicLeapPiazzaRemake.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Web;
using Utility;

namespace LogicLeapPiazzaRemake.Services
{
    public interface IUserSessionService
    {
        Task CreateUserSession(ClaimsPrincipal user, UserManager<ApplicationUser> _userManager, UnitOfWork _unitOfWork, int classId = 0);
        UserSessionModel GetUserSession();
    }

    public class UserSessionService : IUserSessionService
    {
        private UserSessionModel UserSession;

        public UserSessionModel GetUserSession()
        {
            return UserSession != null ? UserSession : new UserSessionModel();
        }

        public async Task CreateUserSession(ClaimsPrincipal user, UserManager<ApplicationUser> _userManager, UnitOfWork _unitOfWork, int classId = 0) //Creates new UserSession using claimed UserId and setting ClassId to first available for that UserId
        {
            try
            {
                var userId = _userManager.GetUserId(user);
                if (userId != null)
                {
                    if (classId == 0)
                    {
                        var enrollments = _unitOfWork.Enrollment.GetAll(c => c.ApplicationUser.Id == userId);
                        classId = (int)enrollments.First().ClassId;
                    }
                    var userSession = new UserSessionModel
                    {
                        UserId = userId,
                        ClassId = classId
                    };
                    UserSession = userSession;
                }
                else if (classId != 0)
                {
                    UserSession.ClassId = classId;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.Message;
            }
        }
    }
}
