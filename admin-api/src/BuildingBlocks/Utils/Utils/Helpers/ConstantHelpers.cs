using System;
using System.Collections.Generic;

namespace Utils
{
    public static class CacheProfileName
    {
        public const string StaticFiles = nameof(StaticFiles);
    }
    public static class CorsPolicyName
    {
        public const string AllowAny = nameof(AllowAny);
        public const string AllowFrontEnd = nameof(AllowFrontEnd);
        public const string AllowThirdparty = nameof(AllowThirdparty);
    }
    public class AppConstants
    {
        public static string EnvironmentName = "development";
        public static Guid RootAppId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid TestAppId => new Guid("00000000-0000-0000-0000-000000000002");

    }
    public class OracleExecStatusConstants
    {
        public static string Success = "00";
        public static string Fail = "99";      
    }
    public class UserConstants
    {
        public static Guid AdministratorId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid UserId => new Guid("00000000-0000-0000-0000-000000000002");
        public static Guid GuestId => new Guid("00000000-0000-0000-0000-000000000003");
    }
    public class RoleConstants
    {
        public static Guid AdministratorId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid UserId => new Guid("00000000-0000-0000-0000-000000000002");
        public static Guid GuestId => new Guid("00000000-0000-0000-0000-000000000002");
    }

    public class RightConstants
    {
        public static Guid AccessAppId => new Guid("00000000-0000-0000-0000-000000000001");
        public static string AccessAppCode = "TRUY_CAP_HE_THONG";

        public static Guid DefaultAppId => new Guid("00000000-0000-0000-0000-000000000002");
        public static string DefaultAppCode = "TRUY_CAP_MAC_DINH";

        public static Guid FileAdministratorId => new Guid("00000000-0000-0000-0000-000000000003");
        public static string FileAdministratorCode = "QUAN_TRI_FILE";


        public static Guid PemissionId => new Guid("00000000-0000-0000-0000-000000000004");
        public static string PemissionCode = "PHAN_QUYEN";

    }
    public class MdmConstants
    {
        public static Guid PartitionDefaultId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid MetadataTemplateDefaultId => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid MetadataField1Id => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid MetadataField2Id => new Guid("00000000-0000-0000-0000-000000000002");
    }
    public class NavigationConstants
    {
        public static Guid SystemNav => new Guid("00000000-0000-0000-0000-000000000001");
        public static Guid RoleNav => new Guid("00000000-0000-0000-0000-000000000011");
        public static Guid RightNav => new Guid("00000000-0000-0000-0000-000000000021");
        public static Guid UserNav => new Guid("00000000-0000-0000-0000-000000000031");
        public static Guid PartitionNav => new Guid("00000000-0000-0000-0000-000000000041");
        public static Guid MetaTemplateNav => new Guid("00000000-0000-0000-0000-000000000051");
        public static Guid PermissionNav => new Guid("00000000-0000-0000-0000-000000000002");
        public static Guid NavNav => new Guid("00000000-0000-0000-0000-000000000012");
        public static Guid RMUNav => new Guid("00000000-0000-0000-0000-000000000022");
        public static Guid NodeNav => new Guid("00000000-0000-0000-0000-000000000003");
    }
}
