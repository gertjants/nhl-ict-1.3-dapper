using YamlDotNet.Serialization;

namespace DapperBeerClassroom.Helpers
{
    public static class YamlHelper
    {
        public static Dictionary<string, object> GetYamlObject(string filename)
        {
            var yaml = File.ReadAllText(filename);
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<Dictionary<string, object>>(yaml);
        }

        public static void WriteYamlFile(string filename, object obj)
        {
            var serializer = new SerializerBuilder()
                .Build();

            var yaml = serializer.Serialize(obj);
            // \n\n seems to be added by YamlDotNet to multiline commands. Removing it.
            File.WriteAllText($"/workspace/.github/workflows/{filename}.yml", yaml.Replace("\n\n", "\n"));
        }

    }
}