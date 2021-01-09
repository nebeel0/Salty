
public class ParticleType
{
    public int charge;
    public int spinFactor;
    public float mass;
    public int layer;
    public string[] ignoredCollisions;

    public ParticleType(int charge, int spinFactor, float mass, int layer, string[] ignoredCollisions)
    {
        this.charge = charge;
        this.spinFactor = spinFactor;
        this.mass = mass;
        this.layer = layer;
        this.ignoredCollisions = ignoredCollisions;
    }
}
