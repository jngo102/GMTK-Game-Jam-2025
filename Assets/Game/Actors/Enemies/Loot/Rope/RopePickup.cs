public class RopePickup : Loot
{
    public float ropeLengthAdd = 1;
    
    protected override string Message => $"Rope length extended by {ropeLengthAdd} meter!";

    public override void Pickup()
    {
        base.Pickup();
        var lassoController = FindAnyObjectByType<LassoController>();
        lassoController.maxDrawLength += ropeLengthAdd;
        var lassoSpinner = FindAnyObjectByType<LassoSpinner>();
        lassoSpinner.maxLassoDistance += ropeLengthAdd;
    }
}
