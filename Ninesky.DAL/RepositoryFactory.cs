using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninesky.IDAL;

namespace Ninesky.DAL
{
    /// <summary>
    /// 简单工厂
    /// </summary>
    public static class RepositoryFactory
    {
        /// <summary>
        /// 用户仓储
        /// </summary>
        public static InterfaceUserRepository UserRepository { get { return new UserRepository(); } }

        /// <summary>
        /// 用户组仓储
        /// </summary>
        public static InterfaceUserGroupRepository UserGroupRepository { get { return new UserGroupRepository(); } }

        /// <summary>
        /// 用户配置仓储
        /// </summary>
        public static InterfaceUserConfigRepository UserConfigRepository { get { return new UserConfigRepository(); } }
    }
}
