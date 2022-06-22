using Godot;
using System;
using System.Collections.Generic;

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

    private void PopulateTowerMenus(List<BaseTower> baseTowers)
    {
        // throw new NotImplementedException();
    }

    internal List<BaseTower> GenerateTowerSelection()
    {
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

    private void OnTowerSelection1ButtonDown()
    {
        Hide();
    }

    private void OnTowerSelection2ButtonDown()
    {
        Hide();
    }

    private void OnTowerSelection3ButtonDown()
    {
        Hide();
    }
}
