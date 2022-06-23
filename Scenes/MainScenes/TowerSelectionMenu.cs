using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TowerSelectionMenu : TextureRect
{
    private BottomBar? _bottomBar = null;
    private Random rng = new Random();

    internal void InitialTowerSelection(BottomBar bottomBar)
    {
        if (_bottomBar is null)
        {
            _bottomBar = bottomBar;
        }

        PopulateTowerMenus(GenerateTowerSelection());
    }

    private void PopulateTowerMenus(List<BaseTower> towers)
    {
        for (int i = 0; i < towers.Count; i++)
        {
            var vBox = GetNode<VBoxContainer>($"VBoxContainer/TowerSelection{i}/VBoxContainer");
            switch (towers[i].Rarity)
            {
                case BaseTower.TowerRarity.Common:
                    break;
                case BaseTower.TowerRarity.Uncommon:
                    break;
                case BaseTower.TowerRarity.Rare:
                    break;
                case BaseTower.TowerRarity.Legendary:
                    break;
                default:
                    break;
            }
            // vBox.GetNode<TextureRect>("RarityBar").Modulate = null;
            vBox.GetNode<Label>("TowerType").Text = "abc";
            var scale = 1.9f;
            towers[i].Scale = new Vector2(scale, scale);
            vBox.GetNode("Base/Center").AddChild(towers[i]);
        }

        Show();
    }

    internal List<BaseTower> GenerateTowerSelection()
    {
        Show();
        List<BaseTower> towers = new List<BaseTower>();

        while (towers.Count < 3)
        {
            var randomNumber = rng.Next(0, Enum.GetNames(typeof(AC.TowerType)).Length - 1);

            var ac = GetNode<AC>("/root/AC");
            var tower = ac.GetTower((AC.TowerType)randomNumber);
            randomNumber = rng.Next(0, Enum.GetNames(typeof(BaseEnemy.EnemyRarity)).Length - 1);
            if ((int)tower.Rarity <= randomNumber)
            {
                towers.Add(tower);
            }
            else
            {
                tower.QueueFree();
            }
        }

        return towers;
    }

    private void OnTowerSelection0ButtonDown()
    {
        Hide();
    }

    private void OnTowerSelection1ButtonDown()
    {
        Hide();
    }

    private void OnTowerSelection2ButtonDown()
    {
        Hide();
    }
}
