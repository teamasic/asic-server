using AsicServer.Core.Constant;
using AsicServer.Core.Entities;
using AsicServer.Core.ViewModels;
using AsicServer.Core.Utils;
using AsicServer.Infrastructure;
using CsvHelper;
using DataService.Repository;
using DataService.UoW;
using DataService.Validation;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AttendanceSystemIPCamera.Framework.AppSettingConfiguration;

namespace DataService.Service.UserService
{
    public interface IUserService : IBaseService<User>
    {
        Task<AccessTokenResponse> Authenticate(UserAuthentication user);
        Task<AccessTokenResponse> AuthenticateAsAdmin(UserAuthentication user);
        List<CreateUsersResponse> CreateMultipleUsers(IFormFile csvFile, IFormFile zipFile);
        Task<bool> CreateSingleUser(IFormFile zipFile, CreateUser user);
        UserViewModel GetByEmail(string email);
        List<UserViewModel> GetByCodes(string codes);
    }

    public class UserService : BaseService<User>, IUserService
    {
        private readonly JwtTokenProvider jwtTokenProvider;
        private readonly IUserRepository repository;
        private readonly MyConfiguration configuration;

        public UserService(IUserRepository repository,
                            UnitOfWork unitOfWork,
                            JwtTokenProvider jwtTokenProvider,
                            MyConfiguration configuration) : base(unitOfWork)
        {
            this.jwtTokenProvider = jwtTokenProvider;
            this.repository = repository;
            this.configuration = configuration;
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
                //return AuthenticateByUsernameAndPassword(userAuthen);
                throw new NotImplementedException("Not support authentication with username and password");
            }
            throw new BaseException(ErrorMessage.CREDENTIALS_NOT_MATCH);
        }


        private AccessTokenResponse CreateToken(User user)
        {
            return new AccessTokenResponse()
            {
                User = user.ToViewModel<UserViewModel>(),
                AccessToken = jwtTokenProvider.CreateAccesstoken(user),
                Roles = new string[] { user.RoleId + "" }
            };
        }


        private async Task<AccessTokenResponse> AuthenticateByFirebaseAsync(FirebaseRegisterExternal external)
        {
            User user = null;
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

                    user = repository.GetUserByEmail(email);

                    //add operation is used to minimize effort of testing only
                    if (user == null)
                    {
                        user = new User()
                        {
                            Code = email, //change attendee code in db if you want
                            Email = email,
                            Name = name,
                            Image = avatar,
                            RoleId = (int)RolesEnum.ATTENDEE
                        };
                        await this.repository.AddAsync(user);
                    }
                    trans.Commit();
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    throw e;
                }
                if (user != null)
                {
                    token = CreateToken(user);
                }
                return token;
            }
        }

        public List<CreateUsersResponse> CreateMultipleUsers(IFormFile csvFile, IFormFile zipFile)
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
                        Email = user.Email,
                        Name = user.Fullname,
                        Code = user.Code,
                        Image = user.Image
                    };
                    newUser.RoleId = (int)RolesEnum.ATTENDEE;
                    newUsers.Add(newUser);
                }
                if (repository.AddRangeIfNotInDb(newUsers))
                {
                    var userEntries = UnZip(zipFile.OpenReadStream(), newUsers);
                    var response = GetUsersWithNumberOfImagesSaved(userEntries, newUsers);
                    return response;
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
                userStreams.Add(user.Code, new List<ZipArchiveEntry>());
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
            userStreams.Add(user.Code, new List<ZipArchiveEntry>());

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
            var directory = configuration.DatasetFolderPath;
            var directoryInfo = new DirectoryInfo(Path.GetFullPath(directory));
            directoryInfo.Delete(true);
            foreach (var item in values)
            {
                foreach (var entry in item)
                {
                    //if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    //    directory += Path.DirectorySeparatorChar;
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

        private List<CreateUsersResponse> GetUsersWithNumberOfImagesSaved(Dictionary<string, List<ZipArchiveEntry>> users, List<User> usersInDB)
        {
            var results = new List<CreateUsersResponse>();
            foreach (var user in usersInDB)
            {
                if (users.ContainsKey(user.Code))
                {
                    var listImages = new List<ZipArchiveEntry>();
                    users.TryGetValue(user.Code, out listImages);
                    var result = new CreateUsersResponse()
                    {
                        Email = user.Email,
                        Code = user.Code,
                        Fullname = user.Name,
                        Image = user.Image,
                        NoImageSaved = listImages.Count
                    };
                    results.Add(result);
                }
            }
            return results;
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
                Email = user.Email,
                Name = user.Fullname,
                Code = user.Code,
                Image = user.Image
            };
            newUser.RoleId = (int)RolesEnum.ATTENDEE;
            var userInDb = await repository.AddIfNotInDb(newUser);
            var dictionary = UnZip(zipFile.OpenReadStream(), userInDb);
            var listImages = new List<ZipArchiveEntry>();
            dictionary.TryGetValue(userInDb.Code, out listImages);
            return listImages.Count > 0;
        }

        public UserViewModel GetByEmail(string email)
        {
            var user = repository.GetByEmail(email);
            if (user != null)
            {
                return AutoMapper.Mapper.Map<UserViewModel>(user);
            }
            throw new BaseException(HttpStatusCode.NotFound, ErrorMessage.USER_EMAIL_NOT_FOUND, email);
        }

        public async Task<AccessTokenResponse> AuthenticateAsAdmin(UserAuthentication user)
        {
            var result = await Authenticate(user);
            //check role
            var adminRole = (int)RolesEnum.ADMIN;
            if (!result.Roles.Contains(adminRole.ToString(), StringComparer.OrdinalIgnoreCase))
            {
                throw new BaseException(ErrorMessage.NOT_AUTHORIZED_USER);
            }
            return result;
        }

        public List<UserViewModel> GetByCodes(string codes)
        {
            var listCodes = codes.Split(',').ToList();
            var viewModels = new List<UserViewModel>();
            if(listCodes.Count > 0)
            {
                var users = repository.GetByCodes(listCodes);
                foreach (var user in users)
                {
                    viewModels.Add(AutoMapper.Mapper.Map<UserViewModel>(user));
                }
            }
            return viewModels;
        }
    }
}
