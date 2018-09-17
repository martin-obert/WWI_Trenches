using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumping : MonoBehaviour
{

    public AnimationCurve MoveCurve;

    public bool Playing;

    [Range(0.001f, 1)]
    public float JumpSpeed = 1;

    public float SpeedMultiploer = 2;

    [SerializeField] private Transform _jumpEndPoint;


    [Tooltip("In sec?")]
    [SerializeField] public float JumpDuration = 2;

    private float _currentTime = 0;

    private Vector3 _initPosition;

    private Vector3 _direction;

    [SerializeField] private bool _isLooped;

    // Use this for initialization
    void Start()
    {
       
        _initPosition = transform.position;
        if (_jumpEndPoint)
            _direction = (_jumpEndPoint.position - _initPosition).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (Playing)
        {
            _currentTime = Mathf.Clamp(_currentTime + Time.deltaTime, 0, JumpDuration);

            var x = _currentTime / JumpDuration;
            Debug.Log(x);
            transform.position += _direction * (JumpSpeed / 100 * SpeedMultiploer) * MoveCurve.Evaluate(x);

            if (x >= 1 && _isLooped)
            {
                transform.position = _initPosition;
                _currentTime = 0;
                Playing = true;
            }
        }

    }

    public void Stop()
    {
        Playing = false;
    }

    public void Jump(Vector3 position, Transform jumpDestination, AnimationCurve positionAdvanceCurve)
    {
        _initPosition = transform.position;

        MoveCurve = positionAdvanceCurve;
        _jumpEndPoint = jumpDestination;

        _direction = (_jumpEndPoint.position - _initPosition).normalized;
        _currentTime = 0;
        Playing = true;
    }
}