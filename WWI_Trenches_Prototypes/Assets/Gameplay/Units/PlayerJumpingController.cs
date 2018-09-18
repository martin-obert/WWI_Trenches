using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Units
{
    public class PlayerJumpingController : MonoBehaviour
    {
        [SerializeField] private Transform _jumpEndpoint;

        [Tooltip("Use this curve to set jumping position advance")]
        [SerializeField] private AnimationCurve _positionAdvanceCurve;

        public bool Playing;

        [Range(0.001f, 1)] public float JumpSpeed = 1;

        public float SpeedMultiploer = 2;


        [Tooltip("In sec"), SerializeField] public float JumpDuration = 1.667f;

        [SerializeField] private float _currentTime = 0;

        private Vector3 _initPosition;

        private Vector3 _direction;

        [SerializeField] private bool _isLooped;

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

                if (x >= 1)
                {
                    if (_isLooped)
                        transform.position = _initPosition;

                    Stop();
                }
            }
        }

        public void Stop()
        {
            Playing = false;

            _currentTime = 0;
            if (!_isLooped)
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
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerJumpingController))]
    public class PlayerJumpingControllerEditor : Editor
    {
        private int _currentKey = 0;

        private Vector2 _shift = new Vector2(0.1f, 0.1f);
        private PlayerJumpingController _controller;
        private AnimationCurve animationCurve;
        void OnEnable()
        {
            _controller = target as PlayerJumpingController;
            animationCurve = serializedObject.FindProperty("_positionAdvanceCurve").animationCurveValue;
        }

        public override void OnInspectorGUI()
        {
            var modified = false;


            if (serializedObject.FindProperty("_jumpEndpoint").objectReferenceValue != null && GUILayout.Button("Jump"))
            {
                _controller.Stop();
                _controller.Jump((Transform)serializedObject.FindProperty("_jumpEndpoint").objectReferenceValue);

            }

            if (animationCurve != null)
            {
                

                var currentTime = serializedObject.FindProperty("_currentTime").floatValue;

                EditorGUILayout.LabelField("Curve Keys: " + animationCurve.keys.Length);

                if (animationCurve.keys.Length > 0 && _currentKey >= 0 && _currentKey < animationCurve.keys.Length)
                {
                    var key = animationCurve.keys[_currentKey];

                    EditorGUILayout.LabelField("Current key: " + string.Format("[{0} | {1}]", key.time, key.value));
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("<-"))
                    {
                        _currentKey = Mathf.Clamp(_currentKey - 1, 0, animationCurve.keys.Length - 1);
                    }
                    if (GUILayout.Button("Closest to current time " + currentTime))
                    {
                        key = ClosestTo(animationCurve.keys, currentTime);
                    }
                    if (GUILayout.Button("->"))
                    {
                        _currentKey = Mathf.Clamp(_currentKey + 1, 0, animationCurve.keys.Length - 1);
                    }

                    EditorGUILayout.EndHorizontal();
                  

                    _shift = EditorGUILayout.Vector2Field("Shift", _shift);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginHorizontal();

                   
                    if (GUILayout.Button("◄"))
                    {
                        key.time -= _shift.x;
                        modified = true;
                    }

                    if (GUILayout.Button("►"))
                    {
                        key.time += _shift.x;
                        modified = true;
                    }
                    EditorGUILayout.EndHorizontal();

                    

                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("▲"))
                    {
                        key.value += _shift.y;
                        modified = true;
                    }
                    if (GUILayout.Button("▼"))
                    {
                        key.value -= _shift.y;
                        modified = true;
                    }

                    if (modified)
                    {
                        Debug.Log(key.time);

                        Debug.Log(key.value);

                        animationCurve.MoveKey(_currentKey, key);

                        serializedObject.FindProperty("_positionAdvanceCurve").animationCurveValue = animationCurve;
                        serializedObject.ApplyModifiedProperties();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndHorizontal();
                }
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