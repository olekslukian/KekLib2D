using System.Linq;
using System.Numerics;
using ImGuiNET;
using KekLib3D.Voxels;

namespace Sandbox.UI;

public class VoxelSelector
{
    public string SelectedVoxelId { get; private set; }
    private readonly UIController _uiController;
    private readonly VoxelDataManager _voxelDataManager;

    public VoxelSelector(UIController uiController, VoxelDataManager voxelDataManager)
    {
        _uiController = uiController;
        _voxelDataManager = voxelDataManager;

        SelectedVoxelId = _voxelDataManager.GetVoxelIds().FirstOrDefault();
    }

    public void Draw()
    {
        if (_uiController.IsMenuShown)
        {
            var availableVoxels = _voxelDataManager.GetVoxelIds();

            ImGui.Begin("Voxel selector");
            ImGui.TextColored(new Vector4(1, 1, 0, 1), "Voxels");
            ImGui.BeginChild("Scrolling", new Vector2(0));
            foreach (var id in availableVoxels)
            {
                if (ImGui.RadioButton($"{id}", active: id == SelectedVoxelId)) { SelectedVoxelId = id; }
            }
            ImGui.EndChild();

            ImGui.End();
        }
    }
}
