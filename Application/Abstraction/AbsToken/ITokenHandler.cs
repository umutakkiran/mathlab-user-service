using Domain.Entities.user;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstraction.AbsToken
{
    public interface ITokenHandler
    {
        DTOs.Token createAccessToken(int minute, string securityKey, string Audience, string Issuer, ApplicationUser user);

    }
}
