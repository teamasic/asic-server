using AsicServer.Core.Constant;
using AsicServer.Core.Entities;
using AsicServer.Core.Models;
using AsicServer.Core.Utils;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using DataService.Repository;
using DataService.UoW;
using DataService.Validation;
using FirebaseAdmin.Auth;
using FluentValidation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Service
{
    public interface IUserService : IBaseService<User>
    {
        Task<AccessTokenResponse> Authenticate(UserAuthentication user);
        Task<AccessTokenResponse> Register(RegisteredUser user);
        User GetByRollNumber(string rollnumber);
        //Task<AccessTokenResponse> RegisterExternalUsingFirebaseAsync(FirebaseRegisterExternal external);
    }

    public class UserService : BaseService<User>, IUserService
    {
        private readonly JwtTokenProvider jwtTokenProvider;
        private readonly IUserRepository repository;

        public UserService(IUserRepository repository,
                            UnitOfWork unitOfWork,
                            JwtTokenProvider jwtTokenProvider) : base(unitOfWork)
        {
            this.jwtTokenProvider = jwtTokenProvider;
            this.repository = repository;
        }

        public async Task<AccessTokenResponse> Authenticate(UserAuthentication userAuthen)
        {
            if (!string.IsNullOrEmpty(userAuthen.FirebaseToken))
            {
                return await AuthenticateByFirebaseAsync(new FirebaseRegisterExternal() { FirebaseToken = userAuthen.FirebaseToken });
            }
            else if (!string.IsNullOrEmpty(userAuthen.Username)
                && !string.IsNullOrEmpty(userAuthen.Password))
            {
                return AuthenticateByUsernameAndPassword(userAuthen);
            }
            throw new BaseException(ErrorMessage.CREDENTIALS_NOT_MATCH);
        }

        public async Task<AccessTokenResponse> Register(RegisteredUser userRegister)
        {
            using (var trans = unitOfWork.CreateTransaction())
            {
                AccessTokenResponse token = null;
                RegisteredUserValidation validation = new RegisteredUserValidation(this.repository);
                validation.ValidateAndThrow(userRegister);

                var user = userRegister.ToEntity<User>();

                try
                {
                    byte[] hash, salt;
                    PasswordManipulation.CreatePasswordHash(userRegister.Password, out hash, out salt);
                    user.PasswordHash = hash;
                    user.PasswordSalt = salt;

                    var roles = userRegister.Role.Trim().Split(",");
                    foreach (var role in roles)
                    {
                        user.UserRole.Add(new UserRole()
                        {
                            RoleId = (int)Enum.Parse(typeof(RolesEnum), role, true)
                        });
                    }

                    await this.repository.AddAsync(user);
                    token = CreateToken(user);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                return token;
            }
        }

        //public async Task<AccessTokenResponse> RegisterExternalUsingFirebaseAsync(FirebaseRegisterExternal external)
        //{
        //    using (var trans = unitOfWork.CreateTransaction())
        //    {
        //        AccessTokenResponse token = null;
        //        try
        //        {
        //            FirebaseRegisterExternalValidation validation = new FirebaseRegisterExternalValidation();
        //            validation.ValidateAndThrow(external);

        //            FirebaseToken decodedToken = validation.ParsedToken;

        //            var claims = decodedToken.Claims;
        //            string email = claims["email"] + "";
        //            string name = claims["name"] + "";
        //            string avatar = claims["picture"] + "";

        //            var user = repository.GetUserByUsername(email);
        //            if (user == null)
        //            {
        //                user = new User()
        //                {
        //                    Email = email,
        //                    Username = email,
        //                    Fullname = name
        //                };
        //                user.UserRole.Add(new UserRole()
        //                {
        //                    RoleId = (int)RolesEnum.MEMBER
        //                });
        //                await this.repository.AddAsync(user);
        //            }
        //            token = CreateToken(user);
        //            trans.Commit();
        //        }
        //        catch (Exception e)
        //        {
        //            trans.Rollback();
        //            throw e;
        //        }
        //        return token;
        //    }
        //}

        private AccessTokenResponse CreateToken(User user)
        {
            return new AccessTokenResponse()
            {
                User = user.ToViewModel<UserViewModel>(),
                AccessToken = jwtTokenProvider.CreateAccesstoken(user),
                Roles = user.UserRole.Select(ur => ur.RoleId.ToString()).ToArray()
            };
        }

        private AccessTokenResponse AuthenticateByUsernameAndPassword(UserAuthentication userAuthen)
        {
            var user = repository.GetUserByUsername(userAuthen.Username);
            AccessTokenResponse token = null;

            UserAuthenticationValidation validation = new UserAuthenticationValidation();
            var validationResult = validation.Validate(userAuthen);

            if (!validationResult.IsValid || user == null)
                throw new BaseException(ErrorMessage.CREDENTIALS_NOT_MATCH);

            var result = PasswordManipulation.VerifyPasswordHash(userAuthen.Password,
                                user.PasswordHash, user.PasswordSalt);
            if (user != null && result)
            {
                token = CreateToken(user);
            }
            else
            {
                throw new BaseException(ErrorMessage.CREDENTIALS_NOT_MATCH);
            }

            return token;
        }

        private async Task<AccessTokenResponse> AuthenticateByFirebaseAsync(FirebaseRegisterExternal external)
        {
            using (var trans = unitOfWork.CreateTransaction())
            {
                AccessTokenResponse token = null;
                try
                {
                    FirebaseRegisterExternalValidation validation = new FirebaseRegisterExternalValidation();
                    validation.ValidateAndThrow(external);

                    FirebaseToken decodedToken = validation.ParsedToken;

                    var claims = decodedToken.Claims;
                    string email = claims["email"] + "";
                    string name = claims["name"] + "";
                    string avatar = claims["picture"] + "";

                    var user = repository.GetUserByUsername(email);
                    if (user == null)
                    {
                        user = new User()
                        {
                            Email = email,
                            Username = email,
                            Fullname = name
                        };
                        user.UserRole.Add(new UserRole()
                        {
                            RoleId = (int)RolesEnum.ATTENDEE
                        });
                        await this.repository.AddAsync(user);
                    }
                    token = CreateToken(user);
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw e;
                }
                return token;
            }
        }

        public User GetByRollNumber(string rollnumber)
        {
            return repository.GetByRollNumber(rollnumber);
        }
    }
}
