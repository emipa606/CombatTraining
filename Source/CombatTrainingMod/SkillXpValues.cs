namespace KriilMod_CD;

public class SkillXpValues
{
    public int DayOfYear;
    public float XpSinceLastLevel;
    public float XpSinceMidnight;

    public SkillXpValues(int dayOfYear, float xpSinceMidnight, float xpSinceLastLevel)
    {
        DayOfYear = dayOfYear;
        XpSinceMidnight = xpSinceMidnight;
        XpSinceLastLevel = xpSinceLastLevel;
    }
}