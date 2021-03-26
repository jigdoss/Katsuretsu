using Katsuretsu.Infrastructure.Configuration.IdentityServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Katsuretsu.Infrastructure.Configuration
{
    class IdentityServerDataConfiguration
    {
            public List<Role> Roles { get; set; }
            public List<User> Users { get; set; }
        
    }
}
