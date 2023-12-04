using Godot;
using System.Threading.Tasks;

public abstract partial class TargetSpell : Spell
{
    [Export] protected float Speed;
    
    protected bool HasToMove;

    private Enemy _target;
    
    protected bool IsApplied;
    
    public override void _Process(double delta)
    {
        if (HasToMove == false)
        {
            return;
        }
		
        Vector3 direction = (_target.GlobalPosition + _target.RealPosition - GlobalPosition).Normalized();
        Move(direction * (float)delta * Speed);
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
        OnAreaEnteredInternal(area3D);
    }

    protected abstract void OnAreaEnteredInternal(Area3D area3D);
    
    // Direction must be multiplied by delta and speed. 
    private void Move(Vector3 direction)
    {
        Position += direction;
    }

}
