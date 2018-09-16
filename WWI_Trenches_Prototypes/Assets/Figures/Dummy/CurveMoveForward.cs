using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMoveForward : MonoBehaviour
{

    public AnimationCurve MoveCurve;

    public bool Playing;

    [Range(0.001f, 1)]
    public float JumpSpeed = 1;

    public float SpeedMultiploer = 2;
    public Transform JumpEndPoint;


    [Tooltip("In sec?")]
    [SerializeField] public float _jumpDuration = 2;

    private float _currentTime = 0;

    private Vector3 _initPosition;

    private Vector3 _direction;
    // Use this for initialization
    void Start()
    {
        var rigids = GetComponentsInChildren<Rigidbody>();
        foreach (var rigid in rigids)
        {
            rigid.useGravity = false;
            rigid.isKinematic = true;
        }
        _initPosition = transform.position;
        _direction = (JumpEndPoint.position - _initPosition).normalized;
    }

    // Update is called once per frame
    void Update()
    {

        if (Playing)
        {
            _currentTime = Mathf.Clamp(_currentTime + Time.deltaTime, 0, _jumpDuration);

            transform.position += _direction * ((JumpSpeed / 100)* SpeedMultiploer) * MoveCurve.Evaluate(_currentTime);

            //Todo: Tohle je jen debug loop
            if (_currentTime >= _jumpDuration)
            {
                transform.position = _initPosition;
                _currentTime = 0;
            }
        }
        
    }
}