using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TileMazeMaker.TileGen;

namespace TileMazeMaker.UI
{
    //Under construction!!!
    public class DragEventInjector : 
        MonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler
    {
        [SerializeField]
        private MapViewer_Streamer m_MapStreamer;
        [SerializeField]
        private Transform m_CahcedTargetTransform;
        private Transform m_MapStramerTransform;
        private float m_ScaleFactor;
        private Vector3 m_TempPosition;
        private int old_x;
        private int old_y;
        
        void Awake()
        {
            m_MapStreamer.onUpdateSightPosition = UpdateSightPositionAfterTeleport;
            m_ScaleFactor = Camera.main.orthographicSize / (Screen.height * 0.5f);
            m_MapStramerTransform = m_MapStreamer.transform;
        }


        void UpdateSightPositionAfterTeleport(int x, int y)
        {
            old_x = x;
            old_y = y;
            Debug.Log("Current Look at " + x + " - " + y);
        }

        public void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {

        }

        public void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            GazeAtCenter();
        }

        private void GazeAtCenter()
        {
            float grid_size = m_MapStreamer.GridSize;
            Vector3 pos = m_MapStramerTransform.InverseTransformPoint(m_CahcedTargetTransform.position);
            int x = Mathf.CeilToInt((pos.x - grid_size / 2) / grid_size);
            int y = Mathf.CeilToInt((pos.z - grid_size / 2) / grid_size);

            //BUG FIXED:解决边界地区不刷新的问题。
            x = Mathf.Clamp(x, 0, m_MapStreamer.Width);
            y = Mathf.Clamp(y, 0, m_MapStreamer.Height);

            Debug.Log("width " + m_MapStreamer.Width + "X " + x + " Y " + y);
            if (x != old_x || y != old_y)
            {
                m_MapStreamer.ShowMapAt(x, y);
                old_x = x;
                old_y = y;
            }
        }
        
        public void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
        {
            m_TempPosition.x = eventData.delta.x;
            m_TempPosition.z = eventData.delta.y;
            
            m_CahcedTargetTransform.Translate( -m_TempPosition * m_ScaleFactor);
            GazeAtCenter();
        }
    }
}

