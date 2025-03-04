namespace ApiUtilities
{
    public class ApiUrls
    {
        //public static string BaseUrl = System.Environment.GetEnvironmentVariable("RPG_GAME_URL");
        public static string BaseUrl = "http://localhost:8080";
        //public static string SetUrl = BaseUrl + "/set";

        public static string RegisterUrl = BaseUrl + "/register";
        public static string LoginUrl = BaseUrl + "/login";
        public static string SaveUrl = BaseUrl + "/save";
        public static string QuesteUrl = BaseUrl + "/quest/";
        public static string QuestAddUrl = BaseUrl + "/quest";
        public static string QuestList = BaseUrl + "/questList/";
        public static string SaveData = BaseUrl + "/saveData";
    }
}
