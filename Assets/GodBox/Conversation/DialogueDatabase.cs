    using System.Collections.Generic;

namespace GodBox.Conversation
{
    public static class DialogueDatabase
    {
        public static List<string> Greetings = new List<string>
        {
            "Hello there.", "Hi!", "Greetings.", "Hey.", "Good day."
        };

        public static List<string> Partings = new List<string>
        {
            "Goodbye.", "See you.", "Bye!", "Farewell.", "Catch you later."
        };

        public static Dictionary<string, List<string>> Topics = new Dictionary<string, List<string>>
        {
            { "Weather", new List<string> { "Lovely weather today.", "Looks like rain.", "Too hot for me.", "Perfect breeze." } },
            { "Food", new List<string> { "I'm hungry.", "Ate a huge apple.", "Can't find any food.", "Food is scarce." } },
            { "News", new List<string> { "Did you hear?", "Nothing new happening.", "Everyone is so quiet.", "The world is changing." } }
        };

        public static string GetRandomGreeting() => Greetings[UnityEngine.Random.Range(0, Greetings.Count)];
        public static string GetRandomParting() => Partings[UnityEngine.Random.Range(0, Partings.Count)];
        
        public static string GetRandomReply(string topic)
        {
            if (Topics.ContainsKey(topic))
            {
                var list = Topics[topic];
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
            return "...";
        }

        public static string GetRandomTopic()
        {
            var keys = new List<string>(Topics.Keys);
            return keys[UnityEngine.Random.Range(0, keys.Count)];
        }
    }
}
