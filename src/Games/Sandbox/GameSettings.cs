using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KekLib3D;

public sealed class GameSettings
{
    private static GameSettings _instance;
    public static GameSettings Instance => _instance ??= new GameSettings();

    public float Fov { get; set; } = 45f;
    public float MouseSensitivity { get; set; } = 50f;
    public float MovingSpeed { get; set; } = 8f;
    public int CrosshairSize { get; set; } = 20;
    public int CrosshairThickness { get; set; } = 2;
    public Vector3 CrosshairColor { get; set; } = new Vector3(255, 99, 71);

    public GameSettings() { }

    public static GameSettings FromFile(string fileName, ContentManager content)
    {
        GameSettings gameSettings = new();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Settings file not found at {filePath}. Using default settings.");

            return gameSettings;
        }

        string section = null;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var trimmed = line.Trim();

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith(';'))
                continue;

            if (trimmed.StartsWith(']') && trimmed.EndsWith(']'))
            {
                section = trimmed[1..^1];
            }
            else if (section == "Settings" && trimmed.Contains('='))
            {
                var parts = trimmed.Split('=', 2);
                var key = parts[0].Trim();
                var value = parts[1].Trim();

                switch (key.ToUpperInvariant())
                {
                    case "FOV":
                        if (float.TryParse(value, out var fov))
                            gameSettings.Fov = fov;
                        break;
                    case "MOUSE_SENSITIVITY":
                        if (float.TryParse(value, out var sensitivity))
                            gameSettings.MouseSensitivity = sensitivity;
                        break;
                    case "SPEED":
                        if (float.TryParse(value, out var speed))
                            gameSettings.MovingSpeed = speed;
                        break;
                    case "CROSSHAIR_SIZE":
                        if (int.TryParse(value, out var size))
                            gameSettings.CrosshairSize = size;
                        break;
                    case "CROSSHAIR_THICKNESS":
                        if (int.TryParse(value, out var thickness))
                            gameSettings.CrosshairThickness = thickness;
                        break;
                    case "CROSSHAIR_COLOR":
                        gameSettings.CrosshairColor = ParseRgbColor(value);
                        break;
                }
            }
        }

        return gameSettings;
    }

    private static Vector3 ParseRgbColor(string colorString)
    {
        try
        {
            var parts = colorString.Split(',');

            if (parts.Length == 3)
            {
                int r = int.Parse(parts[0].Trim());
                int g = int.Parse(parts[1].Trim());
                int b = int.Parse(parts[2].Trim());

                return new Vector3(r, g, b);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to parse color string '{colorString}': {ex.Message}");
        }

        return new Vector3(255, 99, 71);
    }

}
