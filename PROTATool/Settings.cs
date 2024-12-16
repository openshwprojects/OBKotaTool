using System;
using System.IO;
using System.Text.Json;
using System.ComponentModel;
using System.Windows.Forms;

public class Settings : INotifyPropertyChanged
{
    private string apiKey;
    private string ips;
    private string url;

    public string ApiKey
    {
        get => apiKey;
        set
        {
            if (apiKey != value)
            {
                apiKey = value;
                OnPropertyChanged(nameof(ApiKey));
            }
        }
    }

    public string Ips
    {
        get => ips;
        set
        {
            if (ips != value)
            {
                ips = value;
                OnPropertyChanged(nameof(Ips));
            }
        }
    }

    public string Url
    {
        get => url;
        set
        {
            if (url != value)
            {
                url = value;
                OnPropertyChanged(nameof(Url));
            }
        }
    }

    private static readonly string FilePath = "settings.json";

    public static Settings Load()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }
        return new Settings();
    }

    public void Save()
    {
        string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        Save();
    }

    public void BindTextBox(TextBox textBox, string propertyName)
    {
        switch (propertyName)
        {
            case nameof(ApiKey):
                textBox.Text = ApiKey;
                textBox.TextChanged += (s, e) => ApiKey = textBox.Text;
                break;
            case nameof(Ips):
                textBox.Text = Ips;
                textBox.TextChanged += (s, e) => Ips = textBox.Text;
                break;
            case nameof(Url):
                textBox.Text = Url;
                textBox.TextChanged += (s, e) => Url = textBox.Text;
                break;
            default:
                throw new ArgumentException($"Property {propertyName} not found", nameof(propertyName));
        }
    }
}
