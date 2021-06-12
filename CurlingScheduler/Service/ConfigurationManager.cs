using CurlingScheduler.Model;
using Newtonsoft.Json;
using System.IO;

namespace CurlingScheduler.Service
{
    public class ConfigurationManager
    {
        public void SaveConfiguration(
            string filename,
            Configuration configuration)
        {
            var json = JsonConvert.SerializeObject(configuration);

            File.WriteAllText(filename, json);
        }

        public Configuration LoadConfiguration(
            string filename)
        {
            try
            {
                var json = File.ReadAllText(filename);

                return JsonConvert.DeserializeObject<Configuration>(json);
            }
            catch
            {
                return new Configuration();
            }
        }
    }
}