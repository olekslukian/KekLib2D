using System;
using ImGuiNET;
using KekLib2D.Core.Base;
using Microsoft.Xna.Framework;

namespace Sandbox.Scenes;

public class MainMenuScene(Game1 game) : BaseScene(game)
{
    private readonly Game1 _game = game;
    private NewMapSettings _newMapSettings = new();

    public override void Initialize()
    {
        base.Initialize();

        _game.IsMouseVisible = true;
    }

    public override void LoadContent()
    {
    }

    public override void Draw(GameTime gameTime)
    {
        ImGui.SetNextWindowPos(new System.Numerics.Vector2(_game.Window.ClientBounds.Width / 2, _game.Window.ClientBounds.Height / 2), ImGuiCond.Always, new System.Numerics.Vector2(0.5f, 0.5f));

        if (ImGui.Begin("Voxel Editor", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoMove))
        {
            if (ImGui.Button("New Map"))
            {
                ImGui.OpenPopup("New Map Settings");
            }

            if (ImGui.Button("Load Map"))
            {
                // TODO: Show file picker
            }

            if (ImGui.BeginPopupModal("New Map Settings"))
            {
                string name = _newMapSettings.Name;
                int width = _newMapSettings.Width;
                int height = _newMapSettings.Height;

                if (ImGui.InputText("Name", ref name, 100))
                {
                    _newMapSettings.Name = name;
                }
                if (ImGui.InputInt("Width", ref width))
                {
                    _newMapSettings.Width = width;
                }

                if (ImGui.InputInt("Height", ref height))
                {
                    _newMapSettings.Height = height;
                }

                if (ImGui.Button("Create"))
                {
                    // TODO: Add functionality to start a new map from Game class
                    // _game.StartNewMap(_newMapSettings);
                    ImGui.CloseCurrentPopup();
                }

                ImGui.SameLine();

                if (ImGui.Button("Cancel"))
                {
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndPopup();
            }
        }
        ImGui.End();
    }

    public override void UnloadContent()
    {
        base.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
        return;
    }

    public override void Dispose()
    {
        base.Dispose();
        GC.SuppressFinalize(this);
    }

    ~MainMenuScene() => Dispose();
}

public class NewMapSettings
{
    public string Name { get; set; } = "new_map";
    public int Width { get; set; } = 100;
    public int Height { get; set; } = 100;
}
