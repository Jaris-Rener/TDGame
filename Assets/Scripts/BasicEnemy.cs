using UnityEngine;

public class BasicEnemy : Enemy
{
    private void Update()
    {
        if (_CurrentPath != null)
        {
            var dist = Vector3.Distance(_CurrentPath.Path[_TargetPoint].SnappedPos, transform.position);
            if (dist < 0.01f)
            {
                ++_TargetPoint;
                if(_TargetPoint >= _CurrentPath.Path.Count)
                    Destroy(gameObject);
            }

            var dir = (_CurrentPath.Path[_TargetPoint].SnappedPos - transform.position).normalized;
            transform.Translate(dir*Time.smoothDeltaTime);
        }
    }
}
