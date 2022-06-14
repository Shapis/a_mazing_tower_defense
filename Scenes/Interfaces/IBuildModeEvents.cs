using Godot;
using System;

interface IBuildModeEvents
{
    void OnBuildModeStarted(object sender);
    void OnBuildModeEnded(object sender);
}