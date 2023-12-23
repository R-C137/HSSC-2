/* TrashTalkHandler.cs - HSSC-2
 * 
 * Creation Date: 22/12/2023
 * Authors: C137
 * Original: C137
 * 
 * Edited By: C137
 * 
 * Changes: 
 *      [22/12/2023] - Game pausing support (C137)
 */
using System.Collections;
using UnityEngine;

public class TrashTalkHandler : MonoBehaviour
{
    /// <summary>
    /// The audio source handling the playing of the SFXs
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// The delay in between playing the different 'trash talks' SFX
    /// </summary>
    public float trashTalkDelay;

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(trashTalkDelay);

            audioSource.clip = Utility.singleton.commonSFX.grinchTrashTalk[Random.Range(0, Utility.singleton.commonSFX.grinchTrashTalk.Length)];
            audioSource.Play();

            yield return new WaitForSeconds(audioSource.clip.length);
        }
    }

    private void Reset()
    {
        TryGetComponent(out audioSource);
    }
}
