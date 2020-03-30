﻿using AsicServer.Core.Constant;
using AsicServer.Core.Entities;
using AsicServer.Core.Models;
using AsicServer.Core.Utils;
using AsicServer.Core.ViewModels;
using AsicServer.Infrastructure;
using CsvHelper;
using DataService.Repository;
using DataService.UoW;
using DataService.Validation;
using FirebaseAdmin.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Service.UserService
{
    public interface IUserService : IBaseService<User>
    {
        Task<AccessTokenResponse> Authenticate(UserAuthentication user);
        Task<AccessTokenResponse> Register(RegisteredUser user);
        //Task<AccessTokenResponse> RegisterExternalUsingFirebaseAsync(FirebaseRegisterExternal external);
        List<string> CreateMultipleUsers(IFormFile csvFile, IFormFile zipFile);
        Task<bool> CreateSingleUser(IFormFile zipFile, CreateUser user);
        UserViewModel GetByEmail(string email);
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
                    var validationResult = validation.Validate(external);
                    if (!validationResult.IsValid)
                    {
                        var errors = validationResult.Errors.Select(fail =>
                                                    KeyValuePair.Create<string, IEnumerable<string>>
                                                            (fail.PropertyName, new string[] { fail.ErrorMessage })).ToList();
                        throw new BaseException(errors.AsEnumerable());
                    }

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

        public List<string> CreateMultipleUsers(IFormFile csvFile, IFormFile zipFile)
        {
            var stream = csvFile.OpenReadStream();
            TextReader textReader = new StreamReader(stream);
            using (var csv = new CsvReader(textReader, CultureInfo.InvariantCulture))
            {
                var users = csv.GetRecords<CreateUser>();
                var newUsers = new List<User>();
                foreach (var user in users)
                {
                    var newUser = new User()
                    {
                        Username = user.Email,
                        Email = user.Email,
                        Fullname = user.Fullname,
                        RollNumber = user.RollNumber
                    };
                    newUser.UserRole.Add(new UserRole()
                    {
                        RoleId = (int)RolesEnum.ATTENDEE
                    });
                    newUsers.Add(newUser);
                }
                if (repository.AddRangeIfNotInDb(newUsers))
                {
                    var userEntries = UnZip(zipFile.OpenReadStream(), newUsers);
                    var userWithoutImage = GetUsersWithoutImage(userEntries);
                    return userWithoutImage;
                }
                else
                    throw new BaseException(HttpStatusCode.BadRequest, ErrorMessage.INVALID_USERS);
            }
        }

        private Dictionary<string, List<ZipArchiveEntry>> UnZip(Stream zipStream, List<User> users)
        {
            var userStreams = new Dictionary<string, List<ZipArchiveEntry>>();
            foreach (var user in users)
            {
                userStreams.Add(user.RollNumber, new List<ZipArchiveEntry>());
            }

            using (ZipArchive archive = new ZipArchive(zipStream))
            {
                foreach (var entry in archive.Entries)
                {
                    var fileName = entry.FullName.Split('/');
                    var archiveEntry = new List<ZipArchiveEntry>();
                    if (userStreams.ContainsKey(fileName[0]))
                    {
                        if (fileName[1].Length == 0 || (fileName[1].Length > 0 && IsImageFile(fileName[1])))
                        {
                            userStreams.TryGetValue(fileName[0], out archiveEntry);
                            archiveEntry.Add(entry);
                        }
                    }
                }
                ExtrectToFile(userStreams.Values);
            }
            return userStreams;
        }

        private Dictionary<string, List<ZipArchiveEntry>> UnZip(Stream zipStream, User user)
        {
            var userStreams = new Dictionary<string, List<ZipArchiveEntry>>();
            userStreams.Add(user.RollNumber, new List<ZipArchiveEntry>());

            using (ZipArchive archive = new ZipArchive(zipStream))
            {
                foreach (var entry in archive.Entries)
                {
                    var fileName = entry.FullName.Split('/');
                    var archiveEntry = new List<ZipArchiveEntry>();
                    if (userStreams.ContainsKey(fileName[0]))
                    {
                        if (fileName[1].Length == 0 || (fileName[1].Length > 0 && IsImageFile(fileName[1])))
                        {
                            userStreams.TryGetValue(fileName[0], out archiveEntry);
                            archiveEntry.Add(entry);
                        }
                    }
                }
                ExtrectToFile(userStreams.Values);
            }
            return userStreams;
        }

        private void ExtrectToFile(Dictionary<string, List<ZipArchiveEntry>>.ValueCollection values)
        {
            foreach (var item in values)
            {
                foreach (var entry in item)
                {
                    var directory = Directory.GetCurrentDirectory() + "\\Dataset";
                    if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                        directory += Path.DirectorySeparatorChar;
                    string destinationPath = Path.GetFullPath(Path.Combine(directory, entry.FullName));
                    if (destinationPath.EndsWith("\\"))
                    {
                        destinationPath = destinationPath.Remove(destinationPath.Length - 1);
                        if (!Directory.Exists(destinationPath))
                        {
                            Directory.CreateDirectory(destinationPath);
                        }
                    }
                    else
                        entry.ExtractToFile(destinationPath, true);
                }
            }
        }

        private List<string> GetUsersWithoutImage(Dictionary<string, List<ZipArchiveEntry>> users)
        {
            var usersWithOutImage = new List<string>();
            foreach (var user in users)
            {
                if(user.Value.Count == 0)
                {
                    usersWithOutImage.Add(user.Key);
                }
            }
            return usersWithOutImage;
        }

        private bool IsImageFile(string fileName)
        {
            return fileName.EndsWith(".tif", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> CreateSingleUser(IFormFile zipFile, CreateUser user)
        {
            var newUser = new User()
            {
                Username = user.Email,
                Email = user.Email,
                Fullname = user.Fullname,
                RollNumber = user.RollNumber
            };
            newUser.UserRole.Add(new UserRole()
            {
                RoleId = (int)RolesEnum.ATTENDEE
            });
            var userInDb = await repository.AddIfNotInDb(newUser);
            var dictionary = UnZip(zipFile.OpenReadStream(), userInDb);
            var listImages = new List<ZipArchiveEntry>();
            dictionary.TryGetValue(userInDb.RollNumber, out listImages);
            return listImages.Count > 0;
        }

        public UserViewModel GetByEmail(string email)
        {
            var user = repository.GetByEmail(email);
            if(user != null)
            {
                return AutoMapper.Mapper.Map<UserViewModel>(user);
            }
            throw new BaseException(HttpStatusCode.NotFound, ErrorMessage.USER_EMAIL_NOT_FOUND, email);
        }
    }
}
