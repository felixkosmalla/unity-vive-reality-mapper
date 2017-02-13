using Leap.Unity;
using UnityEngine;

public class PresentCollider : MonoBehaviour
{

   

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private bool IsHand(Collider other)
    {
        if (other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel>())
            return true;
        else
            return false;
    }


    bool exploded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (IsHand(other))
        {
            if (!exploded)
            {
                var spawner = GameObject.Find("PresentSpawner");
                var explodeAllowed = spawner.GetComponent<PresentSpawnerBHV>().presentExploded();

                if (!explodeAllowed)
                {
                    return;
                }


            }



            var theScript = transform.parent.gameObject.GetComponent<Explosion>();
            theScript.explode();
            //transform.parent.gameObject.GetComponent<Explosion>().Rpcexplode();
            Debug.Log("Yay! A hand collided!");




            exploded = true;
            
        }
    }
}