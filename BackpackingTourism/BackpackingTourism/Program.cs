using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.OData;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region Config Odata
builder.Services.AddControllers().AddOData(opt =>
{
    opt.Count().Filter().Expand().Select().OrderBy().SetMaxTop(5000).AddRouteComponents("odata", GetEdmModel());
    opt.EnableQueryFeatures();
    opt.TimeZone = TimeZoneInfo.Utc;
}).AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
#endregion
builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:ValidAudience"],
        ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        RequireExpirationTime = false
    };
    options.Events = new JwtBearerEvents()
    {
        OnForbidden = ctx =>
        {
            var message = /*SingleResultMessage.Fail(new Exception())*/ "";
            var result = JsonSerializer.Serialize(message, message.GetType(), new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.IgnoreCycles });
            return ctx.Response.WriteAsync(result);
        }
    };
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDirectoryBrowser();

#region Config Cors Domain
builder.Services.AddCors(options => options.AddPolicy(
    "AllowInternal",
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowedToAllowWildcardSubdomains()
    ));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost
});

// Use odata route debug, /$odata
app.UseODataRouteDebug();
// Add OData /$query middleware
app.UseODataQueryRequest();
// Add the OData Batch middleware to support OData $Batch
app.UseODataBatching();

app.UseCors("AllowInternal");

app.UseAuthentication();
app.UseAuthorization();
//app.UseExceptionFormatter();
app.MapControllers();
app.Run();
IEdmModel GetEdmModel()
{
    var odataBuilder = new ODataConventionModelBuilder();
    //odataBuilder.EntityType<OrganizationDto>().HasKey(entity => new { entity.Id });
   

    //odataBuilder.EntitySet<OrganizationLevelDto>("OrganizationLevel");

    #region Organization 
    //odataBuilder.EntitySet<OrganizationDto>("Organization");
    //odataBuilder.EntityType<OrganizationDto>().Collection.Action("CreateUnit").ReturnsFromEntitySet<OrganizationDto>("Organization");
    //odataBuilder.EntityType<OrganizationDto>().Action("UpdateUnit").ReturnsFromEntitySet<OrganizationDto>("Organization");
    //odataBuilder.EntityType<OrganizationDto>().Function("UnitDetails").ReturnsFromEntitySet<OrganizationDto>("Organization");
    #endregion

    return odataBuilder.GetEdmModel();
}