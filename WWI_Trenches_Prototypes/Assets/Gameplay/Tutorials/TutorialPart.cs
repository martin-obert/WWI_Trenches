using System;
using Assets.IoC;
using UnityEditor;
using UnityEngine;

namespace Assets.Gameplay.Tutorials
{
    public class TutorialPart : MonoBehaviour
    {
        public string Id;

        [SerializeField] private TutorialPart _nextPart;

        public string NextPartId => _nextPart?.Id;

        [SerializeField] private TutorialDialog[] _dialogs;

        void OnEnable()
        {
            _dialogs = new TutorialDialog[0];
        }

        public TutorialDialog Current =>
            _dialogs.Length == 0 || _currentIndex >= _dialogs.Length || _currentIndex < 0 ? null : _dialogs[_currentIndex];

        private int _currentIndex = 0;

        public event EventHandler IndexChanged;

        public bool IsRepeatable;

        public bool HasFinishedAtLeastOnce;

        public bool IsComplete => _currentIndex >= _dialogs.Length;

        void Start()
        {
            InjectService.Instance.GetInstance<TutorialManager>(manager => manager.AddPart(this));
        }

        public void Continue()
        {
            _currentIndex++;

            OnIndexChanged();
        }

        public void Repeat()
        {
            if (IsRepeatable)
                ResetSelf();

            Continue();
        }

        public void Skip()
        {
            _currentIndex = _dialogs.Length;

            Continue();
        }

        protected virtual void OnIndexChanged()
        {
            IndexChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ResetSelf()
        {
            _currentIndex = -1;
        }

        public void Apply(TutorialPlayer tutorialPlayer)
        {
            if (IsComplete)
            {
                tutorialPlayer.NextButton.gameObject.SetActive(false);
                tutorialPlayer.FinishButton.gameObject.SetActive(true);
                tutorialPlayer.RepeatButton.gameObject.SetActive(IsRepeatable);
            }
            else
            {
                Current.Render(tutorialPlayer);

                tutorialPlayer.NextButton.gameObject.SetActive(true);
                tutorialPlayer.FinishButton.gameObject.SetActive(false);
                tutorialPlayer.RepeatButton.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TutorialPart))]
    public class TutorialPartEditor : Editor
    {
        private SerializedProperty _dialogs;

        private string _tempText;
        private int _currentIndex = -1;

        void OnEnable()
        {
            _dialogs = serializedObject.FindProperty("_dialogs");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var text = GUILayout.TextArea("");

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
            {
                PushDialogBox(serializedObject, _dialogs, text);
            }

            EditorGUILayout.BeginHorizontal();
            if (_currentIndex >= 0 && _dialogs.arraySize > 0)
            {
                if (GUILayout.Button("Remove"))
                {
                    RemoveDialogBox(serializedObject, _dialogs, _currentIndex);
                }
                if (GUILayout.Button("Edit"))
                {
                    EditDialogBox(serializedObject, _dialogs, text, _currentIndex);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();


            for (int i = 0; i < _dialogs.arraySize; i++)
            {
                var element = _dialogs.GetArrayElementAtIndex(i).objectReferenceValue as TutorialDialog;
                if (element)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Edit #" + i))
                    {
                        _tempText = element.Text;
                        _currentIndex = i;
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (i > 0)
                    {
                        if (GUILayout.Button("▲"))
                        {
                            MoveDialog(serializedObject, _dialogs, i, i - 1);
                        }
                    }

                    if (i < _dialogs.arraySize - 1)
                    {
                        if (GUILayout.Button("▼"))
                        {
                            MoveDialog(serializedObject, _dialogs, i, i + 1);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndHorizontal();

                    RenderDialogBox(element);
                }
            }


        }

        private static void MoveDialog(SerializedObject serializedObject, SerializedProperty dialogs, int currentPos, int newPos)
        {
            dialogs.MoveArrayElement(currentPos, newPos);
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private static void EditDialogBox(SerializedObject serializedObject, SerializedProperty dialogs, string text, int index)
        {
            dialogs.InsertArrayElementAtIndex(dialogs.arraySize);

            serializedObject.ApplyModifiedProperties();

            var element = dialogs.GetArrayElementAtIndex(index - 1);

            element.objectReferenceValue = new TutorialDialog { Text = text };

            serializedObject.Update();
        }

        private static void RemoveDialogBox(SerializedObject serializedObject, SerializedProperty dialogs, int index)
        {
            dialogs.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private static void PushDialogBox(SerializedObject serializedObject, SerializedProperty dialogs, string text)
        {
            dialogs.InsertArrayElementAtIndex(dialogs.arraySize);

            serializedObject.ApplyModifiedProperties();

            var element = dialogs.GetArrayElementAtIndex(dialogs.arraySize - 1);

            element.objectReferenceValue = new TutorialDialog { Text = text };

            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }

        private static void RenderDialogBox(TutorialDialog dialog)
        {
            GUILayout.Label(dialog.Text);

        }
    }
#endif
}