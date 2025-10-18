using System.Numerics;
using ImGuiNET;
using KekLib3D.Voxels;

namespace Sandbox.UI;

public class VoxelSelector(UIController uiController, VoxelDataManager voxelDataManager)
{
    public ushort SelectedVoxelId { get; private set; } = 1;
    private readonly UIController _uiController = uiController;
    private readonly VoxelDataManager _voxelDataManager = voxelDataManager;

    public void Draw()
    {
        if (_uiController.IsMenuShown)
        {
            var availableVoxels = _voxelDataManager.GetVoxelIdNameMap();

            ImGui.Begin("Voxel selector");
            ImGui.TextColored(new Vector4(1, 1, 0, 1), "Voxels");
            ImGui.BeginChild("Scrolling", new Vector2(0));
            foreach (var (id, name) in availableVoxels)
            {
                if (ImGui.RadioButton($"{id}: {name}", active: id == SelectedVoxelId)) { SelectedVoxelId = id; }
            }
            ImGui.EndChild();

            ImGui.End();
        }
    }
}
