using Godot;
using System;
using System.Threading.Tasks;

public partial class Weapon : Area3D
{
    [Export] private uint _damage;
    [Export] private float _attackDurationSeconds;
    [Export] private float _attackCooldownSeconds; // Cooldown starts after attack performed.
    
    [Export] private MeshInstance3D _mesh;

    private bool _canAttack = true;
    
    // TODO: delete me
    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("attack") == false)
        {
            return;
        }

        if (_canAttack == false)
        {
            return;
        }

        Attack();
    }

    // TODO: delete me
    private async void Attack()
    {
        _canAttack = false;
        
        EnableHitbox();
        await Task.Delay((int)(_attackDurationSeconds * 1000)); // Attack duration
        DisableHitbox();

        uint finalDelay;
        if (_attackCooldownSeconds - _attackDurationSeconds <= 0)
        {
            finalDelay = 0;
        }
        else
        {
            finalDelay = (uint)((_attackCooldownSeconds - _attackDurationSeconds) * 1000);
        }
        
        await Task.Delay((int)finalDelay);

        _canAttack = true;
    }

    // Animator.
    private void EnableHitbox()
    {
        _mesh.Visible = true;
        Monitoring = true;
    }

    // Animator.
    private void DisableHitbox()
    {
        _mesh.Visible = false;
        Monitoring = false;
    }

    private void OnAreaEntered(Area3D area3D)
    {
        HitboxComponent hitboxComponent = (HitboxComponent)area3D;
        GD.Print($"Player weapon catch {hitboxComponent.Name}");

        hitboxComponent.Hit(_damage);
    }
}
