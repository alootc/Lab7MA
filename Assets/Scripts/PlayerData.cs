using System;

[System.Serializable]
public class PlayerData
{
    public string playerName = "Jugador";
    public int level = 1;
    public int experience = 0;
    public int availableSkillPoints = 0;
    public int strength = 5;
    public int defense = 5;
    public int agility = 5;
    public int totalClicks = 0;

    public int ExperienceRequiredForNextLevel
    {
        get { return level * 100; }
    }

    public float ExperienceProgress
    {
        get { return (float)experience / ExperienceRequiredForNextLevel; }
    }
}