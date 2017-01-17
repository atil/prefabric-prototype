using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Prefabric
{
    public static class Sfx
    {
        private static readonly Dictionary<PfResourceType, AudioClip> Effects =
            new Dictionary<PfResourceType, AudioClip>();

        static Sfx()
        {
            Effects.Add(PfResourceType.SfxTileSelect1, PfResources.Load<AudioClip>(PfResourceType.SfxTileSelect1));
            Effects.Add(PfResourceType.SfxTileSelect2, PfResources.Load<AudioClip>(PfResourceType.SfxTileSelect2));
            Effects.Add(PfResourceType.SfxBendFail, PfResources.Load<AudioClip>(PfResourceType.SfxBendFail));
            Effects.Add(PfResourceType.SfxUnbend, PfResources.Load<AudioClip>(PfResourceType.SfxUnbend));
        }

        public static void Play(PfResourceType sfxResType)
        {
            if (Effects.ContainsKey(sfxResType))
            {
                Camera.main.GetComponent<AudioSource>().PlayOneShot(Effects[sfxResType]);
            }
        }
            
    }
}
