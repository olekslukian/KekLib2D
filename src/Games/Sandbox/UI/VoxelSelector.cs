using ImGuiNET;
using KekLib3D.Voxels;
using Microsoft.Xna.Framework;

namespace Sandbox.UI;

public class VoxelSelector(MenuController menuController, VoxelDataManager voxelDataManager)
{
    public ushort SelectedVoxelId { get; private set; } = 1;
    private readonly MenuController _menuController = menuController;
    private readonly VoxelDataManager _voxelDataManager = voxelDataManager;

    public void Draw()
    {
        if (_menuController.IsMenuShown)
        {
            var availableVoxels = _voxelDataManager.GetVoxelIdNameMap();

            ImGui.Begin("Voxel selector");
            ImGui.TextColored(new Vector4(1, 1, 0, 1).ToNumerics(), "Voxels");
            ImGui.BeginChild("Scrolling", new System.Numerics.Vector2(0));
            foreach (var (id, name) in availableVoxels)
            {
                if (ImGui.RadioButton($"{id}: {name}", active: id == SelectedVoxelId)) { SelectedVoxelId = id; }
            }
            ImGui.EndChild();

            ImGui.End();
        }
    }
}
