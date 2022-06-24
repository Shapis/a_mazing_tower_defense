using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TowerSelectionMenu : TextureRect
{
    private BottomBar? _bottomBar = null;
    private Random rng = new Random();
    private AC? _assortedCatalog = null;
    private List<BaseTower> _towers = new List<BaseTower>();

    internal void InitialTowerSelection(BottomBar bottomBar)
    {
        _assortedCatalog = GetNode<AC>("/root/AC");
        if (_bottomBar is null)
        {
            _bottomBar = bottomBar;
        }
        GenerateTowerSelection();
    }

    private void PopulateTowerMenus()
    {
        for (int i = 0; i < _towers.Count; i++)
        {
            var vBox = GetNode<VBoxContainer>($"VBoxContainer/TowerSelection{i}/VBoxContainer");
            switch (_towers[i].Rarity)
            {
                case BaseTower.TowerRarity.Common:
                    vBox.GetNode<TextureRect>("RarityBar").Modulate = _assortedCatalog!.GetColor(
                        AC.ColorPalette.Silver
                    );
                    break;
                case BaseTower.TowerRarity.Uncommon:
                    vBox.GetNode<TextureRect>("RarityBar").Modulate = _assortedCatalog!.GetColor(
                        AC.ColorPalette.Green
                    );
                    break;
                case BaseTower.TowerRarity.Rare:
                    vBox.GetNode<TextureRect>("RarityBar").Modulate = _assortedCatalog!.GetColor(
                        AC.ColorPalette.Blue
                    );
                    break;
                case BaseTower.TowerRarity.Legendary:
                    vBox.GetNode<TextureRect>("RarityBar").Modulate = _assortedCatalog!.GetColor(
                        AC.ColorPalette.Orange
                    );
                    break;
                default:
                    break;
            }
            var str = _towers[i].TowerType.ToString();
            str = string.Concat(str.Select(x => Char.IsUpper(x) ? " " + x : x.ToString()))
                .TrimStart(' ');
            vBox.GetNode<Label>("TowerType").Text = str;
            vBox.GetNode<Label>("Description").Text = _towers[i].Description;
            var scale = 1.9f;
            _towers[i].Scale = new Vector2(scale, scale);
            vBox.GetNode("Base/Center").AddChild(_towers[i]);
        }

        Show();
    }

    internal void GenerateTowerSelection()
    {
        while (_towers.Count < 3)
        {
            var randomNumber = rng.Next(0, Enum.GetNames(typeof(AC.TowerType)).Length - 1);

            var ac = GetNode<AC>("/root/AC");
            var tower = ac.GetTower((AC.TowerType)randomNumber);
            randomNumber = rng.Next(0, Enum.GetNames(typeof(BaseEnemy.EnemyRarity)).Length - 1);
            if ((int)tower.Rarity <= randomNumber)
            {
                _towers.Add(tower);
            }
            else
            {
                tower.QueueFree();
            }
        }

        PopulateTowerMenus();
    }

    private void FreeAllTowers()
    {
        foreach (var item in _towers)
        {
            item.QueueFree();
        }
        _towers.Clear();
    }

    private void AddTowerToBottomBar(int i)
    {
        _bottomBar!.AddTower(_towers[i].TowerType);
        FreeAllTowers();
    }

    private void OnTowerSelection0ButtonDown()
    {
        AddTowerToBottomBar(0);
        Hide();
    }

    private void OnTowerSelection1ButtonDown()
    {
        AddTowerToBottomBar(1);
        Hide();
    }

    private void OnTowerSelection2ButtonDown()
    {
        AddTowerToBottomBar(2);
        Hide();
    }
}
