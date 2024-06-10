using Godot;
using System;
using System.Linq;

public partial class SceneHandler : Node
{
    private AC.SceneName _currentSceneName = AC.SceneName.MainMenu;
    private Node? _currentScene = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        SceneChanger(AC.SceneName.MainMenu);
    }

    private void SceneChanger(AC.SceneName sceneName)
    {
        var ac = GetNode<AC>("/root/AC");
        var newScene = ac.GetPackedScene(sceneName).Instantiate();

        switch (sceneName)
        {
            case AC.SceneName.MainMenu:
                var mainMenu = newScene as MainMenu;
                mainMenu!.OnNewGamePressed += OnNewGamePressed;
                mainMenu.OnSettingsPressed += OnSettingsPressed;
                mainMenu.OnShopPressed += OnShopPressed;
                mainMenu.OnAboutPressed += OnAboutPressed;
                mainMenu.OnQuitPressed += OnQuitPressed;
                break;
            case AC.SceneName.GameScene:
                break;
            default:
                break;
        }

        _currentSceneName = sceneName;
        AddChild(newScene);
        _currentScene?.QueueFree();
        _currentScene = newScene;
    }

    public void OnNewGamePressed(Node sender)
    {
        GD.Print();
        SceneChanger(AC.SceneName.GameScene);
    }

    public sealed override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("ui_cancel"))
        {
            if (_currentSceneName == AC.SceneName.GameScene)
            {
                SceneChanger(AC.SceneName.MainMenu);
                GetViewport().SetInputAsHandled();
            }
        }
    }

    private void OnSettingsPressed(Node obj) { }

    private void OnShopPressed(Node sender) { }

    private void OnAboutPressed(Node sender) { }

    private void OnQuitPressed(Node sender)
    {
        GetTree().Quit();
    }
}
