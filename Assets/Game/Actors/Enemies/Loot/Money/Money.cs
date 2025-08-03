public class Money : Loot
{
    public int pointsAdd = 200;

    protected override string Message => $"Earned {pointsAdd} points!";

    public override void Pickup()
    {
        base.Pickup();
        var scoreboard = FindAnyObjectByType<Scoreboard>();
        scoreboard.AddScore(pointsAdd);
    }
}
