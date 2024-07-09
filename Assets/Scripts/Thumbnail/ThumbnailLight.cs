using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ThumbnailLight : MonoBehaviour
{
    [SerializeField] Light _light;

    public void Enable()
    {
        _light.enabled = true;
    }

    public void Disable()
    {
        _light.enabled = false;
    }
}