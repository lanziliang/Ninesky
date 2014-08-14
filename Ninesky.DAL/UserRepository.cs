using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninesky.IDAL;
using Ninesky.Models;


namespace Ninesky.DAL
{
    /// <summary>
    /// 用户仓库
    /// </summary>
    public class UserRepository : BaseRepository<User>, InterfaceUserRepository
    {

    }
}
