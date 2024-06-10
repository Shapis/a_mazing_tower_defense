using Godot;
using System;

public partial class MainMenu : Control
{
	 public event Action<Node> OnNewGamePressed;
	 public event Action<Node> OnSettingsPressed;
	 public event Action<Node> OnShopPressed;
	 public event Action<Node> OnAboutPressed;
	 public event Action<Node> OnQuitPressed;

	public void OnBtnNewGamePressed()
	{
		OnNewGamePressed?.Invoke(this);
	}

	public void OnBtnSettingsPressed()
	{
		OnSettingsPressed?.Invoke(this);
	}

	public void OnBtnShopPressed()
	{
		OnShopPressed?.Invoke(this);
	}

	public void OnBtnAboutPressed()
	{
		OnAboutPressed?.Invoke(this);
	}

	public void OnBtnQuitPressed()
	{
		OnQuitPressed?.Invoke(this);
	}
}
