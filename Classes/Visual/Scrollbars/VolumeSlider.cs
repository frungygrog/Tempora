// Copyright 2024 https://github.com/kongehund
// 
// This file is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International (CC BY-NC-ND 4.0).
// You are free to:
// - Share, copy and redistribute the material in any medium or format
//
// Under the following terms:
// - Attribution - You must give appropriate credit, provide a link to the license, and indicate if changes were made.
// - NonCommercial - You may not use the material for commercial purposes.
// - NoDerivatives - If you remix, transform, or build upon the material, you may not distribute the modified material.
//
// Full license text is available at: https://creativecommons.org/licenses/by-nc-nd/4.0/legalcode

using System;
using Godot;
using Tempora.Classes.Utility;

namespace Tempora.Classes.Visual;

public partial class VolumeSlider : HScrollBar
{
	private int busIndex;
	[Export] public string BusName = null!;

	private Label? percentLabel;

	public override void _Ready()
	{
		busIndex = AudioServer.GetBusIndex(BusName);
		ValueChanged += OnValueChanged;

		// Create and configure the percent label
		percentLabel = new Label();
		percentLabel.Text = GetPercentText(Value);
		percentLabel.AddThemeFontOverride("font", GD.Load<Font>("res://Fonts/TorusBoldFont.tres"));
		percentLabel.AddThemeFontSizeOverride("font_size", 11);
		percentLabel.AddThemeColorOverride("font_color", new Color(1, 1, 1, 1));
		percentLabel.AddThemeColorOverride("font_outline_color", new Color(0, 0, 0, 1));
		percentLabel.AddThemeConstantOverride("outline_size", 6);
		percentLabel.HorizontalAlignment = HorizontalAlignment.Center;
		percentLabel.VerticalAlignment = VerticalAlignment.Top;
		percentLabel.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
		percentLabel.SizeFlagsVertical = SizeFlags.ShrinkCenter;
		percentLabel.MouseFilter = MouseFilterEnum.Ignore;
		AddChild(percentLabel);

		float invertedValue = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(busIndex));
		Value = invertedValue;

		UpdatePercentLabel();
	}

	private void OnValueChanged(double value)
	{
		AudioServer.SetBusVolumeDb(
			busIndex,
			Mathf.LinearToDb((float)value));
		SaveToSettings(value);
		UpdatePercentLabel();
	}

	private void SaveToSettings(double value)
	{
		switch (BusName)
		{
			case "Music":
				Settings.Instance.MusicVolumeNormalized = value;
				break;
			case "Metronome":
				Settings.Instance.MetronomeVolumeNormalized = value;
				break;
			case "Master":
				Settings.Instance.MasterVolumeNormalized = value;
				break;
		}
	}

	private string GetPercentText(double value)
	{
		return $"{Mathf.RoundToInt((float)(value * 100))}%";
	}

	private void UpdatePercentLabel()
	{
		if (percentLabel == null)
			return;

		// Update text
		percentLabel.Text = GetPercentText(Value);

		// Calculate the position of the grabber/handle
		float ratio = (float)((Value - MinValue) / (MaxValue - MinValue));
		float handleWidth = 16.0f; // Default Godot handle size, adjust if themed
		float x = ratio * (Size.X - handleWidth) + handleWidth / 2.0f;

		// Position the label below the handle
		float y = Size.Y + 2.0f; // 2px below the slider

		percentLabel.Position = new Vector2(x - percentLabel.Size.X / 2.0f, y);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		UpdatePercentLabel();
	}
}
