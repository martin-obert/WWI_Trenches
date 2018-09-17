using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerJumpingController : MonoBehaviour
{
    [SerializeField] private Transform _jumpEndpoint;

    [Tooltip("Use this curve to set jumping position advance")]
    [SerializeField] private AnimationCurve _positionAdvanceCurve;

    public bool Playing;

    [Range(0.001f, 1)] public float JumpSpeed = 1;

    public float SpeedMultiploer = 2;


    [Tooltip("In sec?")] [SerializeField] public float JumpDuration = 1.667f;

    private float _currentTime = 0;

    private Vector3 _initPosition;

    private Vector3 _direction;

    [SerializeField] private bool _isLooped;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Playing)
        {
            if (!_jumpEndpoint)
            {
                Debug.LogError("Player has no endpoint!");
                return;
            }

            _currentTime = Mathf.Clamp(_currentTime + Time.fixedDeltaTime, 0, JumpDuration);

            var x = _currentTime / JumpDuration;

            var moveAmount = _direction * (JumpSpeed / 100 * SpeedMultiploer) * _positionAdvanceCurve.Evaluate(x);

            transform.Translate(moveAmount, Space.Self);
            
            if (x >= 1 && _isLooped)
            {
                transform.position = _initPosition;

                Stop();
            }
        }
    }

    public void Stop()
    {
        Playing = false;

        _currentTime = 0;

        _jumpEndpoint = null;
    }

    public void Jump(Transform jumpEndpoint)
    {
        _initPosition = transform.position;

        _jumpEndpoint = jumpEndpoint;

        _direction = (jumpEndpoint.position - _initPosition).normalized;

        _currentTime = 0;

        Playing = true;
    }


#if UNITY_EDITOR
    public class PlayerJumpingControllerEditor : Editor
    {
        private int _currentKey = 0;

        private float _timeShift = 0.1f;

        public override void OnInspectorGUI()
        {
            var modified = false;

            var animationCurve = serializedObject.FindProperty("_positionAdvanceCurve").animationCurveValue;

            if (animationCurve != null)
            {
                var keys = animationCurve.keys;

                var currentTime = serializedObject.FindProperty("_currentTime").floatValue;

                EditorGUILayout.LabelField("Keys");

                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("+1"))
                {
                    var len = keys.Length;

                    var newKey = new Keyframe(currentTime, 1);

                    animationCurve.AddKey(newKey);

                    _currentKey = len;

                    modified = true;
                }
                if (GUILayout.Button("-1"))
                {
                    if (keys.Length > 0 && _currentKey >= 0)
                    {
                        animationCurve.RemoveKey(_currentKey);
                        modified = true;
                    }
                }
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.LabelField("Curve Keys: " + keys.Length);

                if (keys.Length > 0 && _currentKey >= 0 && _currentKey < keys.Length)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("→"))
                    {
                        _currentKey = _currentKey + 1 >= keys.Length ? _currentKey = keys.Length - 1 : _currentKey + 1;
                    }
                    if (GUILayout.Button("←"))
                    {
                        _currentKey = _currentKey - 1 <= 0 ? 0 : _currentKey - 1;
                    }

                    EditorGUILayout.EndHorizontal();

                    var key = keys[_currentKey];

                    EditorGUILayout.LabelField("Current key: " + string.Format("[{0} | {1}]", key.time, key.value));

                    if (GUILayout.Button("Closest to current time " + currentTime))
                    {
                        key = ClosestTo(keys, currentTime);
                    }
                    _timeShift = EditorGUILayout.FloatField("Amount", _timeShift);

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("▲"))
                    {
                        key.time += _timeShift;
                        modified = true;
                    }
                    if (GUILayout.Button("▼"))
                    {
                        key.time -= _timeShift;
                        modified = true;
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndHorizontal();
                }

                if (modified)
                    serializedObject.ApplyModifiedProperties();

            }

            base.OnInspectorGUI();
        }

        private Keyframe ClosestTo(IReadOnlyList<Keyframe> keys, float value)
        {
            var closest = keys.Max(x => x.time);
            var result = keys[0];
            foreach (var keyframe in keys)
            {
                if (Mathf.Abs(keyframe.time - value) < closest)
                {
                    result = keyframe;
                }
            }

            return result;
        }
    }
#endif
}