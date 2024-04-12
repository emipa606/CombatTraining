namespace KriilMod_CD;

public class SkillXpValues(int dayOfYear, float xpSinceMidnight, float xpSinceLastLevel)
{
    public readonly int DayOfYear = dayOfYear;
    public readonly float XpSinceLastLevel = xpSinceLastLevel;
    public readonly float XpSinceMidnight = xpSinceMidnight;
}