using Foundation.EntitySystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace MH.Character
{

    public class  ClickEventData
    {
        public Vector3 Position;
        public GameObject GameObject;
    }

    public class PlayerMouseRaycaster : EntityComponent
    {
        [SerializeField] private LayerMask checkLayer;

        private RaycastHit hit;
        private ClickEventData clickEvent;

        private ObjectPool<PointerEventData> pointerDataPool;
        private PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        private List<RaycastResult> raycastResults = new List<RaycastResult>();

        public Action<ClickEventData> OnRightClick;
        public Action<ClickEventData> OnLeftClick;

        #region ---------------- Public Methods -------------

        public override void ManualStart()
        {
            base.ManualStart();

            clickEvent = new ClickEventData();
        }

        public override void ManualUpdate()
        {
            base.ManualLateUpdate();

            CheckToRaycast();
        }

        #endregion

        private void SetUpPool()
        {
            pointerDataPool = new ObjectPool<PointerEventData>(createFunc: () =>new PointerEventData(EventSystem.current),
                                                 actionOnGet: null,
                                                 actionOnRelease: null,
                                                 actionOnDestroy: null,
                                                 collectionCheck: false,
                                                 defaultCapacity: 10,
                                                 maxSize: 100
                                                );
        }

        private void CheckToRaycast()
        {
            if ( Input.GetMouseButtonDown(1) )
            {
                RaycastRightClickedPoint();
            }

            if (Input.GetMouseButtonDown(0))
            {
                RaycastLeftClickedPoint();
            }
        }

        private void RaycastRightClickedPoint()
        {
            //// use physics raycast
            //if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, checkLayer) )
            //{
            //    clickEvent.Position = hit.point;
            //    clickEvent.Object = hit.transform.gameObject;

            //    OnRightClick?.Invoke(clickEvent);
            //}


            if (ExecuteCamRaycast())
            {
                OnRightClick?.Invoke(clickEvent);
            }
        }

        private void RaycastLeftClickedPoint()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, checkLayer))
            {

                clickEvent.Position = hit.point;
                clickEvent.GameObject = hit.transform.gameObject;

                OnLeftClick?.Invoke(clickEvent);
            }
        }

        private bool ExecuteCamRaycast()
        {
            // Create PointerEventData to save raycast data
            //PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            //{
            //    position = Input.mousePosition // position of mouse
            //};
            pointerEventData.position = Input.mousePosition;

            // create list to store results
            //List<RaycastResult> raycastResults = new List<RaycastResult>();
            raycastResults.Clear();


            // Execute raycast
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            // check and get first object which is raycast get
            if (raycastResults.Count == 0)
            {
                return false;
            }

            RaycastResult firstResult = raycastResults[0];

            clickEvent.Position = firstResult.worldPosition;
            clickEvent.GameObject = firstResult.gameObject;
            
            return true;
        }


    }

}
