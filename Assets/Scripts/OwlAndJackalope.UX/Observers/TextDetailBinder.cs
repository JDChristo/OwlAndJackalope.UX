﻿using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace OwlAndJackalope.UX.Observers
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextDetailBinder : BaseDetailBinder
    {
        [SerializeField, DetailName(typeof(string))]
        private string _baseStringDetailName;

        [SerializeField, DetailName(typeof(string), typeof(int),
             typeof(long), typeof(double), typeof(bool), typeof(Enum))]
        private string[] _stringArgumentDetailNames;

        private TextMeshProUGUI _text;
        private DetailObserver<string> _baseStringObserver;
        private DetailObserver[] _stringArgumentObservers;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _baseStringObserver = new DetailObserver<string>(_baseStringDetailName, _referenceModule.Reference, UpdateText);
            _stringArgumentObservers = new DetailObserver[_stringArgumentDetailNames.Length];
            for (var i = 0; i < _stringArgumentObservers.Length; ++i)
            {
                _stringArgumentObservers[i] = new DetailObserver(_stringArgumentDetailNames[i], _referenceModule.Reference, UpdateText);
            }
            
            UpdateText();
        }

        private void OnDestroy()
        {
            _baseStringObserver?.Dispose();
            for (var i = 0; i < _stringArgumentObservers.Length; ++i)
            {
                _stringArgumentObservers[i].Dispose();
            }
        }

        private void UpdateText()
        {
            UpdateText(_baseStringObserver.Value);
        }
        
        private void UpdateText(string text)
        {
            if (_text != null)
            {
                if (_stringArgumentObservers?.Length > 0 && !string.IsNullOrEmpty(text))
                {
                    try
                    {
                        text = string.Format(text, _stringArgumentObservers.Select(x => x.ObjectValue).ToArray());
                    }
                    catch (FormatException)
                    {
                        Debug.LogWarning($"{text} is in a bad format");
                    }
                }
                
                _text.SetText(text);
            }
        }

        protected override int UpdateDetailNames(string previousName, string newName)
        {
            var sum = UpdateDetailName(ref _baseStringDetailName, previousName, newName);
            for (var i = 0; i < _stringArgumentDetailNames.Length; ++i)
            {
                sum += UpdateDetailName(ref _stringArgumentDetailNames[i], previousName, newName);
            }
            return sum;
        }
    }
}