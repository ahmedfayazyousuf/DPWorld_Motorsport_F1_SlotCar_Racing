using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TheHangingHouse.UI.TableInternal
{
    public class Cell : MonoBehaviour
    {
        [Header("Set In Inspector")]
        public TMP_Text labelText;

        private Image _image;

        public string Content
        {
            get { return labelText.text; }
            set { labelText.text = value; }
        }

        public Color Colour
        {
            get { 
                if (_image == null) 
                    _image = GetComponent<Image>();
                return _image.color;
            }
            set {
                if (_image == null)
                    _image = GetComponent<Image>();
                _image.color = value; 
            }
        }

        private void Start()
        {
            _image = GetComponent<Image>();
        }
    }
}

