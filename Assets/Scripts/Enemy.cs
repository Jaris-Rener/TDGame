using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int Health;
    public int MaxHealth;

    protected GridPath _CurrentPath;
    protected int _TargetPoint;

    private void Start()
    {
        Health = MaxHealth;
    }

    public void Damage(int amt)
    {
        Health -= amt;
        if (Health <= 0)
            Kill();
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void AssignPath(GridPath path)
    {
        _CurrentPath = path;
        _TargetPoint = 0;
    }
}
