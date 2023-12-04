using Godot;
using System;
using System.Threading.Tasks;

public partial class Boss : Enemy
{
	[Export] private PackedScene _bossProjectile;
	
	[Export] private PackedScene _horcruxScene;
	[Export] private PackedScene[] _enemyScenes;
	
	[Export] private Node3D[] _horcruxSpawnPositions;
	[Export] private EnemySpawner[] _enemySpawners;

	[Export] private HitboxComponent _hitboxComponent;

	[Export] private Vector3 _realPosition;
	
	[Export] private uint _horcruxSpawnCount;
	[Export] private uint _monsterSpawnCount;
	
	[Export] private float _horcruxSpawnCooldownSeconds;
	[Export] private float _monsterSpawnCooldownSeconds;
	
	private bool _canSpawnHorcrux = true;
	private bool _canSpawnMonsters = true;
	
	private uint _horcruxLeft;

	public override void _Ready()
	{
		for (var i = 0; i < _enemySpawners.Length; i++)
		{
			EnemySpawner spawner = _enemySpawners[i];
			spawner.Position = _horcruxSpawnPositions[i % _horcruxSpawnPositions.Length].Position;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (BlockStateMachine == true)
		{
			return;
		}

		switch (State)
		{
			case EnemyState.Idle:
				Idle();
				break;
			case EnemyState.StartAttack:
				StartAttack();
				break;
			case EnemyState.Attack:
				Attack();
				break;
			case EnemyState.EndAttack:
				EndAttack();
				break;
			case EnemyState.SpawnHorcrux:
				SpawnHorcrux();
				break;
			case EnemyState.SpawnMonsters:
				SpawnMonsters();
				break;
			case EnemyState.Dead: // Do nothing.
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	protected override void Idle()
	{
		if (_canSpawnMonsters == true)
		{
			State = EnemyState.SpawnMonsters;
			return;
		}
		
		if (_canSpawnHorcrux == true)
		{
			State = EnemyState.SpawnHorcrux;
			return;
		}

		if (IsAttackOnCooldown == false)
		{
			State = EnemyState.StartAttack;
		}
	}

	#region StateMachine

	private void SpawnHorcrux()
	{
		// Be INVULNERABLE.
		_canSpawnHorcrux = false;

		for (var i = 0; i < _horcruxSpawnCount; i++)
		{
			Horcrux horcrux = _horcruxScene.Instantiate() as Horcrux;

			if (horcrux == null)
			{
				return;
			}
		
			AddChild(horcrux);
			horcrux.Position = _horcruxSpawnPositions[i].Position;
		}
		
		Horcrux.OnHorcruxDie += OnHorcruxDie;
		
		State = EnemyState.Idle;
	}

	private async void SpawnMonsters()
	{
		_canSpawnMonsters = false;
		
		for (var i = 0; i < _monsterSpawnCount; i++)
		{
			PackedScene randomEnemy = _enemyScenes[GD.Randi() % _enemyScenes.Length];
			_enemySpawners[i % _enemySpawners.Length].Spawn(randomEnemy, Target);
		}
		
		State = EnemyState.Idle;
		await Task.Delay((int)(_monsterSpawnCooldownSeconds * 1000));
		_canSpawnMonsters = true;
	}

	protected override void AttackInternal()
	{
		BossProjectile bossProjectile = _bossProjectile.Instantiate() as BossProjectile;
		
		if (bossProjectile == null)
		{
			return;
		}

		GetTree().Root.AddChild(bossProjectile);
		bossProjectile.GlobalPosition = GlobalPosition + _realPosition;
		bossProjectile.SetDirection(Target.GlobalPosition + Vector3.Up - bossProjectile.GlobalPosition);
		bossProjectile.Cast();
	}
	
	// TODO: Can be removed if can be switched to Idle in Enemy class?
	protected override async void EndAttack()
	{
		BlockStateMachine = true;
		await Task.Delay((int)(EndAttackTimeSeconds * 1000));
		BlockStateMachine = false;

		State = EnemyState.Idle; // TODO: Can be switched to Idle?
	}

	#endregion

	private async void OnHorcruxDie()
	{
		_horcruxLeft--;

		if (_horcruxLeft != 0)
		{
			return;
		}
		
		Horcrux.OnHorcruxDie -= OnHorcruxDie;

		// Be VULNERABLE.
		await Task.Delay((int)(_horcruxSpawnCooldownSeconds * 1000));
		_canSpawnHorcrux = true;
	}
}
