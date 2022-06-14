using Godot;
using System;
using System.Linq;


public partial class SceneHandler : Node
{
    private MainMenu? _mainMenu;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _mainMenu = GetChildren().OfType<MainMenu>().First();
        _mainMenu.OnNewGamePressed += OnNewGamePressed;
        _mainMenu.OnSettingsPressed += OnSettingsPressed;
        _mainMenu.OnShopPressed += OnShopPressed;
        _mainMenu.OnAboutPressed += OnAboutPressed;
        _mainMenu.OnQuitPressed += OnQuitPressed;
    }

    public void OnNewGamePressed(Node sender)
    {

        _mainMenu?.QueueFree();

        var gameScene = GetNode<AC>("/root/AC").GetPackedScene(AC.SceneName.GameScene).Instantiate();

        AddChild(gameScene);
    }


    private void OnSettingsPressed(Node obj)
    {

    }


    private void OnShopPressed(Node sender)
    {

    }

    private void OnAboutPressed(Node sender)
    {

    }

    private void OnQuitPressed(Node sender)
    {
        GetTree().Quit();
    }
}