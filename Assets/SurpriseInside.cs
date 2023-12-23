using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurpriseInside : MonoBehaviour
{
    public GameObject[] traps;
    public GameObject confetti;

    public Animator anim;

    GameObject replacementPrefab;

    public void InstantiateReplacement()
    {
        GameObject x = null;

        if (replacementPrefab != null)
        {
            x = Instantiate(replacementPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Replacement Prefab is null. Assign a prefab in the editor.");
        }

        Instantiate(confetti, x.transform);
        Destroy(gameObject);
    }

    public void TrapInside()
    {
        int randomIndex = Random.Range(0, traps.Length);
        GameObject selectedPrefab = traps[randomIndex];

        if (selectedPrefab != null)
        {
            replacementPrefab = selectedPrefab;
            anim.SetTrigger("Undulate");
        }
        else
        {
            Debug.LogError("Prefab is null. Make sure all prefabs are assigned.");
        }
    }
}