using Godot;
using System;
using System.Threading.Tasks;

public abstract partial class TargetSpell : Spell
{
    [Export] private float _speed;
    
    private bool _hasToMove;

    private Enemy _target;
    
    public override void _Process(double delta)
    {
        if (_hasToMove == false)
        {
            return;
        }
		
        Vector3 direction = (_target.GlobalPosition - GlobalPosition).Normalized();
        Move(direction * (float)delta * _speed);
    }
    
    public override async void Cast()
    {
        _hasToMove = true;

        await Task.Delay((int)(Duration * 1000));
		
        QueueFree();
    }

    public void SetTarget(Enemy target)
    {
        _target = target;
    }

    private void OnAreaEntered(Area3D area3D)
    {
        OnAreaEnteredInternal(area3D);
    }

    protected abstract void OnAreaEnteredInternal(Area3D area3D);
    
    // Direction must be multiplied by delta and speed. 
    private void Move(Vector3 direction)
    {
        Position += direction;
    }

}
