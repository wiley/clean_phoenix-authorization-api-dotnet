namespace Authorization.Consumer.Domain
{
    public enum GroupTypeEnumV4
    {
        LEARNER = 1,
        ADMIN = 2
    }

    //Class to transform the Enum into a dictionary, for ease of access to values/keys
    public class GroupTypeEnumV4Dict
    {
        public static Dictionary<int, string> EnumToDictByKey()
        {
            return Enum.GetValues(typeof(GroupTypeEnumV4))
               .Cast<GroupTypeEnumV4>()
               .ToDictionary(t => (int)t, t => t.ToString());
        }

        public static Dictionary<string, int> EnumToDictByValue()
        {
            return Enum.GetValues(typeof(GroupTypeEnumV4))
               .Cast<GroupTypeEnumV4>()
               .ToDictionary(t => t.ToString(), t => (int)t);
        }
    }
}
