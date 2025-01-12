﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TodoApi.Identity;

namespace TodoApi.ServiceSetup;

public static class IdentitySetup
{
    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        // Identity
        builder.Services.AddDbContext<IdentityContext>(opts =>
        {
            opts.UseSqlServer(builder.Configuration["ConnectionStrings:TodoApiIdentity"],
                opts => opts.MigrationsAssembly("TodoApi")
                );
        });

        builder.Services.AddIdentity<ApiIdentityUser, IdentityRole>(opts =>
        {
            opts.SignIn.RequireConfirmedAccount = false;
        }).AddEntityFrameworkStores<IdentityContext>()
        .AddDefaultTokenProviders();
    }
}
