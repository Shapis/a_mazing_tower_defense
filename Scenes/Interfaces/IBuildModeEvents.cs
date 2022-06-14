using Godot;
using System;

interface IBuildModeEvents
{
    void OnBuildModeStarted(object sender, AC.TowerName towerName); // Invoked from GameScene.cs
    Vector2i? OnBuildModeEnded(object sender, AC.TowerName towerName); // Invoked from GameScene.cs
}