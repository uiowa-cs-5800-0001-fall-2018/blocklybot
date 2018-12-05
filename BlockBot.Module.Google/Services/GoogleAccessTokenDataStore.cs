using System;
using System.Linq;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using Google.Apis.Util.Store;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BlockBot.Module.Google.Services
{
    public class GoogleAccessTokenDataStore
    {
        public async Task StoreAsync<T>(ApplicationDbContext _context, string key, T value)
        {
            ApplicationUser user =
                await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim =
                    await _context.ApplicationUserClaims.FirstOrDefaultAsync(x =>
                        x.UserId == user.Id && x.ClaimType == "GoogleAccessToken");
                if (claim != null)
                {
                    claim.ClaimValue = JsonConvert.SerializeObject(value);
                }
                else
                {
                    _context.ApplicationUserClaims.Add(new ApplicationUserClaim
                    {
                        UserId = user.Id,
                        ClaimType = "GoogleAccessToken",
                        ClaimValue = JsonConvert.SerializeObject(value)
                    });
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"Cannot store Google Access Token for user '{key}', no such user found.");
            }
        }

        public async Task DeleteAsync<T>(ApplicationDbContext _context, string key)
        {
            ApplicationUser user =
                await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim = await _context.ApplicationUserClaims
                    .FirstOrDefaultAsync(x => x.UserId == user.Id
                                              && x.ClaimType == "GoogleAccessToken");
                if (claim != null)
                {
                    _context.ApplicationUserClaims.Remove(claim);
                    await _context.SaveChangesAsync();
                }
            }

            // TODO consider logging if there is nothing to delete
        }

        public async Task<T> GetAsync<T>(ApplicationDbContext _context, string key)
        {
            ApplicationUser user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim = await _context.ApplicationUserClaims
                    .FirstOrDefaultAsync(x => x.UserId == user.Id
                                              && x.ClaimType == "GoogleAccessToken");
                if (claim != null)
                {
                    return JsonConvert.DeserializeObject<T>(claim.ClaimValue);
                }

                return default(T);
            }

            throw new Exception($"Cannot retrieve Google Access Token for user '{key}', no such user found.");
        }

        public Task ClearAsync(ApplicationDbContext _context)
        {
            IQueryable<ApplicationUserClaim> claims =
                _context.ApplicationUserClaims.Where(x => x.ClaimType == "GoogleAccessToken");
            _context.ApplicationUserClaims.RemoveRange(claims);
            return Task.CompletedTask;
        }

        public async Task<string> GetRefreshToken(ApplicationDbContext _context, string key)
        {
            ApplicationUser user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(x => x.NormalizedUserName == key);
            if (user != null)
            {
                ApplicationUserClaim claim = await _context.ApplicationUserClaims
                    .FirstOrDefaultAsync(x => x.UserId == user.Id
                                              && x.ClaimType == "GoogleRefreshToken");
                return claim?.ClaimValue;
            }

            throw new Exception($"Cannot retrieve Google Access Token for user '{key}', no such user found.");
        }
    }
}