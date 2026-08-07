using System.Text;
using ITHelpDesk.API.Configuration;
using ITHelpDesk.API.Data;
using ITHelpDesk.API.Helpers;
using ITHelpDesk.API.Interfaces;
using ITHelpDesk.API.Repositories;
using ITHelpDesk.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// DATABASE
// =====================================================

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DefaultConnection"
            )
        )
);

// =====================================================
// CORS
// =====================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowReact",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    );
});

// =====================================================
// CONTROLLERS AND SWAGGER
// =====================================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter JWT Bearer token"
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type =
                            ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        }
    );
});

// =====================================================
// AUTHENTICATION AND AUTHORIZATION
// =====================================================

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme
    )
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration[
                        "Jwt:Issuer"
                    ],

                ValidAudience =
                    builder.Configuration[
                        "Jwt:Audience"
                    ],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration[
                                "Jwt:Key"
                            ]!
                        )
                    )
            };
    });

builder.Services.AddAuthorization();

// =====================================================
// AUTHENTICATION SERVICES
// =====================================================

builder.Services.AddScoped<
    IUserRepository,
    UserRepository>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

builder.Services.AddScoped<JwtHelper>();

builder.Services.AddScoped<
    IEmailService,
    EmailService>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(
        "EmailSettings"
    )
);

// =====================================================
// TICKET SERVICES
// =====================================================

builder.Services.AddScoped<
    ITicketRepository,
    TicketRepository>();

builder.Services.AddScoped<
    ITicketService,
    TicketService>();

builder.Services.AddScoped<
    IActivityLogRepository,
    ActivityLogRepository>();

builder.Services.AddScoped<
    ITicketWorkLogRepository,
    TicketWorkLogRepository>();

// =====================================================
// ATTACHMENT SERVICES
// =====================================================

builder.Services.AddScoped<
    ITicketAttachmentRepository,
    TicketAttachmentRepository>();

builder.Services.AddScoped<
    ITicketAttachmentService,
    TicketAttachmentService>();

// =====================================================
// ASSIGNMENT SERVICES
// =====================================================

builder.Services.AddScoped<
    ITicketAssignmentRepository,
    TicketAssignmentRepository>();

builder.Services.AddScoped<
    ITicketAssignmentService,
    TicketAssignmentService>();

// =====================================================
// LOOKUP SERVICES
// =====================================================

builder.Services.AddScoped<
    ICategoryRepository,
    CategoryRepository>();

builder.Services.AddScoped<
    ICategoryService,
    CategoryService>();

builder.Services.AddScoped<
    IPriorityRepository,
    PriorityRepository>();

builder.Services.AddScoped<
    IPriorityService,
    PriorityService>();

builder.Services.AddScoped<
    IStatusRepository,
    StatusRepository>();

builder.Services.AddScoped<
    IStatusService,
    StatusService>();









builder.Services.AddScoped<
    IDashboardRepository,
    DashboardRepository
>();

builder.Services.AddScoped<
    IDashboardService,
    DashboardService
>();
var app = builder.Build();

// =====================================================
// HTTP PIPELINE
// =====================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReact");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();