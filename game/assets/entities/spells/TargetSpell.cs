using Godot;
using System;
using System.Threading.Tasks;

public abstract partial class TargetSpell : Spell
{
    [Export] private float _speed;
    
    protected bool HasToMove;

    private Enemy _target;
    
    protected bool IsApplied;
    
    public override void _Process(double delta)
    {
        if (HasToMove == false)
        {
            return;
        }
		
        Vector3 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
        Move(direction * (float)delta * _speed);
    }
    
    public override async void Cast()
    {
        HasToMove = true;

        await Task.Delay((int)(Duration * 1000));

        QueueFree();
    }

    public void SetTarget(Enemy target)
    {
        _target = target;
    }

    private void OnAreaEntered(Area3D area3D)
    {
        GD.Print("1");
        OnAreaEnteredInternal(area3D);
    }

    protected abstract void OnAreaEnteredInternal(Area3D area3D);
    
    // Direction must be multiplied by delta and speed. 
    private void Move(Vector3 direction)
    {
        Position += direction;
    }

}
