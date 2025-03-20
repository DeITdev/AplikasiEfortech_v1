using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GazeInteraction : MonoBehaviour
{
    // Lists to track objects with InfoBehaviour and AnimationBehaviour
    private List<InfoBehaviour> infos = new List<InfoBehaviour>();
    private List<AnimationBehaviour> animations = new List<AnimationBehaviour>();

    void Start()
    {
        UpdateObjectsLists();
        // Subscribe to the spawn and destroy events
        CustomARPlacement.OnObjectSpawned += UpdateObjectsLists;
        CustomARPlacement.OnObjectDestroyed += UpdateObjectsLists;
    }

    void OnDestroy()
    {
        // Unsubscribe from events when this object is destroyed
        CustomARPlacement.OnObjectSpawned -= UpdateObjectsLists;
        CustomARPlacement.OnObjectDestroyed -= UpdateObjectsLists;
    }

    void UpdateObjectsLists()
    {
        // Update both lists
        infos = FindObjectsOfType<InfoBehaviour>().ToList();
        animations = FindObjectsOfType<AnimationBehaviour>().ToList();

        Debug.Log($"Updated lists. InfoBehaviours: {infos.Count}, AnimationBehaviours: {animations.Count}");
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            GameObject go = hit.collider.gameObject;
            if (go.CompareTag("info"))
            {
                // Handle InfoBehaviour components
                InfoBehaviour infoBehaviour = go.GetComponent<InfoBehaviour>();
                if (infoBehaviour != null)
                {
                    OpenInfo(infoBehaviour);
                }

                // Handle AnimationBehaviour components
                // First try to find on the direct GameObject
                AnimationBehaviour animBehaviour = go.GetComponent<AnimationBehaviour>();

                // If not found, try to find in parent (as per your requirement)
                if (animBehaviour == null && go.transform.parent != null)
                {
                    animBehaviour = go.transform.parent.GetComponent<AnimationBehaviour>();
                }

                if (animBehaviour != null)
                {
                    OpenAnimation(animBehaviour);
                }
            }
            else
            {
                CloseAll();
            }
        }
        else
        {
            CloseAll();
        }
    }

    void OpenInfo(InfoBehaviour desiredInfo)
    {
        foreach (InfoBehaviour info in infos)
        {
            if (info == desiredInfo)
            {
                info.OpenInfo();
            }
            else
            {
                info.CloseInfo();
            }
        }
    }

    void OpenAnimation(AnimationBehaviour desiredAnim)
    {
        foreach (AnimationBehaviour anim in animations)
        {
            if (anim == desiredAnim)
            {
                anim.OpenAnimation();
            }
            else
            {
                anim.CloseAnimation();
            }
        }
    }

    void CloseAll()
    {
        // Close all InfoBehaviour objects
        foreach (InfoBehaviour info in infos)
        {
            info.CloseInfo();
        }

        // Close all AnimationBehaviour objects
        foreach (AnimationBehaviour anim in animations)
        {
            anim.CloseAnimation();
        }
    }
}