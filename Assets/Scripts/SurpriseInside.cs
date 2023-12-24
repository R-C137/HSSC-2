using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseInside : MonoBehaviour
{
    public GameObject trap;
    public GameObject confetti;

    public Animator anim;

    public bool isTrap;

    public bool isReplaced;

    public bool shootBack = false;

    GameObject replacementPrefab;

    public void InstantiateReplacement()
    {
        if (isReplaced)
            return;

        GameObject x = null;

        if (replacementPrefab != null)
        {
            x = Instantiate(replacementPrefab, transform.position, Quaternion.identity);
            x.transform.parent = MapGeneration.singleton.spawnedTerrains[^1].transform;

            var trap = x.GetComponent<TrapBehaviour>();

            if (shootBack)
                trap.TrapShot();
            else
                trap.Activate();

        }
        else
        {
            Debug.LogError("Replacement Prefab is null. Assign a prefab in the editor.");
        }

        Instantiate(confetti, x.transform);
        Destroy(gameObject);
    }

    [ContextMenu("Force Replace")]
    public void ForceReplacement()
    {
        if (shootBack)
            return;

        shootBack = true;

        InstantiateReplacement();
        Destroy(gameObject);
        isReplaced = true;
    }

    public void TrapInside(float activationTime)
    {

        GameObject selectedPrefab = trap;

        isTrap = true;

        if (selectedPrefab != null)
        {
            replacementPrefab = selectedPrefab;

            anim.speed = anim.runtimeAnimatorController.animationClips[1].length / 
                (GrinchPositionalHandling.singleton.positions[GrinchPositionalHandling.singleton.currentPosition].trapActivationTimeMultiplier * activationTime);
            anim.SetTrigger("Undulate");
        }
        else
        {
            Debug.LogError("Prefab is null. Make sure all prefabs are assigned.");
        }
    }
}