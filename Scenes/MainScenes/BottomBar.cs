using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BottomBar : TextureRect
{
    public Func<bool>? OnPausePlayPressedEvent;

    [Signal]
    public event Action<object, AC.TowerType>? OnBuildBtnDown;
    public Func<object, AC.TowerType, bool>? OnBuildBtnUp;

    [Export]
    private NodePath? _TowerOverflowTextPath;
    private Label? _TowerOverflowText;

    [Export]
    List<NodePath>? _buildButtonsPath;
    private List<TextureButton> _buildButtons = new List<TextureButton>();
    private List<AC.TowerType> _towerBank = new List<AC.TowerType>
    {
        AC.TowerType.GunTurret,
        AC.TowerType.GunTurret,
        AC.TowerType.DualGunTurret
    };

    internal void AddTower(AC.TowerType towerType, bool addAtBeggining = false)
    {
        if (!addAtBeggining)
        {
            _towerBank.Add(towerType);
        }
        else
        {
            _towerBank.Insert(0, towerType);
        }
        UpdateAllButtons();
    }

    private void UpdateAllButtons()
    {
        var ac = GetNode<AC>("/root/AC");

        for (int i = 0; i < _buildButtons.Count; i++)
        {
            if (i < _towerBank.Count)
            {
                _buildButtons[i].GetNode<TextureRect>("Icon").Texture = ac.GetTower(_towerBank[i])
                    .GetNode<Sprite2D>("Turret")
                    .Texture;
                _buildButtons[i].Disabled = false;
            }
            else
            {
                _buildButtons[i].GetNode<TextureRect>("Icon").Texture = null;
                _buildButtons[i].Disabled = true;
            }
        }
        var overflow = _towerBank.Count - _buildButtons.Count;
        if (overflow <= 0)
        {
            _TowerOverflowText!.Text = $"+{overflow}";
            _TowerOverflowText!.Visible = false;
        }
        else if (overflow > 9)
        {
            _TowerOverflowText!.Text = $"++";
            _TowerOverflowText!.Visible = true;
        }
        else
        {
            _TowerOverflowText!.Text = $"+{overflow}";
            _TowerOverflowText!.Visible = true;
        }
    }

    public sealed override void _Ready()
    {
        _TowerOverflowText = GetNode<Label>(_TowerOverflowTextPath);

        foreach (NodePath path in _buildButtonsPath!)
        {
            TextureButton temp = GetNode<TextureButton>(path);
            _buildButtons.Add(temp);
        }

        UpdateAllButtons();
    }

    private void OnPausePlayPressed()
    {
        if (OnPausePlayPressedEvent?.Invoke() == true)
        {
            return;
        }
        GetTree().Paused = !GetTree().Paused;
    }

    #region  build button events
    private void BtnDown(int i)
    {
        OnBuildBtnDown?.Invoke(this, _towerBank[i]);
        _buildButtons[i].GetNode<TextureRect>("Icon").Texture = null;
    }

    private void BtnUp(int i)
    {
        if ((bool)OnBuildBtnUp?.Invoke(this, _towerBank[i])!)
        {
            _towerBank.RemoveAt(i);
            UpdateAllButtons();
        }
        else
        {
            GD.Print("here");
            var ac = GetNode<AC>("/root/AC");
            _buildButtons[i].GetNode<TextureRect>("Icon").Texture = ac.GetTower(_towerBank[i])
                .GetNode<Sprite2D>("Turret")
                .Texture;
        }
    }

    private void OnBuildBtn0Down()
    {
        BtnDown(0);
    }

    private void OnBuildBtn1Down()
    {
        BtnDown(1);
    }

    private void OnBuildBtn2Down()
    {
        BtnDown(2);
    }

    private void OnBuildBtn3Down()
    {
        BtnDown(3);
    }

    private void OnBuildBtn0Up()
    {
        BtnUp(0);
    }

    private void OnBuildBtn1Up()
    {
        BtnUp(1);
    }

    private void OnBuildBtn2Up()
    {
        BtnUp(2);
    }

    private void OnBuildBtn3Up()
    {
        BtnUp(3);
    }
    #endregion
}
