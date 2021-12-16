namespace API.Extensions
{
    using global::API.BussinessLogic;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// <see cref="IServiceCollection"/> extension methods add project services.
    /// </summary>
    /// <remarks>
    /// AddSingleton - Only one instance is ever created and returned.
    /// AddScoped - A new instance is created and returned for each request/response cycle.
    /// AddTransient - A new instance is created and returned each time.
    /// </remarks>
    public static class ProjectServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services) => services
            .AddSingleton<IApplicationHandler, ApplicationHandler>()
            .AddSingleton<IFunctionsHandler, FunctionsHandler>()
            .AddSingleton<IAuthHandler, AuthHandler>()
            .AddSingleton<IUserHandler, UserHandler>()
            .AddSingleton<IUserPermisionHandler, UserPermisionHandler>()
            .AddSingleton<IFuncPermsHandler, FuncPermsHandler>()
            .AddSingleton<IUserDepartsHandler, UserDepartsHandler>()
            .AddSingleton<IRolesHandler, RoleHandler>()
            .AddSingleton<IUserRolesHandler, UserRolesHandler>()
            .AddSingleton<IDepartmentsHandler, DepartmentsHandler>()
            .AddSingleton<IPermissionsHandler, PermissionsHandler>()
            .AddSingleton<IRolePermsHandler, RolePermsHandler>()
            .AddSingleton<IFuncChanelsHandler, FuncChanelsHandler>()
            .AddSingleton<IUserContactHandler, UserContactHandler>()
            .AddSingleton<IContactsHandler, ContactsHandler>()
            .AddSingleton<ILogActionsHandler, LogActionsHandler>()
            .AddSingleton<IUserBiometricHandler, UserBiometricHandler>()
            .AddSingleton<IChanelsHandler, ChanelsHandler>()
            .AddSingleton<IUserGroupHandler, UserGroupHandler>()
            .AddSingleton<IATM_UserPermisionHandler, ATM_UserPermisionHandler>()
            .AddSingleton<ISAAreasHandler, SAAreasHandler>();
    }
}