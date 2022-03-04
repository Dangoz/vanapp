using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;
using Server.Types;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Microsoft.AspNetCore.Identity;

namespace Server.Data
{
    public class UserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<SiteUser>> GetUsersAsync()
        {
            return await _context.Users
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(SiteUser user) 
        {
           _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<SiteUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
            .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<SiteUser>> GetMembersAsync()
        {
            // List<SiteUser> users = await _context.Users 
            //     .ToListAsync();

            // var user_objs = new List<UserType>();
            // foreach (var item in users)
            // {
            //     UserType user_obj = new UserType
            //     {
            //         id = item.id,
            //         age = item.age,
            //     };
            //     user_objs.Add(user_obj);
            // }

            return await _context.Users 
                .ToListAsync();
        }
    }
}