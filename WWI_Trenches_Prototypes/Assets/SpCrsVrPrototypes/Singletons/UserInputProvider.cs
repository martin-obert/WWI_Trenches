using Assets.IoC;
using Assets.SpCrsVrPrototypes.ComponentDatas;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public class UserInputProvider : MonoBehaviorDependencyResolver
    {
        public GameObject DebugCursor;
        private GameObject _debugCursorInstance;
        private EntityManager _entityManager;
        protected override void OnEnableHandle()
        {
            if (DebugCursor)
                _debugCursorInstance = Instantiate(DebugCursor);

            _entityManager = World.Active.GetOrCreateManager<EntityManager>();
        }

        protected override void OnDestroyHandle()
        {

        }

        public Vector3 CursorPosition { get; private set; }

        void Update()
        {
            if (_debugCursorInstance && (Input.GetMouseButton((int)MouseButton.RightMouse) || Input.GetMouseButton((int)MouseButton.LeftMouse)))
                _debugCursorInstance.transform.position = CursorPosition;
        }

        public void SetCursorPosition(BaseEventData data)
        {
            var clickData = data as PointerEventData;

            if (clickData != null) CursorPosition = clickData.pointerCurrentRaycast.worldPosition;


            var navs = _entityManager.CreateComponentGroup(typeof(Navigation));
            var navigations = navs.GetComponentDataArray<Navigation>();

            for (var i = 0; i < navigations.Length; i++)
            {
                var nav = navigations[i];
                nav.Destination = CursorPosition;
                navigations[i] = nav;
            }

        }
    }
}