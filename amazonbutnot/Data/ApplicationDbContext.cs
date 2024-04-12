﻿using amazonbutnot.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace amazonbutnot.Data;

public class ApplicationDbContext : IdentityDbContext<AspNetUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}