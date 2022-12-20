public enum Orientation
{
    north,
    east,
    south,
    west,
    none
}


public enum AimDirection
{
    Up,
    UpRight,
    UpLeft,
    Right,
    Left,
    Down
    
}
// different gamestates
public enum GameState
{
    gameStarted,
    playingLevel,
    engagingEnemies,
    bossStage,
    engagingBoss,
    levelCompleted,
    gameWon,
    gameLost, 
    gamePaused,
    dungeonOverviewMap,
    restartGame
}
