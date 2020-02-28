//
// Copyright (c) Microsoft Corporation. All rights reserved.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RemoteAssist.Mobile.UI
{
    [RequireComponent(typeof(Image))]
    public class SpriteLooper : MonoBehaviour
    {
        [SerializeField]
        private float speed = 1;
        
        private float time = 0;
        private Image image;
        
        [SerializeField]
        private List<Sprite> sprites = new List<Sprite>();
        
        private void Awake()
        {
            image = GetComponent<Image>();
        }

        void Update()
        {
            time += Time.deltaTime * speed;
            if(time > sprites.Count)
            {
                time = 60;
            }
            int index = Mathf.FloorToInt(time);
            image.sprite = sprites[index];
        }
    }
}
