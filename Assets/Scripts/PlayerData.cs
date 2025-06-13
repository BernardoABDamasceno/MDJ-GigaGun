using System;

[Serializable]
public class PlayerData
{
    public string playerId;
    public float timeSpentFPS;
    public float timeSpentAssembly;
    public float airTime;
    public int revolvers;
    public int shotguns;
    public int plasmaRifles;
    public int flamethrowers;
    public int rpgs;
    
    public PlayerData()
    {
        playerId = "Player_" + Guid.NewGuid().ToString();
        timeSpentFPS = 0f;
        timeSpentAssembly = 0f;
        airTime = 0f;
        revolvers = 0;
        shotguns = 0;
        plasmaRifles = 0;
        flamethrowers = 0;
        rpgs = 0;
    }
}