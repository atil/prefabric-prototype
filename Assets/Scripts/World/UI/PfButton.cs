using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prefabric;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Prefabric
{
    /// <summary>
    /// An extension of Unity's button to have mouse hover and click sound
    /// </summary>
    [RequireComponent(typeof(AudioSource), typeof(Button))]
    public class PfButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Action Clicked;

        private AudioSource _audioSource;

        private AudioClip _clickClip;
        private AudioClip _enterClip;
        private AudioClip _exitClip; // For future use

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _clickClip = PfResources.Load<AudioClip>(PfResourceType.SfxClick2);
            _enterClip = PfResources.Load<AudioClip>(PfResourceType.SfxClick1);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _audioSource.PlayOneShot(_enterClip);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _audioSource.PlayOneShot(_clickClip);
            if (Clicked != null)
            {
                Clicked();
            }
        }
    }
}
