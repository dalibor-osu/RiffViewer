using System;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using static PrettyLogSharp.PrettyLogger;

namespace RiffViewer.UI;

public class Settings
{
    [JsonIgnore]
    private static Settings? _instance = null;

    [JsonIgnore]
    public static Settings Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }
            
            Log("Instance was null");
            
            InitializeNewSettings();

            return _instance ?? throw new NoNullAllowedException("Settings error");
        }
        private set => _instance = value;
    }


    public string LastOpenedFilePath { get; set; }

    public static void Save()
    {
        File.WriteAllText("./settings.json", JsonConvert.SerializeObject(Instance));
    }

    public static void TryLoad()
    {
        if (!File.Exists("./settings.json"))
        {
            InitializeNewSettings();
        }

        try
        {
            string json = File.ReadAllText("./settings.json");
            var settings = JsonConvert.DeserializeObject<Settings>(json);
            _instance = settings;
        }
        catch (Exception e)
        {
            Log("Failed to parse settings. Initializing new settings");
            InitializeNewSettings();
        }
    }

    public static void InitializeNewSettings()
    {
        _instance = new Settings();
        Save();
    }
}