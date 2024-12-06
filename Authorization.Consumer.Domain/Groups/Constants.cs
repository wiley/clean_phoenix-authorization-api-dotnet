namespace Groups.Domain
{
    public enum ContextEnum
    {
        All = 0,
        Catalyst = 1,
        Epic = 2,
        LPI = 3,
        SGP = 4
    }

    public enum GroupMemberEnum
    {
        Member = 1,
        Viewer = 2
    }

    public enum GroupStatusEnum
    {
        Active = 1,
        Archived = 2
    }

    public enum GroupVisibilityEnum
    {
        Hidden = 1,
        Owner = 2,
        AllMembers = 3
    }

    public enum GroupMemberSearchEnum
    {
        All = 0,
        Member = 1,
        Viewer = 2
    }

    public enum GroupStatusSearchEnum
    {
        All = 0,
        Active = 1,
        Archived = 2
    }

    public enum GroupVisibilitySearchEnum
    {
        All = 0,
        Hidden = 1,
        Owner = 2,
        AllMembers = 3
    }

    public static class APITokens
    {
        public const string GroupsAPIToken = "";
        public const string CompanyAPIToken = "";
    }

    public class HealthResults
    {
        public const string OK = "OK";
        public const string Unavailable = "Unavailable";
        public const string Fail = "Fail";
    }

    public class DependenciesTypes
    {
        public const string MySql = "MySql";
        public const string CompanyAPI = "CompanyAPI";
    }

    public class Constants
    {
        public const int GroupsSearchMaxSize = 100000;
        public const int GroupsMembersMinSize = 2;
    }
}