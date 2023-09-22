using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Translation.Models
{

    public sealed class ConfigCache
    {
        private static ConfigCache _instance = null; // private instance of the cache
        private static readonly object _lock = new object(); // lock object for thread safety

        public static ConfigCache Instance // public instance of the cache
        {
            get
            {
                lock (_lock) // thread-safety lock 
                {
                    if (_instance == null)
                    {
                        _instance = new ConfigCache(); // create new instance of the cache
                    }
                    return _instance;
                }
            }
        }

        private readonly string _connectionString = "connection string of the database"; // Replace with actual connection string
        private List<Config> _configCache;

        private ConfigCache() // private constructor to prevent direct instantiation of the cache
        {
            _configCache = new List<Config>();
            LoadConfigValues(); // load ConfigValues into the cache on startup
        }

        private void LoadConfigValues()
        {
            string commandText = "SELECT ConfigKey, ConfigValue FROM Config"; // Replace ConfigTable with actual table name
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ConfigModel config = new ConfigModel()
                        {
                            ConfigKey = reader.GetString(0), // Get ConfigKey value from the first column of the reader
                            ConfigValue = reader.GetString(1) // Get ConfigValue value from the second column of the reader
                        };
                        _configCache.Add(config);
                    }
                }
            }
        }

        public string GetValueFromKey(string configKey)
        {
            ConfigModel config = _configCache.FirstOrDefault(x => x.ConfigKey == configKey);
            if (config == null)
            {
                throw new ArgumentException($"ConfigKey: {configKey} not found");
            }
            return config.ConfigValue;
        }
    }
}
