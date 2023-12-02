using Application.Abstraction.AbsToken;
using Application.DTOs;
using Domain.Entities.user;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Concrete
{
    public class TokenHandler : ITokenHandler
    {

        public Application.DTOs.Token createAccessToken(int minute, string securityKey, string Audience, string Issuer, ApplicationUser user)
        {
            Application.DTOs.Token token = new();

            // SecurityKey in simetirğini alıyoruz
            SymmetricSecurityKey symSecurityKey = new(Encoding.UTF8.GetBytes(securityKey));

            //Şifrelenmiş Kimliği Oluşturuyoruz
            SigningCredentials signingCredentials = new(symSecurityKey, SecurityAlgorithms.HmacSha256);

            //oluşturulacak Token ayarlarını veriyoruz
            token.Expiration = DateTime.UtcNow.AddMinutes(minute);

            JwtSecurityToken securityToken = new(
                audience: Audience,
                issuer: Issuer,
                expires: token.Expiration,
                notBefore: DateTime.UtcNow, // burada token oluştulduğu anda ömrü başlasın diyoruz
                signingCredentials: signingCredentials,
                claims: new List<Claim> { new(ClaimTypes.Name, user.UserName)}
                
                ) ;

            //Token oluşturucu sınıfından bir örnek alalım
            JwtSecurityTokenHandler tokenHandler = new();
            token.AccessToken = tokenHandler.WriteToken(securityToken);

            return token;

        }
    }
}
