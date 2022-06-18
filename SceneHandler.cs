using Godot;
using System;
using System.Linq;

public partial class SceneHandler : Node
{
    private MainMenu? _mainMenu;
    private AC.SceneName _currentSceneName = AC.SceneName.MainMenu;
    private Node? _currentScene = null;

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

        _currentSceneName = AC.SceneName.GameScene;
        var gameScene = GetNode<AC>("/root/AC")
            .GetPackedScene(AC.SceneName.GameScene)
            .Instantiate();

        AddChild(gameScene);
    }

    // public sealed override void _UnhandledInput(InputEvent inputEvent)
    // {
    //     if (inputEvent.IsActionPressed("ui_cancel"))
    //     {
    //         GD.Print("here");
    //     }
    // }

    private void OnSettingsPressed(Node obj) { }

    private void OnShopPressed(Node sender) { }

    private void OnAboutPressed(Node sender) { }

    private void OnQuitPressed(Node sender)
    {
        GetTree().Quit();
    }
}
