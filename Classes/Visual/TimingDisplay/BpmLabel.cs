using Godot;
using System;

public partial class BpmLabel : Label
{
    public event EventHandler? DoubleClicked = null;

    public override void _GuiInput(InputEvent @event)
    {
        //base._GuiInput(@event);
        if (@event is InputEventMouseButton mouseEvent)
        {
            // Trigger on double-click (any button) or single middle mouse button click
            if ((mouseEvent.DoubleClick) ||
                (mouseEvent.ButtonIndex == MouseButton.Middle && mouseEvent.Pressed))
            {
                DoubleClicked?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
