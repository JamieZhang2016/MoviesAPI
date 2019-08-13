using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetSingle(string userName);
    }
}
