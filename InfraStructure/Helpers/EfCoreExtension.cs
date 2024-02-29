using Core.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static InfraStructure.Enums.DbEnum;

namespace InfraStructure.Helpers
{
    public static class EfCoreExtension
    {
        public static async Task<T> AddUserAndTimestamp<T>(this UserManager<ApplicationUser> _userManager, T entity, ClaimsPrincipal User, DbActionEnum action) where T : new()
        {
            switch (action)
            {
                case DbActionEnum.Create:
                    var user = await _userManager.GetUserAsync(User);
                    SetValue(entity,"CreatedAt", DateTime.Now);
                    SetValue(entity,"CreatedBy", _userManager.GetUserId(User));
                    SetValue(entity,"UpdatedAt", DateTime.Now);
                    break;
                case DbActionEnum.Update:
                    SetValue(entity,"UpdatedAt", DateTime.Now);
                    SetValue(entity,"UpdatedBy", _userManager.GetUserId(User));
                    break;
                default: break;
            }
            return entity;
        }
        public static void SetValue<T>(this T sender, string propertyName, object value)
        {
            var propertyInfo = sender.GetType().GetProperty(propertyName);

            if (propertyInfo is null) return;

            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(sender, Enum.Parse(propertyInfo.PropertyType, value.ToString()!));
            }
            else
            {
                var safeValue = (value == null) ? null : Convert.ChangeType(value, type);
                propertyInfo.SetValue(sender, safeValue, null);
            }
        }
    }
}
