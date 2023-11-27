using Microsoft.EntityFrameworkCore;
using Passflow.Domain;
using Passflow.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Passflow.Infrastructure.Database;
public static class PassflowDbContextSeeder
{
	public const string DefaultUserPassword = "Qwerty123$";
	public const string DefaultAdminPassword = "Qwerty123$";


	public static User DefaultAdmin { get; private set; } = null!;

	public static User DefaultUser { get; private set; } = null!;


	public static async Task SeedDataAsync(this PassflowDbContext context)
	{
		User defaultAdmin = new()
		{
			UserName = "Admin@gmail.com",
			Email = "Admin@gmail.com",
			PasswordHash = PasswordEncrypter.HashPassword(DefaultAdminPassword),
			IsAdmin = true
		}, defaultUser = new()
		{
			UserName = "User@gmail.com",
			Email = "User@gmail.com",
			PasswordHash = PasswordEncrypter.HashPassword(DefaultUserPassword),
			IsAdmin = false
		};
		await SeedEntityAsync(context, defaultAdmin, defaultUser);
		DefaultAdmin = defaultAdmin;
		DefaultUser = defaultUser;
	}
	private static async Task SeedEntityAsync<TEntity>(DbContext context, params TEntity[] data) where TEntity : class
	{
		var set = context.Set<TEntity>();
		if (!await set.AnyAsync())
		{
			await set.AddRangeAsync(data);
			await context.SaveChangesAsync();
		}
	}
}
