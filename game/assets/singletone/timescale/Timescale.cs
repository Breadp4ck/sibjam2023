public static class Timescale
{
    public static float Enemy => _enemy;
    private static float _enemy = 1.0f;
    
    public static float Player => _player;
    private static float _player = 1.0f;
    
    public static void SetEnemyTimescale(float scale)
    {
        _enemy = scale;
    }
    
    public static void SetPlayerTimescale(float scale)
    {
        _player = scale;
    }
}
