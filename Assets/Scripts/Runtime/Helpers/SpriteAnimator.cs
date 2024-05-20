using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimator : MonoBehaviour
{
    [ReorderableList]
    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private float _timeToNextFrame;
    [SerializeField] private bool loopOn = true;
    private float _countDownTimer;
    private Image _image;
    private SpriteRenderer _spriteRenderer;


    public bool IsAnimating;


    void Awake()
    {
        TryGetComponent(out _image);
        TryGetComponent(out _spriteRenderer);

        if (_image is not null)
        {
            _image.sprite = _sprites[0];
        } else if (_spriteRenderer is not null)
        {
            _spriteRenderer.sprite = _sprites[0];
        }
        
        _countDownTimer = _timeToNextFrame;
    }

    public void Reset(bool loop)
    {
        _index = 0;
        _countDownTimer = _timeToNextFrame;
        loopOn = loop;
        IsAnimating = true;
        if (_image is not null)
        {
            _image.sprite = _sprites[_index];
        }
        else if (_spriteRenderer is not null)
        {
            _spriteRenderer.sprite = _sprites[_index];
        }
    }

    private int _index = 0;
    
    void Update()
    {
        _countDownTimer -= Time.deltaTime;
        if (!(_countDownTimer <= 0)) return;

        if (_index < _sprites.Length - 1)
        {
            _index++;
        }
        else
        {
            IsAnimating = false;
            if (loopOn)
            {
                _index = 0;
            }
            else
            {
                return;
            }
        }

        if (_image is not null) {
            _image.sprite = _sprites[_index];
        } else if (_spriteRenderer is not null) {
            _spriteRenderer.sprite = _sprites[_index];
        }
        _countDownTimer = _timeToNextFrame;
    }
}
